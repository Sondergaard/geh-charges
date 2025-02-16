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

namespace GreenEnergyHub.Charges.Domain.Dtos.SharedDtos
{
    /// <summary>
    /// IMPORTANT: This is used in transport so the numbers matters.
    /// </summary>
    public enum MarketParticipantRole
    {
        /// <summary>
        /// Should only be used for unvalidated input.
        /// It should never be used or stored for other things than validation and rejections.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Also known as DDQ.
        /// </summary>
        EnergySupplier = 1,

        /// <summary>
        /// Also known as DDM.
        /// </summary>
        GridAccessProvider = 2,

        /// <summary>
        /// Also known as EZ.
        /// In Denmark it's Energinet.
        /// </summary>
        SystemOperator = 3,

        /// <summary>
        /// Not used in the charges domain.
        /// Also known as MDR.
        /// </summary>
        MeteredDataResponsible = 4,

        /// <summary>
        /// Not used in the charges domain.
        /// Also known as STS.
        /// </summary>
        EnergyAgency = 5,

        /// <summary>
        /// Not used in the charges domain.
        /// Also known as DGL.
        /// In Denmark it's DataHub.
        /// </summary>
        MeteredDataAdministrator = 6,

        /// <summary>
        /// Also known as DDZ.
        /// In Denmark it's DataHub.
        /// </summary>
        MeteringPointAdministrator = 7,

        /// <summary>
        /// Not used in the charges domain.
        /// Also known as DDK.
        /// </summary>
        BalanceResponsibleParty = 8,

        /// <summary>
        /// Not used in the charges domain.
        /// Also known as DDX.
        /// </summary>
        ImbalanceSettlementResponsible = 9,

        /// <summary>
        /// Not used in the charges domain.
        /// Also known as DEA.
        /// </summary>
        MeteredDataAggregator = 10,
    }
}
