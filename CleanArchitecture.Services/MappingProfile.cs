using AutoMapper;
using CleanArchitecture.DataAccess.Models;
using CleanArchitecture.Services.DTOs.Book;

namespace CleanArchitecture.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Book, BookReadDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.BookCategory.Name));

            CreateMap<BookCreateDTO, Book>();
            CreateMap<BookUpdateDTO, Book>();
        }
    }

}
