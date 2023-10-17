using Microsoft.AspNetCore.Mvc;

namespace Bookify.WEB.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public DashboardController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var numberOfCopies = _context.Books.Count(c => !c.IsDeleted);
            numberOfCopies = numberOfCopies<= 10 ? numberOfCopies:(numberOfCopies/10)*10;
            var numberOfSubscribers = _context.Subscribers.Count(c => !c.IsDeleted);
            var lastAddedBooks = _context.Books.Where(b => !b.IsDeleted)
                .OrderByDescending(b => b.Id).Take(8).ToList();
            var topBooks = _context.RentalCopies
                .Include(r => r.BookCopy).ThenInclude(c => c.Book).ThenInclude(b => b.Author).
                GroupBy(c => new
                {
                    c.BookCopy!.BookId,
                    c.BookCopy.Book.Title,
                    c.BookCopy.Book.ImageThumbnailUrl,
                    AuthorName = c.BookCopy.Book.Author.Name
                }).Select(b => new
                {
                    b.Key.BookId,
                    b.Key.Title,
                    b.Key.ImageThumbnailUrl,
                    b.Key.AuthorName,
                    Count = b.Count()
                }).OrderByDescending(b => b.Count)
                .Take(6).Select(b => new BookViewModel
                {
                    Id = b.BookId,
                    Title = b.Title,
                    ImageThumbnailUrl = b.ImageThumbnailUrl,
                    Author = b.AuthorName
                }).ToList();
            var viewModel = new DashboardViewModel()
            {
                NumberOfCopies = numberOfCopies,
                NumberOfSubscribers = numberOfSubscribers,
                LastAddedBooks = _mapper.Map<IEnumerable<BookViewModel>>(lastAddedBooks),
                TopBooks = topBooks
            };
            return View(viewModel);
        }
        [AjaxOnly]
        public IActionResult GetRentalsPerDay(DateTime? startDate,DateTime? endDate)
        {
            startDate ??= DateTime.Today.AddDays(-29);
            endDate ??= DateTime.Today;
            var data = _context.RentalCopies.Where(c =>c.RentalDate >= startDate && c.RentalDate <= endDate)
                .GroupBy(c => new {Date = c.RentalDate}).Select(c=> new ChartItemViewModel
                {
                    Label = c.Key.Date.ToString("d MMM"),
                    Value = c.Count().ToString()
                }).ToList();
            List<ChartItemViewModel> figures = new();
            for(DateTime day = (DateTime)startDate;day <= endDate;day= day.AddDays(1))
            {
                var dayData = data.SingleOrDefault(d => d.Label == day.ToString("d MMM"));
                ChartItemViewModel item = new()
                {
                    Label = day.ToString("d MMM"),
                    Value = dayData is null ? "0" : dayData.Value
                };
                figures.Add(item);
            }
            return Ok(figures);
        }
        [AjaxOnly]
        public IActionResult GetSubscribersPerCity()
        {
            var data = _context.Subscribers.Include(s => s.Governerate)
                .GroupBy(s => new { GovernerateName = s.Governerate.Name })
                .Select(g => new ChartItemViewModel
                {
                    Label = g.Key.GovernerateName,
                    Value = g.Count().ToString()
                }).ToList();
            return Ok(data);
        }
    }
}
