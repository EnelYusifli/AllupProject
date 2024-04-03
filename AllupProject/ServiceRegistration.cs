﻿using AllupProject.Business.Implementations;
using AllupProject.Business.Interfaces;

namespace AllupProject;

public static class ServiceRegistration
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<ISliderService, SliderService>();
        services.AddScoped<IBannerService, BannerService>();
        services.AddScoped<ICategoryService, CategoryService>();
    }
}
