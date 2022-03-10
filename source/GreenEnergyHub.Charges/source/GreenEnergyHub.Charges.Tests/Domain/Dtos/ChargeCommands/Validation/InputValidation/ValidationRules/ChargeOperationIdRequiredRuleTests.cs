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
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands.Validation.InputValidation.ValidationRules;
using GreenEnergyHub.Charges.Domain.Dtos.Validation;
using GreenEnergyHub.Charges.TestCore.Attributes;
using GreenEnergyHub.Charges.Tests.Builders.Command;
using GreenEnergyHub.TestHelpers;
using Xunit;
using Xunit.Categories;

namespace GreenEnergyHub.Charges.Tests.Domain.Dtos.ChargeCommands.Validation.InputValidation.ValidationRules
{
    [UnitTest]
    public class ChargeOperationIdRequiredRuleTests
    {
        [Theory]
        [InlineAutoMoqData("ChargeOperationId", true)]
        [InlineAutoMoqData("", false)]
        [InlineAutoMoqData(" ", false)]
        [InlineAutoMoqData(null!, false)]
        public void ChargeOperationIdRequiredRule_Test(
            string chargeOperationId,
            bool expected,
            ChargeCommandBuilder builder)
        {
            var chargeCommand = builder.WithOperationId(chargeOperationId).Build();
            var chargeOperationDto = chargeCommand.Charges.First();
            var sut = new ChargeOperationIdRequiredRule(chargeOperationDto);
            Assert.Equal(expected, sut.IsValid);
        }

        [Theory]
        [InlineAutoDomainData]
        public void ValidationRuleIdentifier_ShouldBe_EqualTo(ChargeCommandBuilder builder)
        {
            var invalidCommand = CreateInvalidCommand(builder);
            var chargeOperationDto = invalidCommand.Charges.First();
            var sut = new ChargeOperationIdRequiredRule(chargeOperationDto);
            sut.ValidationRuleIdentifier.Should()
                .Be(ValidationRuleIdentifier.ChargeOperationIdRequired);
        }

        private static ChargeCommand CreateInvalidCommand(ChargeCommandBuilder builder)
        {
            return builder.WithOperationId(string.Empty).Build();
        }
    }
}
