﻿using CatalogService.Domain.Entities;

namespace CatalogService.Domain.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductsAsync(int pageNumber, int pageSize);
        Task<Product> GetProductByIdAsync(Guid id);
        Task CreateProductAsync(Product product);
        Task UpdateProductAsync(Product product);
    }
}