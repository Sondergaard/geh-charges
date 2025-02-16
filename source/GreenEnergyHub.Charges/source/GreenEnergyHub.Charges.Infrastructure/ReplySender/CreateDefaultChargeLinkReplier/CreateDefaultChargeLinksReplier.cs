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
using Energinet.Charges.Contracts;
using Energinet.DataHub.Core.App.FunctionApp.Middleware.CorrelationId;
using Google.Protobuf;
using GreenEnergyHub.Charges.Application.ChargeLinks.CreateDefaultChargeLinkReplier;
using GreenEnergyHub.Charges.Contracts;

namespace GreenEnergyHub.Charges.Infrastructure.ReplySender.CreateDefaultChargeLinkReplier
{
    public sealed class CreateDefaultChargeLinksReplier : ICreateDefaultChargeLinksReplier
    {
        private readonly ICorrelationContext _correlationContext;
        private readonly IServiceBusReplySenderProvider _serviceBusReplySenderProvider;

        public CreateDefaultChargeLinksReplier(
            ICorrelationContext correlationContext,
            IServiceBusReplySenderProvider serviceBusReplySenderProvider)
        {
            _correlationContext = correlationContext;
            _serviceBusReplySenderProvider = serviceBusReplySenderProvider;
        }

        public Task ReplyWithSucceededAsync(
            string meteringPointId,
            bool didCreateChargeLinks,
            string replyTo)
        {
            ValidateParametersOrThrow(meteringPointId, replyTo, _correlationContext.Id);

            var createDefaultChargeLinksReplySucceeded = new CreateDefaultChargeLinksReply
            {
                MeteringPointId = meteringPointId,
                CreateDefaultChargeLinksSucceeded =
                    new CreateDefaultChargeLinksReply.Types.CreateDefaultChargeLinksSucceeded
                {
                    DidCreateChargeLinks = didCreateChargeLinks,
                },
            };

            return SendReplyAsync(createDefaultChargeLinksReplySucceeded, replyTo, _correlationContext.Id);
        }

        public Task ReplyWithFailedAsync(
            string meteringPointId,
            ErrorCode errorCode,
            string replyTo)
        {
            ValidateParametersOrThrow(meteringPointId, replyTo, _correlationContext.Id);

            var createDefaultChargeLinksReplyFailed = new CreateDefaultChargeLinksReply
            {
                MeteringPointId = meteringPointId,
                CreateDefaultChargeLinksFailed =
                    new CreateDefaultChargeLinksReply.Types.CreateDefaultChargeLinksFailed
                    {
                        ErrorCode =
                            (CreateDefaultChargeLinksReply.Types.CreateDefaultChargeLinksFailed.Types.ErrorCode)errorCode,
                    },
            };

            return SendReplyAsync(createDefaultChargeLinksReplyFailed, replyTo, _correlationContext.Id);
        }

        private Task SendReplyAsync(
            CreateDefaultChargeLinksReply createDefaultChargeLinks,
            string replyTo,
            string correlationId)
        {
            var sender = _serviceBusReplySenderProvider.GetInstance(replyTo);

            return sender.SendReplyAsync(createDefaultChargeLinks.ToByteArray(), correlationId);
        }

        private static void ValidateParametersOrThrow(string meteringPointId, string replyTo, string correlationId)
        {
            if (meteringPointId == null)
                throw new ArgumentNullException(nameof(meteringPointId));

            if (replyTo == null)
                throw new ArgumentNullException(nameof(replyTo));

            if (correlationId == null)
                throw new ArgumentNullException(nameof(correlationId));
        }
    }
}
