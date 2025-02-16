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
using System.Collections.Generic;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands;
using GreenEnergyHub.Charges.Domain.Dtos.SharedDtos;
using GreenEnergyHub.Charges.Tests.Builders.Command;
using GreenEnergyHub.TestHelpers;
using Xunit;
using Xunit.Categories;

namespace GreenEnergyHub.Charges.Tests.Domain.Dtos.ChargeCommands
{
    [UnitTest]
    public class ChargeCommandNullCheckerTests
    {
        [Theory]
        [InlineAutoDomainData(BusinessReasonCode.UpdateChargeInformation, null)]
        [InlineAutoDomainData(BusinessReasonCode.UpdateChargeInformation, "")]
        [InlineAutoDomainData(BusinessReasonCode.UpdateChargePrices, null)]
        [InlineAutoDomainData(BusinessReasonCode.UpdateChargePrices, "")]
        public void ChargeCommandPropertiesAreNotNullOrWhitespace(
            BusinessReasonCode businessReasonCode,
            string documentId)
        {
            // Arrange
            var chargeOperationDto = new ChargeOperationDtoBuilder()
                .Build();
            var documentDto = new DocumentDtoBuilder()
                .WithDocumentId(documentId)
                .WithBusinessReasonCode(businessReasonCode)
                .Build();
            var chargeCommand = new ChargeCommandBuilder()
                .WithDocumentDto(documentDto)
                .WithChargeOperation(chargeOperationDto)
                .Build();

            var chargeCommands = new List<ChargeCommand> { chargeCommand };

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                ChargeCommandNullChecker.ThrowExceptionIfRequiredPropertyIsNull(chargeCommands));
        }

        [Theory]
        [InlineAutoDomainData]
        public void ThrowExceptionIfRequiredPropertyIsNull_WhenValid_DoesNotThrow(ChargeCommandBuilder chargeCommandBuilder)
        {
            // Arrange
            var chargeCommand = chargeCommandBuilder.Build();
            var chargeCommands = new List<ChargeCommand> { chargeCommand };

            // Act
            var ex = Record.Exception(() =>
                ChargeCommandNullChecker.ThrowExceptionIfRequiredPropertyIsNull(chargeCommands));

            // Assert
            Assert.Null(ex);
        }

        [Fact]
        public void ThrowExceptionIfRequiredPropertyIsNull_WhenCommandIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            ChargeCommand? chargeCommand = null;
            var chargeCommands = new List<ChargeCommand> { chargeCommand! };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                ChargeCommandNullChecker.ThrowExceptionIfRequiredPropertyIsNull(chargeCommands));
        }

        [Theory]
        [InlineAutoDomainData]
        public void ThrowExceptionIfRequiredPropertyIsNull_WhenParticipantIsNull_ThrowsArgumentNullException(
            ChargeCommandBuilder builder)
        {
            // Arrange
            MarketParticipantDto? marketParticipant = null;
            var documentDto = new DocumentDtoBuilder()
                .WithSender(marketParticipant!)
                .Build();
            var chargeCommand = builder.WithDocumentDto(documentDto).Build();
            var chargeCommands = new List<ChargeCommand> { chargeCommand };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                ChargeCommandNullChecker.ThrowExceptionIfRequiredPropertyIsNull(chargeCommands));
        }

        [Theory]
        [InlineAutoDomainData]
        public void ChargeCommandDocumentIsNullThrowsException(ChargeCommandBuilder builder)
        {
            // Arrange
            var chargeCommand = builder.WithDocumentDto(null!).Build();
            var chargeCommands = new List<ChargeCommand> { chargeCommand };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                ChargeCommandNullChecker.ThrowExceptionIfRequiredPropertyIsNull(chargeCommands));
        }

        [Fact]
        public void ChargeCommandChargeOperationIsNullThrowsException()
        {
            // Arrange
            var chargeCommand = new ChargeCommandBuilder().WithChargeOperation(null!).Build();
            var chargeCommands = new List<ChargeCommand> { chargeCommand };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                ChargeCommandNullChecker.ThrowExceptionIfRequiredPropertyIsNull(chargeCommands));
        }
    }
}
