namespace Bookify.WEB.Controllers
{
    [Authorize]
    public class AuthorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public AuthorsController(ApplicationDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }
        public IActionResult Index()
        {
            var authors = _context.Authors.AsNoTracking().ToList();
            var viewModel = _mapper.Map<IEnumerable<AuthorViewModel>>(authors);
            return View(viewModel);
        }
        [AjaxOnly]
        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_Form");
        }
        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AuthorFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var author = _mapper.Map<Author>(model);
            _context.Authors.Add(author);
            _context.SaveChanges();
            var viewModel = _mapper.Map<AuthorViewModel>(author);
            return PartialView("_AuthorRow", viewModel);
        }

        [AjaxOnly]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var author = _context.Authors.Find(id);
            if (author == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<AuthorFormViewModel>(author);
            return PartialView("_Form", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(AuthorFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var author = _context.Authors.Find(model.Id);
            _mapper.Map(model, author);
            author!.LastUpdatedOn = DateTime.Now;
            _context.SaveChanges();
            var viewModel = _mapper.Map<AuthorViewModel>(author);
            //TempData["Message"] = "Saved Successfully";
            return PartialView("_AuthorRow", viewModel);
        }

        [AjaxOnly]
        [HttpDelete]
        public IActionResult ToggleStatus(int id)
        {
            var author = _context.Authors.Find(id);
            if (author == null)
            {
                return NotFound();
            }
            author.IsDeleted = !author.IsDeleted;
            author.LastUpdatedOn = DateTime.Now;
            _context.SaveChanges();
            return Ok(author.LastUpdatedOn.ToString());
        }

        public IActionResult AllowItem(AuthorFormViewModel model)
        {
            var author = _context.Authors.SingleOrDefault(e => e.Name == model.Name);
            bool isAllowed = author == null || author.Id == model.Id;
            return Json(isAllowed);
        }
    }
}
