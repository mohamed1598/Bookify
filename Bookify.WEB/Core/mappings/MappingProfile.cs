namespace Bookify.WEB.Core.mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Category Mapping
            CreateMap<Category, CategoryViewModel>();
            CreateMap<CategoryFormViewModel, Category>().ReverseMap();
            //Author Mapping
            CreateMap<Author, AuthorViewModel>();
            CreateMap<AuthorFormViewModel, Author>().ReverseMap();
        }
    }
}
