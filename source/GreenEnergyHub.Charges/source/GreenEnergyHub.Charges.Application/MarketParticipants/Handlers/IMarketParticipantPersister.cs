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

using System.Threading.Tasks;
using GreenEnergyHub.Charges.Domain.Dtos.MarketParticipantsUpdatedEvents;

namespace GreenEnergyHub.Charges.Application.MarketParticipants.Handlers
{
    /// <summary>
    /// Handles market participant updates
    /// </summary>
    public interface IMarketParticipantPersister
    {
        /// <summary>
        /// Adds or update a market participant from an integration event
        /// </summary>
        /// <param name="marketParticipantUpdatedEvent"></param>
        Task PersistAsync(MarketParticipantUpdatedEvent marketParticipantUpdatedEvent);
    }
}
