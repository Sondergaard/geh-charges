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

using System.Linq;
using System.Threading.Tasks;
using Energinet.Charges.Contracts.Charge;
using Energinet.Charges.Contracts.ChargeLink;
using GreenEnergyHub.Charges.QueryApi;
using GreenEnergyHub.Charges.QueryApi.Model;
using GreenEnergyHub.Charges.QueryApi.QueryPredicates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GreenEnergyHub.Charges.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChargeLinksController : ControllerBase
    {
        private readonly IData _data;

        public ChargeLinksController(IData data)
        {
            _data = data;
        }

        /// <summary>
        /// Returns all charge links data for a given metering point. Currently it returns mocked data.
        /// </summary>
        /// <param name="meteringPointId">The 18-digits metering point identifier used by the Danish version of Green Energy Hub.
        /// Use 404 to get a "404 Not Found" response.</param>
        /// <returns>Mocked charge links data or "404 Not Found"</returns>
        [HttpGet("GetAsync")]
        public async Task<IActionResult> GetAsync(string meteringPointId)
        {
            if (meteringPointId == null)
                return BadRequest();

            // TODO BJARKE: Make ChargeLinkDto instantiation reusable
            var chargeLink = await _data
                .ChargeLinks
                .ForMeteringPoint(meteringPointId)
                .Select(c => CreateChargeLinkDto(c))
                .SingleOrDefaultAsync();

            if (chargeLink == null)
                return NotFound();

            return Ok(chargeLink);
        }

        private static ChargeLinkDto CreateChargeLinkDto(ChargeLink c)
        {
            return new ChargeLinkDto(
                (ChargeType)c.Charge.GetChargeType(), // TODO BJARKE: Map correctly
                c.Charge.SenderProvidedChargeId,
                c.Charge.Name,
                c.Charge.Owner.MarketParticipantId,
                "Netvirksomhed XYZ", // Hardcoded as we currently don't have the data
                c.Charge.TaxIndicator,
                c.Charge.TransparentInvoicing,
                c.Factor,
                c.StartDateTime,
                c.EndDateTime);
        }
    }
}
