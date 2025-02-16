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
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands;
using GreenEnergyHub.Charges.Domain.MarketParticipants;

namespace GreenEnergyHub.Charges.Domain.Charges
{
    public class ChargeFactory : IChargeFactory
    {
        private readonly IMarketParticipantRepository _marketParticipantRepository;

        public ChargeFactory(IMarketParticipantRepository marketParticipantRepository)
        {
            _marketParticipantRepository = marketParticipantRepository;
        }

        public async Task<Charge> CreateFromChargeOperationDtoAsync(ChargeOperationDto chargeOperationDto)
        {
            var owner = await _marketParticipantRepository
                .SingleOrNullAsync(chargeOperationDto.ChargeOwner)
                .ConfigureAwait(false);

            if (owner == null)
                throw new InvalidOperationException($"Market participant '{chargeOperationDto.ChargeOwner}' does not exist.");

            return Charge.Create(
                chargeOperationDto.Id,
                chargeOperationDto.ChargeName,
                chargeOperationDto.ChargeDescription,
                chargeOperationDto.ChargeId,
                owner.Id,
                chargeOperationDto.Type,
                chargeOperationDto.Resolution,
                chargeOperationDto.TaxIndicator,
                chargeOperationDto.VatClassification,
                chargeOperationDto.TransparentInvoicing == TransparentInvoicing.Transparent,
                chargeOperationDto.StartDateTime,
                chargeOperationDto.EndDateTime);
        }
    }
}
