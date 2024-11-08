﻿using CatalogService.API.DTOs;
using CatalogService.Domain.Entities;
using CatalogService.Domain.Entities.Validations;
using CatalogService.Domain.Repositories;
using CatalogService.Domain.Responses;
using CatalogService.Domain.Responses.Messages;
using CatalogService.Domain.Services;
using CatalogService.Infrastructure.CacheStorage;
using FluentValidation;
using FluentValidation.Results;

namespace CatalogService.API.Services
{
    public class ProductService(IProductRepository productRepository, ICacheService cacheService) : IProductService
    {
        private readonly IProductRepository _productRepository = productRepository;
        private readonly ICacheService _cacheService = cacheService;

        public async Task<Response<List<Product>>> GetAllProductsAsync(int pageNumber, int pageSize)
        {
            var cacheKey = $"products_{pageNumber}_{pageSize}";
            var cacheProduct = await _cacheService.GetAsync<List<GetAllProductsDTO>>(cacheKey);
            if (cacheProduct is not null)
            {
                var result = cacheProduct.Select(GetAllProductsDTO.MapToEntity).ToList();
                return new Response<List<Product>>(result, 200, ResponseMessages.VALID_OPERATION.GetDescription());
            }

            var products = await _productRepository.GetAllProductsAsync(pageNumber, pageSize);
            if (products is null || products.Count == 0)
                return new Response<List<Product>>(null, 404, ResponseMessages.INVALID_OPERATION.GetDescription());

            await _cacheService.SetAsync(cacheKey, products);

            return new Response<List<Product>>(products, 200, ResponseMessages.VALID_OPERATION.GetDescription());
        }

        public async Task<Response<Product>> GetProductByIdAsync(string id)
        {
            var cacheProduct = await _cacheService.GetAsync<ProductDTO>(id);

            if (cacheProduct is not null)
                return new Response<Product>(ProductDTO.MapToEntity(cacheProduct));

            var product = await _productRepository.GetProductByIdAsync(id);
            if (product is null) return new Response<Product>(null, 404, ResponseMessages.INVALID_OPERATION.GetDescription());

            await _cacheService.SetAsync(id, product);

            return new Response<Product>(product, 200, ResponseMessages.VALID_OPERATION.GetDescription());
        }

        public async Task<Response<Product>> CreateProductAsync(Product product)
        {
            var validationResult = ValidateEntity(new ProductValidation(), product);

            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return new Response<Product>(null, 400, errors);
            }

            await _productRepository.CreateProductAsync(product);
            return new Response<Product>(null, 201, ResponseMessages.VALID_OPERATION.GetDescription());
        }

        public async Task<Response<Product>> DeleteProductAsync(string id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product is null)
                return new Response<Product>(null, 404, ResponseMessages.INVALID_OPERATION.GetDescription());

            product.SetProductAsDeleted();
            await _productRepository.UpdateProductAsync(product);
            return new Response<Product>(null, 204, ResponseMessages.VALID_OPERATION.GetDescription());
        }

        public async Task<Response<Product>> UpdateProductAsync(Product product, string id)
        {
            var validationResult = ValidateEntity(new ProductValidation(), product);

            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return new Response<Product>(null, 400, errors);
            }

            var oldProduct = await _productRepository.GetProductByIdAsync(id);
            oldProduct.UpdateProduct(product);

            await _productRepository.UpdateProductAsync(oldProduct);
            return new Response<Product>(null, 204);
        }

        protected ValidationResult ValidateEntity<TV, TE>(TV validation, TE entity) where TV
                : AbstractValidator<TE> where TE : class => validation.Validate(entity);
    }
}
