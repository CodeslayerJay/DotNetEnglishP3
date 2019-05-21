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
    // All validation for product price goes here.
    public class ProductPriceValidation : AbstractValidator<ProductViewModel>
    {
        public ProductPriceValidation(IStringLocalizer<ProductService> localizer)
        {
            
            RuleFor(x => x.Price)
                // Set cascade mode on
                .Cascade(CascadeMode.StopOnFirstFailure)
                // Check price is not empty/null
                .NotEmpty().WithMessage(x => localizer["MissingPrice"])
                // Check that the value entered is a number
                .Must(Price => double.TryParse(Price.ToString(), out double result)).WithMessage(x => localizer["PriceNotANumber"])
                // Check that the value entered is greater than 0
                .Must(Price => (Double.Parse(Price.ToString()) > 0)).WithMessage(x => localizer["PriceNotGreaterThanZero"]);
        }

    }
}
