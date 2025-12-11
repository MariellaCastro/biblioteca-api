using AutoMapper;
using UniversityLibrary.Application.DTOs.Book;
using UniversityLibrary.Application.DTOs.Loan;
using UniversityLibrary.Domain.Entities;

namespace UniversityLibrary.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Book, BookDto>();
            
            CreateMap<CreateBookDto, Book>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Loans, opt => opt.Ignore());
            
            CreateMap<Loan, LoanDto>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive()))
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book != null ? src.Book.Title : string.Empty))
                .ForMember(dest => dest.BookAuthor, opt => opt.MapFrom(src => src.Book != null ? src.Book.Author : string.Empty))
                .ForMember(dest => dest.BookISBN, opt => opt.MapFrom(src => src.Book != null ? src.Book.ISBN : string.Empty));
            
            CreateMap<CreateLoanDto, Loan>()
                .ForMember(dest => dest.LoanDate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Active"))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.ReturnDate, opt => opt.MapFrom(src => (DateTime?)null))
                .ForMember(dest => dest.Book, opt => opt.Ignore());
            
        }
    }
}