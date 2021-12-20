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
using GreenEnergyHub.Charges.Domain.Dtos.ChargeLinksAcceptedEvents;
using GreenEnergyHub.Charges.Domain.Dtos.DefaultChargeLinksDataAvailableNotifiedEvents;
using GreenEnergyHub.Charges.Infrastructure.Core.MessageMetaData;
using GreenEnergyHub.Charges.Infrastructure.Core.MessagingExtensions;

namespace GreenEnergyHub.Charges.Application.ChargeLinks.Handlers
{
    public class DefaultChargeLinksCreatedReplier : IDefaultChargeLinksCreatedReplier
    {
        private readonly IMessageDispatcher<DefaultChargeLinksCreatedEvent> _messageDispatcher;
        private readonly IDefaultChargeLinksCreatedEventFactory _defaultChargeLinksCreatedEventFactory;
        private readonly IMessageMetaDataContext _messageMetaDataContext;

        public DefaultChargeLinksCreatedReplier(
            IMessageDispatcher<DefaultChargeLinksCreatedEvent> messageDispatcher,
            IDefaultChargeLinksCreatedEventFactory defaultChargeLinksCreatedEventFactory,
            IMessageMetaDataContext messageMetaDataContext)
        {
            _messageDispatcher = messageDispatcher;
            _defaultChargeLinksCreatedEventFactory = defaultChargeLinksCreatedEventFactory;
            _messageMetaDataContext = messageMetaDataContext;
        }

        public async Task ReplyAsync(ChargeLinksAcceptedEvent chargeLinksAcceptedEvent)
        {
            if (chargeLinksAcceptedEvent == null) throw new ArgumentNullException(nameof(chargeLinksAcceptedEvent));

            if (_messageMetaDataContext.IsReplyToSet())
            {
                var defaultChargeLinksCreatedEvent =
                    _defaultChargeLinksCreatedEventFactory.Create(chargeLinksAcceptedEvent);
                await _messageDispatcher.DispatchAsync(defaultChargeLinksCreatedEvent);
            }
        }
    }
}
