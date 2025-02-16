﻿// Copyright 2020 Energinet DataHub A/S
//
// Licensed under the Apache License, Version 2.0 (the "License2");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using FluentAssertions;
using GreenEnergyHub.Charges.Domain.Charges;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands.Validation.InputValidation.ValidationRules;
using GreenEnergyHub.Charges.TestCore.Attributes;
using GreenEnergyHub.Charges.Tests.Builders.Command;
using Xunit;
using Xunit.Categories;

namespace GreenEnergyHub.Charges.Tests.Domain.Dtos.ChargeCommands.Validation.InputValidation.ValidationRules
{
    [UnitTest]
    public class TaxIndicatorIsRequiredValidationRuleTests
    {
        [Theory]
        [InlineAutoMoqData(TaxIndicator.Tax, true)]
        [InlineAutoMoqData(TaxIndicator.NoTax, true)]
        [InlineAutoMoqData(TaxIndicator.Unknown, false)]
        public void IsValid_WhenCalled_ShouldReturnExpectedValue(
            TaxIndicator taxIndicator,
            bool expected,
            ChargeOperationDtoBuilder chargeOperationDtoBuilder)
        {
            var chargeOperationDto = chargeOperationDtoBuilder.WithTaxIndicator(taxIndicator).Build();
            var sut = new TaxIndicatorIsRequiredValidationRule(chargeOperationDto);
            sut.IsValid.Should().Be(expected);
        }
    }
}
