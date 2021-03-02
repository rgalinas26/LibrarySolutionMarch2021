using AutoMapper;
using LibraryApi.Domain;
using LibraryApi.Models.Books;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryApi.AutoMapperProfiles
{
    public class BooksProfile : Profile
    {
        public BooksProfile()
        {
            CreateMap<Book, GetBookDetailsResponse>();
            CreateMap<Book, BookSummaryItem>();
            CreateMap<PostBookRequest, Book>()
                .ForMember(dest => dest.AddedToInventory, cfg => cfg.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.IsAvailable, cfg => cfg.MapFrom(_ => true));
        }
    }
}
