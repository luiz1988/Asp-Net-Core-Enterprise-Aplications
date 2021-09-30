using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSE.WebApp.MVC.Extentions;
using NSE.WebApp.MVC.Service;
using NSE.WebApp.MVC.Service.Handlers;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NSE.WebApp.MVC.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IValidationAttributeAdapterProvider, CpfValidationAttributeAdapterProvider>();

            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

            services.AddHttpClient<IAutenticacaoService, AutenticacaoService>();

            services.AddHttpClient<ICatalogoService, CatalogoService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
            .AddPolicyHandler(PollyExtensions.EsperarTentar())
             .AddTransientHttpErrorPolicy(
                p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

            // .AddTransientHttpErrorPolicy(
            //p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));

            //services.AddHttpClient("Refit", options =>
            //{
            //    options.BaseAddress = new Uri(configuration.GetSection("CatalogoUrl").Value);
            //})
            //    .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
            //    .AddTypedClient(Refit.RestService.For<ICatalogoServiceRefit>);



            //.AddPolicyHandler(PollyExtensions.EsperarTentar())
            //.AddTransientHttpErrorPolicy(
            //    p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));


            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<IUser, AspNetUser>();

        }

        public class PollyExtensions
        {
            public static AsyncRetryPolicy<HttpResponseMessage> EsperarTentar()
            {
                var retry = HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(new[]
                    {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10),
                    }, (outcome, timespan, retryCount, context) =>
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine($"Tentando pela {retryCount} vez!");
                        Console.ForegroundColor = ConsoleColor.White;
                    });

                return retry;
            }
        }
    }
}
