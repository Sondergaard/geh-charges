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
using System.Threading.Tasks;
using Energinet.Charges.Contracts.ChargeLink;

namespace Energinet.DataHub.Charges.Clients.ChargeLinks
{
    /// <summary>
    /// Charge Links Client Interface
    /// </summary>
    public interface IChargeLinksClient
    {
        /// <summary>
        /// Gets all charge links data for a given metering point.
        /// </summary>
        /// <param name="meteringPointId">The 18-digits metering point identifier used by the Danish version of Green Energy Hub.</param>
        /// <returns>A collection of Charge Link DTOs</returns>
        public Task<IList<ChargeLinkV1Dto>> GetAsync(string meteringPointId);
    }
}
