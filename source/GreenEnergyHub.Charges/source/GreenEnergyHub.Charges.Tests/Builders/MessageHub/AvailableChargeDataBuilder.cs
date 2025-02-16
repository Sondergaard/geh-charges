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
using GreenEnergyHub.Charges.Domain.Charges;
using GreenEnergyHub.Charges.Domain.Dtos.SharedDtos;
using GreenEnergyHub.Charges.MessageHub.Models.AvailableChargeData;
using NodaTime;

namespace GreenEnergyHub.Charges.Tests.Builders.MessageHub
{
    public class AvailableChargeDataBuilder
    {
        private BusinessReasonCode _businessReasonCode;
        private Guid _availableDataReferenceId;
        private int _operationOrder = 0;
        private Instant _requestDateTime = SystemClock.Instance.GetCurrentInstant();
        private Guid _actorId;

        public AvailableChargeDataBuilder()
        {
            _businessReasonCode = BusinessReasonCode.UpdateChargeInformation;
            _availableDataReferenceId = Guid.NewGuid();
            _actorId = Guid.NewGuid();
        }

        public AvailableChargeDataBuilder WithBusinessReasonCode(BusinessReasonCode businessReasonCode)
        {
            _businessReasonCode = businessReasonCode;
            return this;
        }

        public AvailableChargeDataBuilder WithAvailableDataReferenceId(Guid referenceId)
        {
            _availableDataReferenceId = referenceId;
            return this;
        }

        public AvailableChargeDataBuilder WithActorId(Guid actorId)
        {
            _actorId = actorId;
            return this;
        }

        public AvailableChargeDataBuilder WithOperationOrder(int operationOrder)
        {
            _operationOrder = operationOrder;
            return this;
        }

        public AvailableChargeDataBuilder WithRequestDateTime(Instant requestDateTime)
        {
            _requestDateTime = requestDateTime;
            return this;
        }

        public AvailableChargeData Build()
        {
            return new AvailableChargeData(
                "senderId",
                MarketParticipantRole.MeteringPointAdministrator,
                "recipientId",
                MarketParticipantRole.GridAccessProvider,
                _businessReasonCode,
                _requestDateTime,
                _availableDataReferenceId,
                "chargeId",
                "chargeOwner",
                ChargeType.Fee,
                "chargeName",
                "chargeDescription",
                SystemClock.Instance.GetCurrentInstant(),
                SystemClock.Instance.GetCurrentInstant(),
                VatClassification.Vat25,
                true,
                true,
                Resolution.PT15M,
                DocumentType.NotifyPriceList,
                _operationOrder,
                _actorId,
                new List<AvailableChargeDataPoint>());
        }
    }
}
