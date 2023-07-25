using Bookify.WEB.Core.consts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace Bookify.WEB.Controllers
{
    [Authorize(Roles = AppRoles.Admin)]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager; 
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public UsersController(UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager, IEmailSender emailSender, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _emailSender = emailSender;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var viewModel = _mapper.Map<IEnumerable<UserViewModel>>(users);
            return View(viewModel);
        }
        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> Create()
        {
            var viewModel = new UserFormViewModel()
            {
                Id= "",
                Password = "",
                ConfirmPassword = "",
                Roles = await _roleManager.Roles.Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                }).ToListAsync()
        };
            return PartialView("_Form",viewModel); 
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            ApplicationUser user = new()
            {
                FullName = model.FullName,
                Email = model.Email,
                UserName = model.UserName,
                CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            };
            var result = await _userManager.CreateAsync(user,model.Password!);
            if (result.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, model.SelectedRoles);

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new {area = "Identity",userId = user.Id,code },
                    protocol: Request.Scheme
                    );
                var filePath = $"{_webHostEnvironment.WebRootPath}/templates/email.html";
                StreamReader str = new(filePath);
                var body = str.ReadToEnd();
                str.Close();
                //var url = Url.Action("Index", "Home", null, Request.Scheme);

                body = body
                    .Replace("[imageUrl]", "https://media.licdn.com/dms/image/C4E0BAQGJnY1knbLaZQ/company-logo_200_200/0/1679659398596?e=2147483647&v=beta&t=XX1efXhyNp5iltrU8cZjRK9cMN2gcQQFfCw8QzRlK7M")
                    .Replace("[header]", $"Hey {user.FullName}, thanks for joining us!")
                    .Replace("[body]", "Please confirm your email")
                    .Replace("[url]", HtmlEncoder.Default.Encode(callbackUrl!))
                    .Replace("[linkTitle]", "Active Account!");
                await _emailSender.SendEmailAsync(user.Email, "Confirm Your Email", body);
                var viewModel = _mapper.Map<UserViewModel>(user);
                return PartialView("_UserRow", viewModel);
            }
            return BadRequest(string.Join(',',result.Errors.Select(e=> e.Description)));
        }
        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user is null)
            {
                return NotFound();
            }
            var viewModel = new ResetPasswordFormViewModel()
            {
                Id = id
            };
            return PartialView("_ResetPasswordForm", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var user = await _userManager.FindByIdAsync(model.Id!);
            if (user is null)
                return NotFound();
            var currentPasswordHash = user.PasswordHash;

            await _userManager.RemovePasswordAsync(user);

            var result = await _userManager.AddPasswordAsync(user, model.Password);
            if (result.Succeeded)
            {
                user.LastUpdatedOn = DateTime.Now;
                user.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                await _userManager.UpdateAsync(user);
                var viewModel = _mapper.Map<UserViewModel>(user);
                return PartialView("_UserRow", viewModel);
            }
            user.PasswordHash = currentPasswordHash;
            await _userManager.UpdateAsync(user);
            return BadRequest(string.Join(',', result.Errors.Select(e => e.Description)));
        }

        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var viewModel = _mapper.Map<UserFormViewModel>(user);
            viewModel.Roles = await _roleManager.Roles.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name
            }).ToListAsync();
            viewModel.SelectedRoles = await _userManager.GetRolesAsync(user);
            return PartialView("_Form", viewModel);
        }
        [HttpPost]
        [AjaxOnly]
        public async Task<IActionResult> Edit(UserFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.FindByIdAsync(model.Id!);

            if (user is null)
                return NotFound();

            user = _mapper.Map(model, user);
            user.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            user.LastUpdatedOn = DateTime.Now;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);

                var rolesUpdated = !currentRoles.SequenceEqual(model.SelectedRoles);

                if (rolesUpdated)
                {
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    await _userManager.AddToRolesAsync(user, model.SelectedRoles);
                }
                await _userManager.UpdateSecurityStampAsync(user);
                var viewModel = _mapper.Map<UserViewModel>(user);
                return PartialView("_UserRow", viewModel);
            }

            return BadRequest(string.Join(',', result.Errors.Select(e => e.Description)));
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            user.IsDeleted = !user.IsDeleted;
            user.LastUpdatedOn = DateTime.Now;
            user.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            await _userManager.UpdateAsync(user);
            if (user.IsDeleted)
            {
                await _userManager.UpdateSecurityStampAsync(user);
            }
            return Ok(user.LastUpdatedOn.ToString());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            //user.LockoutEnd = DateTime.UtcNow;
            //await _userManager.UpdateAsync(user);
            //return Ok(user.LockoutEnd);
            var isLocked = await _userManager.IsLockedOutAsync(user);
            if (isLocked)
                await _userManager.SetLockoutEndDateAsync(user, null);
            return Ok();
        }



        public async Task<IActionResult> AllowUserName(UserFormViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            var isAllowed = user is null || user.Id.Equals(model.Id);
            return Json(isAllowed);
        }
        public async Task<IActionResult> AllowEmail(UserFormViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var isAllowed = user is null || user.Id.Equals(model.Id);
            return Json(isAllowed);
        }
    }
}
