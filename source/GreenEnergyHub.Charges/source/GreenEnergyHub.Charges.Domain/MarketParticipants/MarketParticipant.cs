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
using GreenEnergyHub.Charges.Domain.Dtos.SharedDtos;

namespace GreenEnergyHub.Charges.Domain.MarketParticipants
{
    /// <summary>
    /// A market participant, e.g. a Grid Access Provider, whom may submit a charge message.
    /// </summary>
    public class MarketParticipant
    {
        public static readonly IReadOnlyCollection<MarketParticipantRole> _validRoles = new List<MarketParticipantRole>
        {
            MarketParticipantRole.EnergySupplier,
            MarketParticipantRole.SystemOperator,
            MarketParticipantRole.GridAccessProvider,
            MarketParticipantRole.MeteringPointAdministrator,
        }.AsReadOnly();

        public MarketParticipant(
            Guid id,
            Guid actorId,
            Guid? b2CActorId,
            string marketParticipantId,
            bool isActive,
            MarketParticipantRole businessProcessRole)
        {
            Id = id;
            ActorId = actorId;
            B2CActorId = b2CActorId;
            MarketParticipantId = marketParticipantId;
            IsActive = isActive;
            UpdateBusinessProcessRole(businessProcessRole);
        }

        // ReSharper disable once UnusedMember.Local - Required by persistence framework
        private MarketParticipant()
        {
            MarketParticipantId = null!;
        }

        public Guid Id { get; }

        /// <summary>
        /// ID identifying the actor in the Market Participant domain
        /// The setter is public as the charges domain doesn't enforce any validation
        /// as it is the responsibility of the market participant domain providing the data.
        /// </summary>
        public Guid ActorId { get; set; }

        /// <summary>
        /// ID used for authentication of B2B requests.
        /// The setter is public as the charges domain doesn't enforce any validation
        /// as it is the responsibility of the market participant domain providing the data.
        /// </summary>
        public Guid? B2CActorId { get; set; }

        /// <summary>
        /// The ID that identifies the market participant. In Denmark this would be the GLN number or EIC code.
        /// This ID must be immutable. A new market participant id would require de-activating the market participant
        /// and replacing it by a new market participant.
        /// </summary>
        public string MarketParticipantId { get; }

        /// <summary>
        /// The roles of the market participant.
        /// </summary>
        public MarketParticipantRole BusinessProcessRole { get; private set; }

        public void UpdateBusinessProcessRole(MarketParticipantRole role)
        {
            if (!_validRoles.Contains(role))
                throw new ArgumentException($"Business process role '{role}' is not valid in the charges domain.");

            BusinessProcessRole = role;
        }

        /// <summary>
        /// Market participants will not be deleted. They will be made in-active.
        /// The setter is public as the charges domain doesn't enforce any validation
        /// as it is the responsibility of the market participant domain providing the data.
        /// </summary>
        public bool IsActive { get; set; }
    }
}
