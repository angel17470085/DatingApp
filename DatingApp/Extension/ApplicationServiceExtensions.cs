using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.Data;
using DatingApp.Interfaces;
using DatingApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DatingApp.Extension
{
    public static class ApplicationServiceExtensions
    {
      
      public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config )
      {
            services.AddScoped<ITokenService, TokenService>();
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));    
            return services;
      }
    }
}