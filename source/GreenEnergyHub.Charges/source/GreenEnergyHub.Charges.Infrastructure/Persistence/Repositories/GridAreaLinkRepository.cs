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
using GreenEnergyHub.Charges.Domain.GridAreaLinks;
using Microsoft.EntityFrameworkCore;

namespace GreenEnergyHub.Charges.Infrastructure.Persistence.Repositories
{
    public class GridAreaLinkRepository : IGridAreaLinkRepository
    {
        private readonly IChargesDatabaseContext _chargesDatabaseContext;

        public GridAreaLinkRepository(IChargesDatabaseContext chargesDatabaseContext)
        {
            _chargesDatabaseContext = chargesDatabaseContext;
        }

        public async Task AddAsync(GridAreaLink gridAreaLink)
        {
            ArgumentNullException.ThrowIfNull(gridAreaLink);
            await _chargesDatabaseContext.GridAreaLinks.AddAsync(gridAreaLink).ConfigureAwait(false);
        }

        public async Task<GridAreaLink?> GetOrNullAsync(Guid gridAreaLinkId)
        {
            return await _chargesDatabaseContext
                .GridAreaLinks
                .SingleOrDefaultAsync(gal => gal.Id == gridAreaLinkId).ConfigureAwait(false);
        }

        public async Task<GridAreaLink?> GetGridAreaOrNullAsync(Guid gridAreaId)
        {
            return await _chargesDatabaseContext
                .GridAreaLinks
                .SingleOrDefaultAsync(gal => gal.GridAreaId == gridAreaId).ConfigureAwait(false);
        }
    }
}
