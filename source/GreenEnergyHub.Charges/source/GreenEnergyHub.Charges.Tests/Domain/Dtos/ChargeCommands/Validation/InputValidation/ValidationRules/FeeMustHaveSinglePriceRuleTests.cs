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

using System.Linq;
using FluentAssertions;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands.Validation;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands.Validation.InputValidation.ValidationRules;
using GreenEnergyHub.Charges.TestCore.Attributes;
using GreenEnergyHub.Charges.Tests.Builders;
using GreenEnergyHub.TestHelpers;
using Xunit;
using Xunit.Categories;
using ChargeType = GreenEnergyHub.Charges.Domain.Charges.ChargeType;

namespace GreenEnergyHub.Charges.Tests.Domain.Dtos.ChargeCommands.Validation.InputValidation.ValidationRules
{
    [UnitTest]
    public class FeeMustHaveSinglePriceRuleTests
    {
        [Theory]
        [InlineAutoMoqData(0, false)]
        [InlineAutoMoqData(1, true)]
        [InlineAutoMoqData(2, false)]
        public void IsValid_WhenCalledWith1PricePoint_ShouldParseValidation(
            int priceCount,
            bool expected,
            ChargeCommandBuilder chargeCommandBuilder)
        {
            // Arrange
            var command = CreateCommand(chargeCommandBuilder, ChargeType.Fee, priceCount);

            // Act
            var sut = new FeeMustHaveSinglePriceRule(command);

            // Assert
            sut.IsValid.Should().Be(expected);
        }

        [Theory]
        [InlineAutoMoqData(ChargeType.Tariff)]
        [InlineAutoMoqData(ChargeType.Unknown)]
        public void IsValid_WhenNeitherFeeOrSubscription_ShouldParseValidation(
            ChargeType chargeType,
            ChargeCommandBuilder chargeCommandBuilder)
        {
            var chargeCommand = chargeCommandBuilder.WithChargeType(chargeType).Build();
            var sut = new FeeMustHaveSinglePriceRule(chargeCommand);
            sut.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineAutoDomainData]
        public void ValidationRuleIdentifier_ShouldBe_EqualTo(ChargeCommandBuilder chargeCommandBuilder)
        {
            var chargeCommand = CreateCommand(chargeCommandBuilder, ChargeType.Fee, 0);
            var sut = new FeeMustHaveSinglePriceRule(chargeCommand);
            sut.ValidationRuleIdentifier.Should().Be(ValidationRuleIdentifier.FeeMustHaveSinglePrice);
        }

        private static ChargeCommand CreateCommand(ChargeCommandBuilder builder, ChargeType chargeType, int priceCount)
        {
            return builder.WithChargeType(chargeType).WithPointWithXNumberOfPrices(priceCount).Build();
        }
    }
}
