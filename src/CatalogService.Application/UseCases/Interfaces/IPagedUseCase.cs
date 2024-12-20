﻿using CatalogService.Application.Responses;
using FluentValidation;
using FluentValidation.Results;

namespace CatalogService.Application.UseCases.Interfaces
{
    public interface IPagedUseCase<I, O>
    {
        Task<PagedResponse<O>> HandleAsync(I input);
        string[] GetAllErrors(ValidationResult validationResult);
        ValidationResult ValidateEntity<TV, TE>(TV validation, TE entity) where TV
                : AbstractValidator<TE> where TE : class;
    }
}
