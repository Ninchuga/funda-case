using Funda.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Funda.DAL.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDalServices(this IServiceCollection services)
        {
            services.AddScoped<IMakelaarRepository, MakelaarRepository>();
            return services;
        }
    }
}
