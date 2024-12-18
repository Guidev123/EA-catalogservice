﻿using CatalogService.Application.DTOs;
using CatalogService.Application.Mappers;
using CatalogService.Application.Responses;
using CatalogService.Application.UseCases.Interfaces;

namespace CatalogService.API.Endpoints.ProductEndpoints
{
    public class GetProductByIdEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app) => app.MapGet("/{id}", HandleAsync).Produces<Response<ProductDTO>>();
        public static async Task<IResult> HandleAsync(IProductUseCase productUseCase, string id)
        {
            var result = await productUseCase.GetProductByIdAsync(id);

            if (result.IsSuccess && result.Data is not null)
                return TypedResults.Ok(new Response<ProductDTO>(result.Data.MapFromEntity()));

            return TypedResults.BadRequest(result);
        }
    }
}
