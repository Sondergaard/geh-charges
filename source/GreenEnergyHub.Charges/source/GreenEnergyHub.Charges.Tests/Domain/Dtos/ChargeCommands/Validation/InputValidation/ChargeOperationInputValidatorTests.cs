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

using System.Collections.Generic;
using AutoFixture.Xunit2;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands;
using GreenEnergyHub.Charges.Domain.Dtos.Validation;
using Moq;
using Xunit;
using Xunit.Categories;

namespace GreenEnergyHub.Charges.Tests.Domain.Dtos.ChargeCommands.Validation.InputValidation
{
    [UnitTest]
    public class ChargeOperationInputValidatorTests
    {
        [Theory]
        [InlineAutoData]
        public void Validate_WhenValidatingChargeCommand_ReturnsChargeCommandValidationResult(
            Mock<IInputValidationRulesFactory<ChargeOperationDto>> chargeOperationInputValidationRulesFactory,
            ChargeOperationDto chargeOperationDto)
        {
            // Arrange
            var invalidRule = new OperationValidationRuleContainer(
                new TestValidationRule(false, ValidationRuleIdentifier.StartDateValidation),
                chargeOperationDto.Id);
            var invalidRules = new List<IValidationRuleContainer> { invalidRule };
            var invalidRuleSet = ValidationRuleSet.FromRules(invalidRules);
            chargeOperationInputValidationRulesFactory
                .Setup(c => c.CreateRules(It.IsAny<ChargeOperationDto>()))
                .Returns(invalidRuleSet);
            var sut = new InputValidator<ChargeOperationDto>(chargeOperationInputValidationRulesFactory.Object);

            // Act
            var result = sut.Validate(chargeOperationDto);

            // Assert
            Assert.IsType<ValidationResult>(result);
        }
    }
}
