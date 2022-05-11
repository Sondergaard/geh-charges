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

using System.Collections.Generic;
using GreenEnergyHub.Charges.Domain.ChargeInformation;
using GreenEnergyHub.Charges.Domain.ChargePrices;
using GreenEnergyHub.Charges.Domain.Common;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands;
using NodaTime;

namespace GreenEnergyHub.Charges.Tests.Builders.Command
{
   public class ChargeOperationDtoBuilder
    {
        private List<Point> _points;
        private string _chargeInformationId;
        private Instant _startDateTime;
        private Instant? _endDateTime;
        private VatClassification _vatClassification;
        private TransparentInvoicing _transparentInvoicing;
        private TaxIndicator _taxIndicator;
        private string _owner;
        private string _description;
        private string _chargeName;
        private ChargeType _chargeType;
        private Resolution _resolution;
        private string _operationId;

        public ChargeOperationDtoBuilder()
        {
            _operationId = "operationId";
            _chargeInformationId = "some charge id";
            _startDateTime = SystemClock.Instance.GetCurrentInstant()
                .Plus(Duration.FromDays(500));
            _endDateTime = SystemClock.Instance.GetCurrentInstant()
                .Plus(Duration.FromDays(1000));
            _vatClassification = VatClassification.Vat25;
            _taxIndicator = TaxIndicator.Tax;
            _owner = "owner";
            _description = "some description";
            _chargeName = "some charge name";
            _chargeType = ChargeType.Fee;
            _points = new List<Point>();
            _resolution = Resolution.PT1H;
        }

        public ChargeOperationDtoBuilder WithDescription(string description)
        {
            _description = description;
            return this;
        }

        public ChargeOperationDtoBuilder WithChargeName(string name)
        {
            _chargeName = name;
            return this;
        }

        public ChargeOperationDtoBuilder WithChargeOperationId(string operationId)
        {
            _operationId = operationId;
            return this;
        }

        public ChargeOperationDtoBuilder WithChargeInformationId(string chargeInformationId)
        {
            _chargeInformationId = chargeInformationId;
            return this;
        }

        public ChargeOperationDtoBuilder WithTaxIndicator(TaxIndicator taxIndicator)
        {
            _taxIndicator = taxIndicator;
            return this;
        }

        public ChargeOperationDtoBuilder WithOwner(string owner)
        {
            _owner = owner;
            return this;
        }

        public ChargeOperationDtoBuilder WithVatClassification(VatClassification vatClassification)
        {
            _vatClassification = vatClassification;
            return this;
        }

        public ChargeOperationDtoBuilder WithTransparentInvoicing(TransparentInvoicing transparentInvoicing)
        {
            _transparentInvoicing = transparentInvoicing;
            return this;
        }

        public ChargeOperationDtoBuilder WithChargeType(ChargeType type)
        {
            _chargeType = type;
            return this;
        }

        public ChargeOperationDtoBuilder WithStartDateTime(Instant startDateTime)
        {
            _startDateTime = startDateTime;
            return this;
        }

        public ChargeOperationDtoBuilder WithEndDateTime(Instant endDateTime)
        {
            _endDateTime = endDateTime;
            return this;
        }

        public ChargeOperationDtoBuilder WithPoints(List<Point> points)
        {
            _points = points;
            return this;
        }

        public ChargeOperationDtoBuilder WithPoint(int position, decimal price)
        {
            _points.Add(new Point(position, price, SystemClock.Instance.GetCurrentInstant()));
            return this;
        }

        public ChargeOperationDtoBuilder WithPointWithXNumberOfPrices(int numberOfPrices)
        {
            for (var i = 0; i < numberOfPrices; i++)
            {
                var point = new Point(i + 1, i * 10, SystemClock.Instance.GetCurrentInstant());
                _points.Add(point);
            }

            return this;
        }

        public ChargeOperationDtoBuilder WithResolution(Resolution resolution)
        {
            _resolution = resolution;
            return this;
        }

        public ChargeOperationDto Build()
        {
            return new ChargeOperationDto(
                _operationId,
                _chargeType,
                _chargeInformationId,
                _chargeName,
                _description,
                _owner,
                _resolution,
                _taxIndicator,
                _transparentInvoicing,
                _vatClassification,
                _startDateTime,
                _endDateTime,
                _points);
        }
    }
}
