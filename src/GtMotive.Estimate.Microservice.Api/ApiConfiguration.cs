using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using GtMotive.Estimate.Microservice.Api.Authorization;
using GtMotive.Estimate.Microservice.Api.Filters;
using GtMotive.Estimate.Microservice.Api.UseCases.CreateVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.GetVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.ListVehicles;
using GtMotive.Estimate.Microservice.Api.UseCases.RentVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.ReturnVehicle;
using GtMotive.Estimate.Microservice.ApplicationCore;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.CreateVehicle;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.GetVehicle;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ListVehicles;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.RentVehicle;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ReturnVehicle;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: CLSCompliant(false)]

namespace GtMotive.Estimate.Microservice.Api
{
    [ExcludeFromCodeCoverage]
    public static class ApiConfiguration
    {
        public static void ConfigureControllers(MvcOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            options.Filters.Add<BusinessExceptionFilter>();
        }

        public static IMvcBuilder WithApiControllers(this IMvcBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.AddApplicationPart(typeof(ApiConfiguration).GetTypeInfo().Assembly);

            AddApiDependencies(builder.Services);

            return builder;
        }

        public static void AddApiDependencies(this IServiceCollection services)
        {
            services.AddAuthorization(AuthorizationOptionsExtensions.Configure);
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApiConfiguration).GetTypeInfo().Assembly));
            services.AddUseCases();
            services.AddPresenters();
        }

        public static IServiceCollection AddPresenters(this IServiceCollection services)
        {
            services.AddScoped<CreateVehiclePresenter>();
            services.AddScoped<ICreateVehicleOutputPort>(sp => sp.GetRequiredService<CreateVehiclePresenter>());

            services.AddScoped<ListVehiclesPresenter>();
            services.AddScoped<IListVehiclesOutputPort>(sp => sp.GetRequiredService<ListVehiclesPresenter>());

            services.AddScoped<GetVehiclePresenter>();
            services.AddScoped<IGetVehicleOutputPort>(sp => sp.GetRequiredService<GetVehiclePresenter>());

            services.AddScoped<RentVehiclePresenter>();
            services.AddScoped<IRentVehicleOutputPort>(sp => sp.GetRequiredService<RentVehiclePresenter>());

            services.AddScoped<ReturnVehiclePresenter>();
            services.AddScoped<IReturnVehicleOutputPort>(sp => sp.GetRequiredService<ReturnVehiclePresenter>());

            return services;
        }
    }
}
