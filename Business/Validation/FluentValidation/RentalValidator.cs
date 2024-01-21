﻿using Entities.Concrete;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Validation.FluentValidation
{
    public class RentalValidator:AbstractValidator<Rental>
    {
        public RentalValidator() { 
            RuleFor(r => r.RentDate).NotEmpty().NotNull();
            RuleFor(r => r.ReturnDate).NotEmpty().NotNull();
        }
    }
}
