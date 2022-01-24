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
using System.Linq;
using System.Threading.Tasks;
using GreenEnergyHub.Charges.Domain.MarketParticipants;
using Microsoft.EntityFrameworkCore;

namespace GreenEnergyHub.Charges.Infrastructure.Persistence.Repositories
{
    public class MarketParticipantRepository : IMarketParticipantRepository
    {
        private readonly IChargesDatabaseContext _chargesDatabaseContext;

        public MarketParticipantRepository(
            IChargesDatabaseContext chargesDatabaseContext)
        {
            _chargesDatabaseContext = chargesDatabaseContext;
        }

        public Task<MarketParticipant> GetOrNullAsync(string marketParticipantId)
        {
            return _chargesDatabaseContext
                .MarketParticipants
                .SingleOrDefaultAsync(mp => mp.MarketParticipantId == marketParticipantId);
        }

        public async Task<List<MarketParticipant>> GetGridAccessProvidersAsync()
        {
            return await _chargesDatabaseContext
                .MarketParticipants
                .Where(mp => mp.BusinessProcessRole == MarketParticipantRole.GridAccessProvider)
                .Where(m => m.IsActive)
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<MarketParticipant>> GetAsync(IEnumerable<Guid> ids)
        {
            return await _chargesDatabaseContext
                .MarketParticipants
                .Where(mp => ids.Contains(mp.Id))
                .ToListAsync();
        }

        /// <summary>
        /// This implementation is temp until grid areas and market participants are implemented in their own
        /// domains and integration event are used  to update a query model in the charges domain.
        ///
        /// Later we need to use the metering point ID to find the grid area and then find the responsible market
        /// participant of the grid area.
        /// </summary>
        /// <param name="meteringPointId">ID of the metering point to find the grid access provider for</param>
        /// <returns>The grid access provider responsible for the metering point</returns>
        public Task<MarketParticipant> GetGridAccessProviderAsync(string meteringPointId)
        {
            return Task.FromResult(new MarketParticipant(
                Guid.NewGuid(),
                "8100000000030",
                true,
                MarketParticipantRole.GridAccessProvider));
        }

        public async Task<MarketParticipant> GetMeteringPointAdministratorAsync()
        {
            return await GetAsync(MarketParticipantRole.MeteringPointAdministrator).ConfigureAwait(false);
        }

        public async Task<MarketParticipant> GetSystemOperatorAsync()
        {
            return await GetAsync(MarketParticipantRole.SystemOperator).ConfigureAwait(false);
        }

        private async Task<MarketParticipant> GetAsync(MarketParticipantRole marketParticipantRole)
        {
            return await _chargesDatabaseContext
                .MarketParticipants
                .Where(mp => mp.BusinessProcessRole == marketParticipantRole)
                .Where(mp => mp.IsActive)
                .SingleAsync()
                .ConfigureAwait(false);
        }
    }
}
