using AutoMapper;

namespace Bookify.WEB.Core.mappings
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryViewModel>();
            CreateMap<CategoryFormViewModel,Category>().ReverseMap();
        }
    }
}
