using Bookify.WEB.Core.consts;
using Bookify.WEB.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace Bookify.WEB.Core.mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
			//Categories
			CreateMap<Category, CategoryViewModel>();
			CreateMap<CategoryFormViewModel, Category>().ReverseMap();
			CreateMap<Category, SelectListItem>()
				.ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));

			//Authors
			CreateMap<Author, AuthorViewModel>();
			CreateMap<AuthorFormViewModel, Author>().ReverseMap();
			CreateMap<Author, SelectListItem>()
				.ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));

			//Books
			CreateMap<BookFormViewModel, Book>()
				.ReverseMap()
				.ForMember(dest => dest.Categories, opt => opt.Ignore());

			CreateMap<Book, BookViewModel>()
				.ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author!.Name))
				.ForMember(dest => dest.Categories,
					opt => opt.MapFrom(src => src.Categories.Select(c => c.Category!.Name).ToList()));

			CreateMap<BookCopy, BookCopyViewModel>()
				.ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book!.Title))
				.ForMember(dest => dest.BookId, opt => opt.MapFrom(src => src.Book!.Id))
				.ForMember(dest => dest.BookThumbnailUrl, opt => opt.MapFrom(src => src.Book!.ImageThumbnailUrl));

			CreateMap<BookCopy, BookCopyFormViewModel>();

			//Users
			CreateMap<ApplicationUser, UserViewModel>();
			CreateMap<UserFormViewModel, ApplicationUser>()
				.ForMember(dest => dest.NormalizedEmail, opt => opt.MapFrom(src => src.Email.ToUpper()))
				.ForMember(dest => dest.NormalizedUserName, opt => opt.MapFrom(src => src.UserName.ToUpper()))
				.ReverseMap();

			//Governerates & Areas
			CreateMap<Governerate, SelectListItem>()
				.ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));

			CreateMap<Area, SelectListItem>()
				.ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));

			//Subscribers
			CreateMap<Subscriber, SubscriberFormViewModel>().ReverseMap();

			CreateMap<Subscriber, SubscriberSearchResultViewModel>()
				.ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

			CreateMap<Subscriber, SubscriberViewModel>()
				.ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
				.ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Area!.Name))
				.ForMember(dest => dest.Governerate, opt => opt.MapFrom(src => src.Governerate!.Name));

			CreateMap<Subscribtion, SubscribtionViewModel>()
				.ForMember(dest => dest.Status , opt => opt.MapFrom(src => src.EndDate < DateTime.Today ? SubscriberStatus.InActive : SubscriberStatus.Active));

			//Rentals
			CreateMap<Rental, RentalViewModel>();
			CreateMap<RentalCopy, RentalCopyViewModel>();
		}
    }
}
