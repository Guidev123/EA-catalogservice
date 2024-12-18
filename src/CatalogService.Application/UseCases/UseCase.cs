﻿using CatalogService.Application.Responses;
using CatalogService.Application.UseCases.Interfaces;

namespace CatalogService.Application.UseCases
{
    public abstract class UseCase<I, O> : BaseUseCase, IUseCase<I, O>
    {
        public virtual Task<Response<O>> HandleAsync(I input) =>
            throw new NotImplementedException();
    }
}
