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
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Energinet.Charges.Contracts;
using Energinet.DataHub.Core.App.Common.Abstractions.ServiceBus;
using Energinet.DataHub.Core.FunctionApp.TestCommon;
using FluentAssertions;
using Google.Protobuf;
using GreenEnergyHub.Charges.Domain.Dtos.SharedDtos;
using GreenEnergyHub.Charges.IntegrationTest.Core.Fixtures.FunctionApp;
using GreenEnergyHub.Charges.IntegrationTest.Core.TestHelpers;
using GreenEnergyHub.Charges.IntegrationTests.Fixtures;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;
using Actor = Energinet.DataHub.Core.App.Common.Abstractions.Actor.Actor;

namespace GreenEnergyHub.Charges.IntegrationTests.DomainTests
{
    [IntegrationTest]
    public class CreateDefaultChargeLinksReceiverTests
    {
        [Collection(nameof(ChargesFunctionAppCollectionFixture))]
        public class RunAsync : FunctionAppTestBase<ChargesFunctionAppFixture>, IAsyncLifetime
        {
            public RunAsync(ChargesFunctionAppFixture fixture, ITestOutputHelper testOutputHelper)
                : base(fixture, testOutputHelper)
            {
            }

            [Fact]
            public async Task When_ReceivingCreateDefaultChargeLinksRequest_MessageHubIsNotifiedAboutAvailableData_And_Then_When_MessageHubRequestsTheBundle_Then_MessageHubReceivesBundleReply()
            {
                // Arrange
                var meteringPointId = "571313180000000029";
                var request = CreateServiceBusMessage(
                    meteringPointId,
                    Fixture.CreateLinkReplyQueue.Name,
                    out var correlationId,
                    out var parentId);

                // Act
                await MockTelemetryClient.WrappedOperationWithTelemetryDependencyInformationAsync(
                    () => Fixture.CreateLinkRequestQueue.SenderClient.SendMessageAsync(request), correlationId, parentId);

                // Assert
                await Fixture.MessageHubMock.AssertPeekReceivesRepliesAsync(correlationId);
            }

            [Theory]
            [InlineData("571313180000000012")]
            [InlineData("571313180000000045")]
            public async Task When_ReceivingCreateDefaultChargeLinksRequest_MeteringPointDomainIsNotifiedThatDefaultChargeLinksAreCreated(
                string meteringPointId)
            {
                // Arrange
                var request = CreateServiceBusMessage(
                    meteringPointId,
                    Fixture.CreateLinkReplyQueue.Name,
                    out var correlationId,
                    out var parentId);

                using var isMessageReceived = await Fixture.CreateLinkReplyQueueListener
                    .ListenForMessageAsync(correlationId)
                    .ConfigureAwait(false);

                // Act
                await MockTelemetryClient.WrappedOperationWithTelemetryDependencyInformationAsync(
                    () => Fixture.CreateLinkRequestQueue.SenderClient.SendMessageAsync(request), correlationId, parentId);

                // Assert
                var isMessageReceivedByQueue = isMessageReceived.MessageAwaiter!.Wait(TimeSpan.FromSeconds(60));
                isMessageReceivedByQueue.Should().BeTrue();
            }

            public Task InitializeAsync()
            {
                return Task.CompletedTask;
            }

            public Task DisposeAsync()
            {
                Fixture.MessageHubMock.Clear();
                return Task.CompletedTask;
            }

            private ServiceBusMessage CreateServiceBusMessage(
                string meteringPointId, string replyToQueueName, out string correlationId, out string parentId)
            {
                correlationId = CorrelationIdGenerator.Create();
                var message = new CreateDefaultChargeLinks { MeteringPointId = meteringPointId };
                parentId = $"00-{correlationId}-b7ad6b7169203331-01";

                var actor = JsonSerializer.Serialize(new Actor(
                    SeededData.GridAreaLink.Provider8100000000030.Id,
                    "???",
                    "???",
                    MarketParticipantRole.GridAccessProvider.ToString()));

                var byteArray = message.ToByteArray();
                var serviceBusMessage = new ServiceBusMessage(byteArray)
                {
                    CorrelationId = correlationId,
                    ApplicationProperties =
                    {
                        new KeyValuePair<string, object>("ReplyTo", replyToQueueName),
                        new KeyValuePair<string, object>(Constants.ServiceBusIdentityKey, actor),
                    },
                };
                return serviceBusMessage;
            }
        }
    }
}
