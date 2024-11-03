﻿using CatalogService.API.Endpoints.ProductEndpoints;

namespace CatalogService.API.Endpoints
{
    public static class Endpoint
    {
        public static void MapEndpoints(this WebApplication app)
        {
            var endpoints = app
                .MapGroup("");

            endpoints.MapGroup("api/v1/catalog")
                .WithTags("Catalog")
                .MapEndpoint<CreateProductEndpoint>()
                .MapEndpoint<GetProductByIdEndpoint>()
                .MapEndpoint<GetAllProductsEndpoint>();
        }

        private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
            where TEndpoint : IEndpoint
        {
            TEndpoint.Map(app);
            return app;
        }
    }
}