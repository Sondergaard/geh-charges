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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace GreenEnergyHub.Charges.QueryApi.Model
{
    [Table("Charge", Schema = "Charges")]
    [Index(nameof(SenderProvidedChargeId), nameof(Type), nameof(OwnerId), Name = "IX_SenderProvidedChargeId_ChargeType_MarketParticipantId")]
    public partial class Charge
    {
        public Charge()
        {
            ChargeLinks = new HashSet<ChargeLink>();
            ChargePeriods = new HashSet<ChargePeriod>();
            ChargePoints = new HashSet<ChargePoint>();
            DefaultChargeLinks = new HashSet<DefaultChargeLink>();
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(35)]
        public string SenderProvidedChargeId { get; set; }

        public int Type { get; set; }

        public Guid OwnerId { get; set; }

        public bool TaxIndicator { get; set; }

        public int Resolution { get; set; }

        [ForeignKey(nameof(OwnerId))]
        [InverseProperty(nameof(MarketParticipant.Charges))]
        public virtual MarketParticipant Owner { get; set; }

        [InverseProperty(nameof(ChargeLink.Charge))]
        public virtual ICollection<ChargeLink> ChargeLinks { get; set; }

        [InverseProperty(nameof(ChargePeriod.Charge))]
        public virtual ICollection<ChargePeriod> ChargePeriods { get; set; }

        [InverseProperty(nameof(ChargePoint.Charge))]
        public virtual ICollection<ChargePoint> ChargePoints { get; set; }

        [InverseProperty(nameof(DefaultChargeLink.Charge))]
        public virtual ICollection<DefaultChargeLink> DefaultChargeLinks { get; set; }
    }
}
