using Microsoft.Extensions.DependencyInjection;
using UniversityLibrary.Application.Interfaces;
using UniversityLibrary.Application.Services;
using UniversityLibrary.Application.Mappings;

namespace UniversityLibrary.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));

            services.AddScoped<IBookService, BookService>();
            services.AddScoped<ILoanService, LoanService>();

            return services;
        }
    }
}