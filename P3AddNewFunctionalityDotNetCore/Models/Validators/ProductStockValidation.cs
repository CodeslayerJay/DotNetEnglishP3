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
    // All validation for stock goes here.
    public class ProductStockValidation : AbstractValidator<ProductViewModel>
    {

        public ProductStockValidation(IStringLocalizer<ProductService> localizer)
        {
            
            RuleFor(x => x.Stock)
                // Set cascade mode on
                .Cascade(CascadeMode.StopOnFirstFailure)
                //Check for missing stock/ quantity    
                .NotEmpty().WithMessage(x => localizer["MissingStock"])
                // Verify stock/quantity is a number (int)
                .Must(Stock => int.TryParse(Stock, out int result)).WithMessage(x => localizer["StockNotAnInteger"])
                // Verify stock/quantity is greater than 0
                .Must(Stock => (double.Parse(Stock) > 0)).WithMessage(x => localizer["StockNotGreaterThanZero"]);
        }
    }
}
