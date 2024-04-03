using AllupProject.Business.Interfaces;
using PustokTemp.Business.Implementations;

namespace AllupProject;

public static class ServiceRegistration
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<ISliderService, SliderService>();
    }
}
