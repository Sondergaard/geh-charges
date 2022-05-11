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
using System.Threading.Tasks;
using GreenEnergyHub.Charges.Application.ChargeInformation.Acknowledgement;
using GreenEnergyHub.Charges.Application.ChargePrices.Acknowledgement;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommandAcceptedEvents;

namespace GreenEnergyHub.Charges.Application.ChargeCommands.Handlers
{
    public class ChargeIntegrationEventsPublisher : IChargeIntegrationEventsPublisher
    {
        private readonly IChargeInformationPublisher _chargeInformationPublisher;
        private readonly IChargePricesUpdatedPublisher _chargePricesUpdatedPublisher;

        public ChargeIntegrationEventsPublisher(
            IChargeInformationPublisher chargeInformationPublisher,
            IChargePricesUpdatedPublisher chargePricesUpdatedPublisher)
        {
            _chargeInformationPublisher = chargeInformationPublisher;
            _chargePricesUpdatedPublisher = chargePricesUpdatedPublisher;
        }

        public async Task PublishAsync(ChargeCommandAcceptedEvent chargeCommandAcceptedEvent)
        {
            ArgumentNullException.ThrowIfNull(chargeCommandAcceptedEvent);

            foreach (var chargeOperationDto in chargeCommandAcceptedEvent.Command.ChargeOperations)
            {
                await _chargeInformationPublisher.PublishChargeInformationCreatedAsync(chargeOperationDto).ConfigureAwait(false);

                if (chargeOperationDto.Points.Any())
                {
                    await _chargePricesUpdatedPublisher
                        .PublishChargePricesAsync(chargeOperationDto)
                        .ConfigureAwait(false);
                }
            }
        }
    }
}
