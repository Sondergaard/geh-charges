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
using Energinet.Charges.Contracts;
using GreenEnergyHub.Charges.Application.ChargeLinks.Handlers;
using GreenEnergyHub.Charges.Domain.Dtos.CreateDefaultChargeLinksRequests;
using GreenEnergyHub.Charges.FunctionHost.Common;
using GreenEnergyHub.Charges.Infrastructure.Core.MessagingExtensions;
using Microsoft.Azure.Functions.Worker;

namespace GreenEnergyHub.Charges.FunctionHost.ChargeLinks
{
    public class CreateChargeLinkReceiverEndpoint
    {
        /// <summary>
        /// The name of the function.
        /// Function name affects the URL and thus possibly dependent infrastructure.
        /// </summary>
        public const string FunctionName = nameof(CreateChargeLinkReceiverEndpoint);
        private readonly MessageExtractor<CreateDefaultChargeLinks> _messageExtractor;
        private readonly ICreateLinkRequestHandler _createLinkRequestHandler;

        public CreateChargeLinkReceiverEndpoint(
            MessageExtractor<CreateDefaultChargeLinks> messageExtractor,
            ICreateLinkRequestHandler createLinkRequestHandler)
        {
            _messageExtractor = messageExtractor;
            _createLinkRequestHandler = createLinkRequestHandler;
        }

        [Function(FunctionName)]
        public async Task RunAsync(
            [ServiceBusTrigger(
                "%" + EnvironmentSettingNames.CreateLinksRequestQueueName + "%",
                Connection = EnvironmentSettingNames.DataHubListenerConnectionString)]
            byte[] message)
        {
            var createLinkCommandEvent =
                (CreateDefaultChargeLinksRequest)await _messageExtractor.ExtractAsync(message).ConfigureAwait(false);
            await _createLinkRequestHandler.HandleAsync(createLinkCommandEvent).ConfigureAwait(false);
        }
    }
}
