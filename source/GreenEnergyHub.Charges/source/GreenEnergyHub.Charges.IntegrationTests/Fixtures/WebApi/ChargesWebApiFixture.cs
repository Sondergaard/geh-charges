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
using GreenEnergyHub.Charges.IntegrationTests.Authorization;
using GreenEnergyHub.Charges.IntegrationTests.Fixtures.Database;
using GreenEnergyHub.Charges.WebApi;
using Microsoft.Extensions.Configuration;

namespace GreenEnergyHub.Charges.IntegrationTests.Fixtures.WebApi
{
    public class ChargesWebApiFixture : WebApiFixture
    {
        public ChargesWebApiFixture()
        {
            DatabaseManager = new ChargesDatabaseManager();
            AuthorizationConfiguration = new AuthorizationConfiguration();
        }

        public ChargesDatabaseManager DatabaseManager { get; }

        public AuthorizationConfiguration AuthorizationConfiguration { get; }

        /// <inheritdoc/>
        protected override void OnConfigureEnvironment()
        {
        }

        /// <inheritdoc/>
        protected override async Task OnInitializeWebApiDependenciesAsync(IConfiguration localSettingsSnapshot)
        {
            // => Database
            await DatabaseManager.CreateDatabaseAsync();

            // Overwrites the setting so the Web Api app uses the database we have control of in the test
            Environment.SetEnvironmentVariable(
                $"CONNECTIONSTRINGS:{EnvironmentSettingNames.ChargeDbConnectionString}",
                DatabaseManager.ConnectionString);

            // When supported the url will have to be overwritten from test.common
            //Environment.SetEnvironmentVariable(EnvironmentSettingNames.FrontEndOpenIdUrl, AuthorizationConfiguration.FrontEndOpenIdUrl);
            Environment.SetEnvironmentVariable(EnvironmentSettingNames.FrontEndServiceAppId, AuthorizationConfiguration.FrontendAppId);
        }

        /// <inheritdoc/>
        protected override async Task OnDisposeWebApiDependenciesAsync()
        {
            // => Database
            await DatabaseManager.DeleteDatabaseAsync();
        }
    }
}
