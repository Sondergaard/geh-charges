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
using System.Threading.Tasks;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeInformationCommands;
using GreenEnergyHub.Charges.Domain.Dtos.ChargePriceCommands;
using GreenEnergyHub.Charges.Domain.Dtos.SharedDtos;

namespace GreenEnergyHub.Charges.Application.Charges.Handlers
{
    public class ChargeCommandBundleHandler : IChargesBundleHandler
    {
        private readonly IChargeInformationCommandHandler _chargeInformationCommandHandler;
        private readonly IChargePriceCommandHandler _chargePriceCommandHandler;

        public ChargeCommandBundleHandler(
            IChargeInformationCommandHandler chargeInformationCommandHandler,
            IChargePriceCommandHandler chargePriceCommandHandler)
        {
            _chargeInformationCommandHandler = chargeInformationCommandHandler;
            _chargePriceCommandHandler = chargePriceCommandHandler;
        }

        public async Task HandleAsync(ChargeBundleDto bundleDto)
        {
            foreach (var command in bundleDto.Commands)
            {
                switch (command)
                {
                    case ChargePriceCommand priceCommand:
                        await _chargePriceCommandHandler
                            .HandleAsync(priceCommand)
                            .ConfigureAwait(false);
                        break;
                    case ChargeInformationCommand informationCommand:
                        await _chargeInformationCommandHandler
                            .HandleAsync(informationCommand)
                            .ConfigureAwait(false);
                        break;
                    default:
                        throw new ArgumentException(
                            $"Could not handle commands from charge bundle {bundleDto}");
                }
            }
        }
    }
}
