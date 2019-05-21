using FluentValidation;
using Microsoft.Extensions.Localization;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P3AddNewFunctionalityDotNetCore.Models.Validators
{
    // All validation for the product name goes here
    public class ProductNameValidation : AbstractValidator<ProductViewModel>
    {
        public ProductNameValidation(IStringLocalizer<ProductService> localizer)
        {
            // Check for missing name
            RuleFor(x => x.Name).NotEmpty().WithMessage(x => localizer["MissingName"]);
        }
    }
}
