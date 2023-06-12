
namespace Bookify.WEB.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public CategoriesController(ApplicationDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }
        public IActionResult Index()
        {
            var categories = _context.Categories.AsNoTracking().ToList();
            var viewModel = _mapper.Map<IEnumerable<CategoryViewModel>>(categories);
            return View(viewModel);
        }
        [AjaxOnly]
        public IActionResult Create()
        {
            return PartialView("_Form");
        }
        [HttpPost]
        [AjaxOnly]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var category = _mapper.Map<Category>(model);
            _context.Categories.Add(category);
            _context.SaveChanges();
            var viewModel = _mapper.Map<CategoryViewModel>(category);

            return PartialView("_CategoryRow", viewModel);
        }
        [HttpGet]
        [AjaxOnly]
        public IActionResult Edit(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return BadRequest();
            }
            var model = _mapper.Map<CategoryFormViewModel>(category);
            return PartialView("_Form", model);
        }
        [HttpPost]
        [AjaxOnly]
        public IActionResult Edit(CategoryFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("_Form", viewModel);
            }
            var category = _context.Categories.Find(viewModel.Id);
            _mapper.Map(viewModel, category);
            category!.LastUpdatedOn = DateTime.Now;
            _context.SaveChanges();
            TempData["Message"] = "Saved Successfully";
            var model = _mapper.Map<CategoryViewModel>(category);
            return PartialView("_CategoryRow", model);
        }
        [HttpDelete]
        [ValidateAntiForgeryToken]

        public IActionResult ToggleStatus(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            category.IsDeleted = !category.IsDeleted;
            category.LastUpdatedOn = DateTime.Now;
            _context.SaveChanges();
            return Ok(category.LastUpdatedOn.ToString());
        }
        public IActionResult AllowItem(CategoryFormViewModel model)
        {
            var category = _context.Categories.SingleOrDefault(e => e.Name == model.Name);
            var isAllowed = category == null || category.Id.Equals(model.Id);
            return Json(isAllowed);
        }
    }
}
