﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

#nullable disable

namespace GreenEnergyHub.Charges.QueryApi.ScaffoldedModels
{
    public partial class ChargePeriodDetails
    {
        public Guid Id { get; set; }
        public Guid ChargeId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int VatClassification { get; set; }
        public bool Retired { get; set; }
        public DateTime? RetiredDateTime { get; set; }
        public Guid ChargeOperationId { get; set; }

        public virtual Charge Charge { get; set; }
        public virtual ChargeOperation ChargeOperation { get; set; }
    }
}