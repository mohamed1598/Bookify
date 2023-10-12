using Bookify.WEB.Core.consts;
using Bookify.WEB.Data.Migrations;
using Hangfire;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Bookify.WEB.Controllers
{
    [Authorize(Roles = AppRoles.Reception)]
    public class SubscribersController : Controller
    {
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;
		private readonly IImageService _imageService;
		private readonly IDataProtector _dataProtector;
        private readonly IEmailBodyBuilder _emailBodyBuilder;
        private readonly IEmailSender _emailSender;
        public SubscribersController(ApplicationDbContext context, IMapper mapper, IImageService imageService, IDataProtectionProvider dataProtectionProvider, IEmailSender emailSender ,IEmailBodyBuilder emailBodyBuilder)
        {
            _context = context;
            _mapper = mapper;
            _imageService = imageService;
            _dataProtector = dataProtectionProvider.CreateProtector("MySecureKey");
            _emailSender = emailSender;
			_emailBodyBuilder = emailBodyBuilder;
        }

        public IActionResult Index()
        {
            return View();
        }
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Search(SearchFormViewModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var subscriber = _context.Subscribers
							.SingleOrDefault(s =>
									s.Email == model.Value
								|| s.NationalId == model.Value
								|| s.MobileNumber == model.Value);

			var viewModel = _mapper.Map<SubscriberSearchResultViewModel>(subscriber);
			if(subscriber is not null)
				viewModel.Key = _dataProtector.Protect(subscriber.Id.ToString());
			return PartialView("_Result", viewModel);
		}
		
		public IActionResult Details(string id)
		{
			int subscriberId = int.Parse(_dataProtector.Unprotect(id));
            var subscriber = _context.Subscribers
                .Include(s => s.Governerate)
                .Include(s => s.Area)
                .Include(s => s.Subscribtions)
                .Include(s => s.Rentals)
                .ThenInclude(r => r.RentalCopies)
                .SingleOrDefault(s => s.Id == subscriberId);
			//var subscriber = _context.Subscribers
			//	.Include(s => s.Governerate)
			//	.Include(s => s.Area)
			//	.Include(s => s.Subscribtions)
			//	.Include(s => s.Rentals.Where(r => !r.IsDeleted))
			//	.ThenInclude(r => r.RentalCopies)
   //             .SingleOrDefault(s => s.Id == subscriberId);

            if (subscriber is null)
				return NotFound();
			
			var viewModel = _mapper.Map<SubscriberViewModel>(subscriber);
            if (subscriber.IsBlackListed)
            {
                foreach (var sub in viewModel.Subscribtions)
					sub.Status = SubscriberStatus.Banned;
            }
            viewModel.Key = id;
			return View(viewModel);
		}

		public IActionResult Create()
		{
			var viewModel = PopulateViewModel();
			return View("Form", viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(SubscriberFormViewModel model)
		{
			if (!ModelState.IsValid)
				return View("Form", PopulateViewModel(model));

			var subscriber = _mapper.Map<Subscriber>(model);

			var imageName = $"{Guid.NewGuid()}{Path.GetExtension(model.Image!.FileName)}";
			var imagePath = "/images/Subscribers";

			var (isUploaded, errorMessage) = await _imageService.UploadAsync(model.Image, imageName, imagePath, hasThumbnail: true);

			if (!isUploaded)
			{
				ModelState.AddModelError("Image", errorMessage!);
				return View("Form", PopulateViewModel(model));
			}

			Subscribtion subscribtion = new()
			{
				CreatedById = subscriber.CreatedById,
				CreatedOn = subscriber.CreatedOn,
				StartDate = DateTime.Today,
				EndDate = DateTime.Today.AddYears(1)
			};
			subscriber.Subscribtions.Add(subscribtion);
			subscriber.ImageUrl = $"{imagePath}/{imageName}";
			subscriber.ImageThumbnailUrl = $"{imagePath}/thumb/{imageName}";
			subscriber.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

			_context.Add(subscriber);
			_context.SaveChanges();

			//TODO: Send welcome email
			var placehoders = new Dictionary<string, string>()
			{
				{"imageUrl","https://res.cloudinary.com/devcreed/image/upload/v1668732314/icon-positive-vote-1_rdexez.svg" },
				{"header",$"Welcome {model.FirstName}," },
				{"body","thanks for joining bookify" }
			};
			var body = _emailBodyBuilder.GetEmailBody(EmailTemplates.Notification, placehoders);
			BackgroundJob.Enqueue(
				() => _emailSender.SendEmailAsync(model.Email,"Welcome to bookify",body)
				); 
			var subsciberId = _dataProtector.Protect(subscriber.Id.ToString());

			return RedirectToAction(nameof(Details), new { id = subsciberId });
		}

		public IActionResult Edit(string id)
		{
			var subscriberId = int.Parse(_dataProtector.Unprotect(id));
			var Subscriber = _context.Subscribers.Find(subscriberId);

			if (Subscriber is null)
				return NotFound();

			var model = _mapper.Map<SubscriberFormViewModel>(Subscriber);
			var viewModel = PopulateViewModel(model);
			viewModel.Key = id; 
			return View("Form", viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(SubscriberFormViewModel model)
		{
			if (!ModelState.IsValid)
				return View("Form", PopulateViewModel(model));
			var id = int.Parse(_dataProtector.Unprotect(model.Key!));
			var Subscriber = _context.Subscribers.Find(id);

			if (Subscriber is null)
				return NotFound();

			if (model.Image is not null)
			{
				if (!string.IsNullOrEmpty(Subscriber.ImageUrl))
					_imageService.Delete(Subscriber.ImageUrl, Subscriber.ImageThumbnailUrl);

				var imageName = $"{Guid.NewGuid()}{Path.GetExtension(model.Image.FileName)}";
				var imagePath = "/images/Subscribers";

				var (isUploaded, errorMessage) = await _imageService.UploadAsync(model.Image, imageName, imagePath, hasThumbnail: true);

				if (!isUploaded)
				{
					ModelState.AddModelError("Image", errorMessage!);
					return View("Form", PopulateViewModel(model));
				}

				model.ImageUrl = $"{imagePath}/{imageName}";
				model.ImageThumbnailUrl = $"{imagePath}/thumb/{imageName}";
			}

			else if (!string.IsNullOrEmpty(Subscriber.ImageUrl))
			{
				model.ImageUrl = Subscriber.ImageUrl;
				model.ImageThumbnailUrl = Subscriber.ImageThumbnailUrl;
			}

			Subscriber = _mapper.Map(model, Subscriber);
			Subscriber.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
			Subscriber.LastUpdatedOn = DateTime.Now;

			_context.SaveChanges();

			return RedirectToAction(nameof(Details), new { id = model.Key! });
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult RenewSubscribtion(string sKey)
		{
			var subscriberId = int.Parse(_dataProtector.Unprotect(sKey));
			var subscriber = _context.Subscribers.Include(s => s.Subscribtions).SingleOrDefault(s => s.Id == subscriberId);
			if(subscriber is null)
			{
				return NotFound();
			}
			if (subscriber.IsBlackListed)
			{
				return BadRequest();
			}
			var lastSubscribtion = subscriber.Subscribtions.LastOrDefault();
			var startDate = lastSubscribtion is null ? DateTime.Today : lastSubscribtion.EndDate < DateTime.Today ? DateTime.Today : lastSubscribtion.EndDate.AddDays(1);
			Subscribtion newSubscription = new()
			{
				CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value,
				CreatedOn = DateTime.Now,
				StartDate = startDate,
				EndDate = startDate.AddYears(1) 
			};
			subscriber.Subscribtions.Add(newSubscription);
			_context.SaveChanges();

			var viewModel = _mapper.Map<SubscribtionViewModel>(newSubscription);
			return PartialView("_SubscriptionRow", viewModel);
		}

		[AjaxOnly]
		public IActionResult GetAreas(int GovernerateId)
		{
			var areas = _context.Areas
					.Where(a => a.GovernerateId == GovernerateId && !a.IsDeleted)
					.OrderBy(g => g.Name)
					.ToList();

			return Ok(_mapper.Map<IEnumerable<SelectListItem>>(areas));
		}

		public IActionResult AllowNationalId(SubscriberFormViewModel model)
		{
			int id = String.IsNullOrEmpty(model.Key)?0:int.Parse(_dataProtector.Unprotect(model.Key!));
			var Subscriber = _context.Subscribers.SingleOrDefault(b => b.NationalId == model.NationalId);
			var isAllowed = Subscriber is null || Subscriber.Id.Equals(id);

			return Json(isAllowed);
		}

		public IActionResult AllowMobileNumber(SubscriberFormViewModel model)
		{
            int id = String.IsNullOrEmpty(model.Key) ? 0 : int.Parse(_dataProtector.Unprotect(model.Key!));
            var Subscriber = _context.Subscribers.SingleOrDefault(b => b.MobileNumber == model.MobileNumber);
			var isAllowed = Subscriber is null || Subscriber.Id.Equals(id);

			return Json(isAllowed);
		}

		public IActionResult AllowEmail(SubscriberFormViewModel model)
		{
            int id = String.IsNullOrEmpty(model.Key) ? 0 : int.Parse(_dataProtector.Unprotect(model.Key!));
            var Subscriber = _context.Subscribers.SingleOrDefault(b => b.Email == model.Email);
			var isAllowed = Subscriber is null || Subscriber.Id.Equals(id);

			return Json(isAllowed);
		}

		private SubscriberFormViewModel PopulateViewModel(SubscriberFormViewModel? model = null)
		{
			SubscriberFormViewModel viewModel = model is null ? new SubscriberFormViewModel() { Key = "" } : model;
			var Governerates = _context.Governerates.Where(a => !a.IsDeleted).OrderBy(a => a.Name).ToList();
			viewModel.Governerates = _mapper.Map<IEnumerable<SelectListItem>>(Governerates);

			if (model?.GovernerateId > 0)
			{
				var areas = _context.Areas
					.Where(a => a.GovernerateId == model.GovernerateId && !a.IsDeleted)
					.OrderBy(a => a.Name)
					.ToList();

				viewModel.Areas = _mapper.Map<IEnumerable<SelectListItem>>(areas);
			}

			return viewModel;
		}
	}
}
