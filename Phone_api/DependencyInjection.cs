﻿using Microsoft.EntityFrameworkCore;
using Phone_api.Data;
using Phone_api.Repositories;
using Phone_api.Services;

namespace Phone_api
{
    /// <summary>
    /// Cấu hình Dependency Injection
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PhoneContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<IPhoneRepository, PhoneRepository>();
            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<IPhoneService, PhoneService>();
            services.AddScoped<IBrandService, BrandService>();
            return services;
        }
    }
}
