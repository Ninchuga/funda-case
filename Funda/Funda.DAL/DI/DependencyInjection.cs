using Funda.DAL.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Funda.DAL.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDalServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IFundaPropertiesClient, FundaPropertiesClient>(client =>
            {
                string fundaBaseUrl = configuration.GetValue<string>("Funda:BaseUrl");
                string fundaUrlKey = configuration.GetValue<string>("Funda:ApiKey");

                client.BaseAddress = new Uri($"{fundaBaseUrl}/{fundaUrlKey}");
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            });

            return services;
        }
    }
}
