using FluentValidation;
using Microsoft.Extensions.Localization;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;

namespace P3AddNewFunctionalityDotNetCore.Models.Validators
{
    public class ProductValidator : AbstractValidator<ProductViewModel>
    {

        public ProductValidator(IStringLocalizer<ProductService> localizer)
        {
            
            Include(new ProductNameValidation(localizer));
            Include(new ProductPriceValidation(localizer));
            Include(new ProductStockValidation(localizer));

        }
        
    }
}