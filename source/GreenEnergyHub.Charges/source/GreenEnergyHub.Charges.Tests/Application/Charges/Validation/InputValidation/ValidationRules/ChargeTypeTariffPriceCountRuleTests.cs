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

using System;
using System.Linq;
using FluentAssertions;
using GreenEnergyHub.Charges.Domain.Charges;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands.Validation;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands.Validation.InputValidation.ValidationRules;
using GreenEnergyHub.Charges.TestCore.Attributes;
using GreenEnergyHub.Charges.Tests.Builders;
using GreenEnergyHub.TestHelpers;
using Xunit;
using Xunit.Categories;
using ChargeType = GreenEnergyHub.Charges.Domain.Charges.ChargeType;

namespace GreenEnergyHub.Charges.Tests.Application.Charges.Validation.InputValidation.ValidationRules
{
    [UnitTest]
    public class ChargeTypeTariffPriceCountRuleTests
    {
        [Theory]
        [InlineAutoMoqData(1, false)]
        [InlineAutoMoqData(23, false)]
        [InlineAutoMoqData(24, true)]
        [InlineAutoMoqData(25, false)]
        [InlineAutoMoqData(96, false)]
        public void IsValid_WhenPT1HAnd24PricePoints_IsTrue(
            int priceCount,
            bool expected,
            ChargeCommandTestBuilder builder)
        {
            // Arrange
            var chargeCommand = builder.WithChargeType(ChargeType.Tariff).WithPointWithXNumberOfPrices(priceCount).Build();

            // Act
            var sut = new ChargeTypeTariffPriceCountRule(chargeCommand);

            // Assert
            sut.IsValid.Should().Be(expected);
        }

        [Theory]
        [InlineAutoMoqData(0, false)]
        [InlineAutoMoqData(1, true)]
        [InlineAutoMoqData(2, false)]
        [InlineAutoMoqData(24, false)]
        [InlineAutoMoqData(96, false)]
        public void IsValid_WhenP1DAnd1PricePoint_IsTrue(
            int priceCount,
            bool expected,
            ChargeCommandTestBuilder builder)
        {
            // Arrange
            var chargeCommand = builder
                .WithChargeType(ChargeType.Tariff)
                .WithResolution(Resolution.P1D)
                .WithPointWithXNumberOfPrices(priceCount).Build();

            // Act
            var sut = new ChargeTypeTariffPriceCountRule(chargeCommand);

            // Assert
            sut.IsValid.Should().Be(expected);
        }

        [Theory]
        [InlineAutoMoqData(0, false)]
        [InlineAutoMoqData(1, false)]
        [InlineAutoMoqData(2, false)]
        [InlineAutoMoqData(24, false)]
        [InlineAutoMoqData(95, false)]
        [InlineAutoMoqData(96, true)]
        [InlineAutoMoqData(97, false)]
        public void IsValid_WhenPT1HAnd96PricePoints_IsTrue(
            int priceCount,
            bool expected,
            ChargeCommandTestBuilder builder)
        {
            // Arrange
            var chargeCommand = builder
                .WithChargeType(ChargeType.Tariff)
                .WithResolution(Resolution.PT15M)
                .WithPointWithXNumberOfPrices(priceCount).Build();

            // Act
            var sut = new ChargeTypeTariffPriceCountRule(chargeCommand);

            // Assert
            sut.IsValid.Should().Be(expected);
        }

        [Theory]
        [InlineAutoMoqData(ChargeType.Fee)]
        [InlineAutoMoqData(ChargeType.Subscription)]
        [InlineAutoMoqData(ChargeType.Unknown)]
        public void IsValid_WhenNotTariff_IsValid(
            ChargeType chargeType,
            ChargeCommandTestBuilder builder)
        {
            var chargeCommand = builder.WithChargeType(chargeType).Build();
            var sut = new ChargeTypeTariffPriceCountRule(chargeCommand);
            sut.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineAutoMoqData(Resolution.Unknown)]
        [InlineAutoMoqData(Resolution.P1M)]
        public void IsValid_WhenTariffAndUnknownResolutionType_Throws(
            Resolution resolution,
            ChargeCommandTestBuilder builder)
        {
            // Arrange
            var chargeCommand = builder.WithChargeType(ChargeType.Tariff).WithResolution(resolution).Build();
            var chargeTypeTariffPriceCountRule = new ChargeTypeTariffPriceCountRule(chargeCommand);

            // Act
            Action act = () => chargeTypeTariffPriceCountRule.IsValid.Should().BeTrue();

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineAutoDomainData]
        public void ValidationRuleIdentifier_ShouldBe_EqualTo(ChargeCommand command)
        {
            var sut = new ChargeTypeTariffPriceCountRule(command);
            sut.ValidationError.ValidationRuleIdentifier.Should()
                .Be(ValidationRuleIdentifier.ChargeTypeTariffPriceCount);
        }

        [Theory]
        [InlineAutoDomainData]
        public void ValidationRuleIdentifier_ShouldContain_RequiredErrorMessageParameterTypes(ChargeCommand command)
        {
            // Arrange
            // Act
            var sut = new ChargeTypeTariffPriceCountRule(command);

            // Assert
            sut.ValidationError.ValidationErrorMessageParameters
                .Select(x => x.ParameterType)
                .Should().Contain(ValidationErrorMessageParameterType.MaxOfPosition);
            sut.ValidationError.ValidationErrorMessageParameters
                .Select(x => x.ParameterType)
                .Should().Contain(ValidationErrorMessageParameterType.PartyChargeTypeId);
            sut.ValidationError.ValidationErrorMessageParameters
                .Select(x => x.ParameterType)
                .Should().Contain(ValidationErrorMessageParameterType.ResolutionDuration);
        }

        [Theory]
        [InlineAutoDomainData]
        public void ValidationRuleIdentifier_ShouldBe_RequiredErrorMessageParameters(ChargeCommand command)
        {
            // Arrange
            // Act
            var sut = new ChargeTypeTariffPriceCountRule(command);

            // Assert
            sut.ValidationError.ValidationErrorMessageParameters
                .Single(x => x.ParameterType == ValidationErrorMessageParameterType.MaxOfPosition)
                .MessageParameter.Should().Be(command.ChargeOperation.Points.Count.ToString());
            sut.ValidationError.ValidationErrorMessageParameters
                .Single(x => x.ParameterType == ValidationErrorMessageParameterType.PartyChargeTypeId)
                .MessageParameter.Should().Be(command.ChargeOperation.ChargeId);
            sut.ValidationError.ValidationErrorMessageParameters
                .Single(x => x.ParameterType == ValidationErrorMessageParameterType.ResolutionDuration)
                .MessageParameter.Should().Be(command.ChargeOperation.Resolution.ToString());
        }
    }
}
