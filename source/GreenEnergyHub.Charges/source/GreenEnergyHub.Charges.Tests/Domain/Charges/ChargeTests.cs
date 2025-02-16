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
using System.Linq;
using Energinet.DataHub.Core.TestCommon.AutoFixture.Attributes;
using FluentAssertions;
using GreenEnergyHub.Charges.Domain.Charges;
using GreenEnergyHub.Charges.Domain.Charges.Exceptions;
using GreenEnergyHub.Charges.Domain.Dtos.Validation;
using GreenEnergyHub.Charges.TestCore;
using GreenEnergyHub.Charges.Tests.Builders.Command;
using Moq;
using NodaTime;
using Xunit;
using Xunit.Categories;

namespace GreenEnergyHub.Charges.Tests.Domain.Charges
{
    [UnitTest]
    public class ChargeTests
    {
        [Theory]
        [InlineAutoMoqData(TaxIndicator.NoTax, TaxIndicator.Tax)]
        [InlineAutoMoqData(TaxIndicator.Tax, TaxIndicator.NoTax)]
        public void UpdateCharge_NewPeriodChangeTaxIndicator_ShouldThrowExceptionWithInvalidRule(
            TaxIndicator existingTax,
            TaxIndicator newTax)
        {
            // Arrange
            var sut = new ChargeBuilder()
                .WithName("ExistingPeriod")
                .WithTaxIndicator(existingTax)
                .WithStartDate(InstantHelper.GetTodayAtMidnightUtc())
                .Build();

            var newPeriod = new ChargePeriodBuilder()
                .WithName("NewPeriod")
                .WithStartDateTime(InstantHelper.GetTomorrowAtMidnightUtc())
                .Build();

            // Act & Assert
            var ex = Assert.Throws<ChargeOperationFailedException>(() => sut.Update(
                newPeriod,
                newTax,
                sut.Resolution,
                Guid.NewGuid().ToString()));
            ex.InvalidRules.Should().Contain(r =>
                r.ValidationRule.ValidationRuleIdentifier == ValidationRuleIdentifier.ChangingTariffTaxValueNotAllowed);
        }

        [Fact]
        public void UpdateCharge_NewPeriodInsideSingleExistingPeriod_SetsNewEndDateForExistingPeriodAndInsertsNewPeriod()
        {
            // Arrange
            var sut = new ChargeBuilder()
                .WithName("ExistingPeriod")
                .WithStartDate(InstantHelper.GetTodayAtMidnightUtc())
                .Build();

            var newPeriod = new ChargePeriodBuilder()
                .WithName("NewPeriod")
                .WithStartDateTime(InstantHelper.GetTomorrowAtMidnightUtc())
                .Build();

            // Act
            sut.Update(
                newPeriod,
                sut.TaxIndicator ? TaxIndicator.Tax : TaxIndicator.NoTax,
                sut.Resolution,
                Guid.NewGuid().ToString());

            // Assert
            var actualTimeline = sut.Periods.OrderBy(p => p.StartDateTime).ToList();
            var actualFirstPeriod = actualTimeline[0];
            var actualSecondPeriod = actualTimeline[1];

            actualTimeline.Should().HaveCount(2);
            actualFirstPeriod.Name.Should().Be("ExistingPeriod");
            actualFirstPeriod.StartDateTime.Should().Be(InstantHelper.GetTodayAtMidnightUtc());
            actualFirstPeriod.EndDateTime.Should().Be(InstantHelper.GetTomorrowAtMidnightUtc());
            actualSecondPeriod.Name.Should().Be("NewPeriod");
            actualSecondPeriod.StartDateTime.Should().Be(InstantHelper.GetTomorrowAtMidnightUtc());
            actualSecondPeriod.EndDateTime.Should().Be(InstantHelper.GetEndDefault());
        }

        [Fact]
        public void UpdateCharge_NewPeriodStartEqualsExistingPeriodStart_NewPeriodOverwritesExisting()
        {
            // Arrange
            var sut = new ChargeBuilder()
                .WithName("FirstPeriod")
                .WithStartDate(InstantHelper.GetYesterdayAtMidnightUtc())
                .AddPeriods(AddTwoPeriods())
                .Build();

            var newPeriod = new ChargePeriodBuilder()
                .WithName("UpdatedPeriod")
                .WithStartDateTime(InstantHelper.GetTomorrowAtMidnightUtc())
                .Build();

            // Act
            sut.Update(
                newPeriod,
                sut.TaxIndicator ? TaxIndicator.Tax : TaxIndicator.NoTax,
                sut.Resolution,
                Guid.NewGuid().ToString());

            // Assert
            var actualTimeline = sut.Periods.OrderBy(p => p.StartDateTime).ToList();
            var actualFirstPeriod = actualTimeline[0];
            var actualSecondPeriod = actualTimeline[1];

            actualTimeline.Should().HaveCount(2);
            actualFirstPeriod.Name.Should().Be("FirstPeriod");
            actualFirstPeriod.StartDateTime.Should().Be(InstantHelper.GetYesterdayAtMidnightUtc());
            actualFirstPeriod.EndDateTime.Should().Be(InstantHelper.GetTomorrowAtMidnightUtc());
            actualSecondPeriod.Name.Should().Be("UpdatedPeriod");
            actualSecondPeriod.StartDateTime.Should().Be(InstantHelper.GetTomorrowAtMidnightUtc());
            actualSecondPeriod.EndDateTime.Should().Be(InstantHelper.GetEndDefault());
        }

        [Fact]
        public void UpdateCharge_NewPeriodStartsBeforeExistingPeriod_NewPeriodOverwritesExisting()
        {
            // Arrange
            var sut = new ChargeBuilder()
                .WithStartDate(InstantHelper.GetTodayAtMidnightUtc())
                .Build();

            var newPeriod = new ChargePeriodBuilder()
                .WithName("NewPeriod")
                .WithStartDateTime(InstantHelper.GetYesterdayAtMidnightUtc())
                .Build();

            // Act
            sut.Update(
                newPeriod,
                sut.TaxIndicator ? TaxIndicator.Tax : TaxIndicator.NoTax,
                sut.Resolution,
                Guid.NewGuid().ToString());

            // Assert
            var actualFirstPeriod = sut.Periods.Single();

            actualFirstPeriod.Name.Should().Be("NewPeriod");
            actualFirstPeriod.StartDateTime.Should().Be(InstantHelper.GetYesterdayAtMidnightUtc());
            actualFirstPeriod.EndDateTime.Should().Be(InstantHelper.GetEndDefault());
        }

        [Theory]
        [InlineAutoMoqData]
        public void UpdateCharge_WhenChargePeriodIsNull_ThrowsArgumentNullException(ChargeBuilder chargeBuilder)
        {
            var sut = chargeBuilder.Build();
            ChargePeriod? chargePeriod = null;

            Assert.Throws<ArgumentNullException>(() => sut.Update(
                chargePeriod!,
                sut.TaxIndicator ? TaxIndicator.Tax : TaxIndicator.NoTax,
                sut.Resolution,
                It.IsAny<string>()));
        }

        [Fact]
        public void UpdateCharge_WhenEndDateIsBound_ThenThrowException()
        {
            // Arrange
            var existingPeriod = new ChargePeriodBuilder()
                .WithName("ExistingPeriod")
                .WithStartDateTime(InstantHelper.GetTodayAtMidnightUtc())
                .Build();

            var sut = new ChargeBuilder()
                .AddPeriod(existingPeriod)
                .Build();

            var newPeriod = new ChargePeriodBuilder()
                .WithStartDateTime(InstantHelper.GetYesterdayAtMidnightUtc())
                .WithEndDateTime(InstantHelper.GetTomorrowAtMidnightUtc())
                .Build();

            // Act
            Assert.Throws<InvalidOperationException>(() => sut.Update(
                newPeriod,
                sut.TaxIndicator ? TaxIndicator.Tax : TaxIndicator.NoTax,
                sut.Resolution,
                Guid.NewGuid().ToString()));
        }

        [Fact]
        public void UpdateCharge_NewPeriodStartsBeforeExistingStopDate_InsertNewPeriodAndKeepStopDate()
        {
            // Arrange
            var sut = new ChargeBuilder()
                .WithStopDate(InstantHelper.GetTodayPlusDaysAtMidnightUtc(4))
                .Build();
            var newPeriod = new ChargePeriodBuilder()
                .WithName("New")
                .WithStartDateTime(InstantHelper.GetTodayAtMidnightUtc())
                .Build();

            // Act
            sut.Update(
                newPeriod,
                sut.TaxIndicator ? TaxIndicator.Tax : TaxIndicator.NoTax,
                sut.Resolution,
                Guid.NewGuid().ToString());

            // Assert
            var actualTimeline = sut.Periods.OrderBy(p => p.StartDateTime).ToList();
            actualTimeline.Should().HaveCount(2);
            var actualFirstPeriod = actualTimeline[0];
            var actualSecondPeriod = actualTimeline[1];
            actualFirstPeriod.Name.Should().Be("ChargeBuilderDefaultName");
            actualFirstPeriod.StartDateTime.Should().Be(InstantHelper.GetStartDefault());
            actualFirstPeriod.EndDateTime.Should().Be(InstantHelper.GetTodayAtMidnightUtc());
            actualSecondPeriod.Name.Should().Be("New");
            actualSecondPeriod.StartDateTime.Should().Be(InstantHelper.GetTodayAtMidnightUtc());
            actualSecondPeriod.EndDateTime.Should().Be(InstantHelper.GetTodayPlusDaysAtMidnightUtc(4));
        }

        [Fact]
        public void StopCharge_WhenStopDateEqualsSingleExistingChargePeriodStartDate_RemovePeriod()
        {
            // Arrange
            var dayAfterTomorrow = InstantHelper.GetTodayPlusDaysAtMidnightUtc(2);
            var sut = new ChargeBuilder().WithStartDate(dayAfterTomorrow).Build();

            // Act
            sut.Stop(dayAfterTomorrow);

            // Assert
            sut.Periods.Count.Should().Be(0);
        }

        [Fact]
        public void StopCharge_WhenSingleExistingChargePeriod_SetNewEndDate()
        {
            // Arrange
            var sut = new ChargeBuilder().WithStartDate(InstantHelper.GetTodayAtMidnightUtc()).Build();
            var dayAfterTomorrow = InstantHelper.GetTodayPlusDaysAtMidnightUtc(2);

            // Act
            sut.Stop(dayAfterTomorrow);

            // Assert
            var actual = sut.Periods.Single();
            actual.EndDateTime.Should().BeEquivalentTo(dayAfterTomorrow);
        }

        [Fact]
        public void StopCharge_WhenExistingPeriods_ThenRemoveAllAfterStop()
        {
            // Arrange
            var sut = new ChargeBuilder()
                .WithName("FirstPeriod")
                .WithStartDate(InstantHelper.GetYesterdayAtMidnightUtc())
                .AddPeriods(AddTwoPeriods())
                .Build();

            var stopDate = InstantHelper.GetTomorrowAtMidnightUtc();

            // Act
            sut.Stop(stopDate);

            // Assert
            sut.Periods.Count.Should().Be(1);
            sut.Periods.Single().EndDateTime.Should().Be(stopDate);
        }

        [Fact]
        public void StopCharge_WhenStopExistAfterStopDate_ThenNewStopReplaceOldStop()
        {
            // Arrange
            var sut = new ChargeBuilder()
                .WithStartDate(InstantHelper.GetTodayAtMidnightUtc())
                .WithStopDate(InstantHelper.GetTodayPlusDaysAtMidnightUtc(10))
                .Build();
            var stopDate = InstantHelper.GetTomorrowAtMidnightUtc();

            // Act
            sut.Stop(stopDate);

            // Assert
            sut.Periods.Single().EndDateTime.Should().Be(stopDate);
        }

        [Fact]
        public void StopCharge_WhenStopExistBeforeStopDate_ThenThrowException()
        {
            // Arrange
            var sut = new ChargeBuilder()
                .WithStartDate(InstantHelper.GetTodayAtMidnightUtc())
                .WithStopDate(InstantHelper.GetTodayPlusDaysAtMidnightUtc(5))
                .Build();
            var stopDate = InstantHelper.GetTodayPlusDaysAtMidnightUtc(10);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => sut.Stop(stopDate));
        }

        [Fact]
        public void StopCharge_WhenChargeStartDateAfterStopDate_ThenThrowException()
        {
            // Arrange
            var sut = new ChargeBuilder().WithStartDate(InstantHelper.GetTodayPlusDaysAtMidnightUtc(5)).Build();
            var stopDate = InstantHelper.GetTodayAtMidnightUtc();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => sut.Stop(stopDate));
        }

        [Fact]
        public void StopCharge_WhenNewEndDateIsNull_ThenThrowException()
        {
            // Arrange
            var sut = new ChargeBuilder().Build();
            Instant? stopDate = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => sut.Stop(stopDate));
        }

        [Fact]
        public void StopCharge_WhenPointsExistOnAndAfterStopDate_PointsRemoved()
        {
            // Arrange
            var points = new List<Point>
            {
                new(0, decimal.One, InstantHelper.GetTodayPlusDaysAtMidnightUtc(0)),
                new(0, decimal.One, InstantHelper.GetTodayPlusDaysAtMidnightUtc(1)),
                new(0, decimal.One, InstantHelper.GetTodayPlusDaysAtMidnightUtc(2)),
                new(0, decimal.One, InstantHelper.GetTodayPlusDaysAtMidnightUtc(3)),
                new(0, decimal.One, InstantHelper.GetTodayPlusDaysAtMidnightUtc(4)),
                new(0, decimal.One, InstantHelper.GetTodayPlusDaysAtMidnightUtc(5)),
            };
            var sut = new ChargeBuilder()
                .WithStartDate(InstantHelper.GetTodayPlusDaysAtMidnightUtc(0))
                .WithPoints(points).Build();

            // Act
            sut.Stop(InstantHelper.GetTodayPlusDaysAtMidnightUtc(2));

            // Assert
            sut.Points.Count.Should().Be(2);
        }

        [Fact]
        public void CancelStop_WhenStopPeriodExists_ThenAddNewLastPeriod()
        {
            // Arrange
            var stopDate = InstantHelper.GetTomorrowAtMidnightUtc();
            var sut = new ChargeBuilder().WithName("stopped").WithStopDate(stopDate).Build();

            var newPeriod = new ChargePeriodBuilder()
                .WithName("cancelledStop")
                .WithStartDateTime(stopDate)
                .Build();

            // Act
            sut.CancelStop(
                newPeriod,
                sut.TaxIndicator ? TaxIndicator.Tax : TaxIndicator.NoTax,
                sut.Resolution,
                Guid.NewGuid().ToString());

            // Assert
            var orderedPeriods = sut.Periods.OrderBy(p => p.StartDateTime).ToList();
            var actualFirst = orderedPeriods.First();
            var actualLast = orderedPeriods.Last();
            actualFirst.Name.Should().Be("stopped");
            actualFirst.StartDateTime.Should().BeEquivalentTo(InstantHelper.GetStartDefault());
            actualFirst.EndDateTime.Should().BeEquivalentTo(stopDate);
            actualLast.Name.Should().Be("cancelledStop");
            actualLast.StartDateTime.Should().BeEquivalentTo(stopDate);
            actualLast.EndDateTime.Should().BeEquivalentTo(InstantHelper.GetEndDefault());
        }

        [Fact]
        public void CancelStop_WhenChargeNotStopped_ThenThrowException()
        {
            // Arrange
            var sut = new ChargeBuilder().Build();
            var cancelPeriod = new ChargePeriodBuilder().WithStartDateTime(InstantHelper.GetTodayAtMidnightUtc()).Build();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => sut.CancelStop(
                cancelPeriod,
                sut.TaxIndicator ? TaxIndicator.Tax : TaxIndicator.NoTax,
                sut.Resolution,
                Guid.NewGuid().ToString()));
        }

        [Fact]
        public void UpdatePrices_WhenPointsExistBetweenStartAndStopDate_PointsUpdated()
        {
            // Arrange
            var points = new List<Point>
            {
                new(0, 1.00m, InstantHelper.GetTodayPlusDaysAtMidnightUtc(0)),
                new(0, 2.00m, InstantHelper.GetTodayPlusDaysAtMidnightUtc(1)),
                new(0, 3.00m, InstantHelper.GetTodayPlusDaysAtMidnightUtc(2)),
                new(0, 4.00m, InstantHelper.GetTodayPlusDaysAtMidnightUtc(3)),
                new(0, 5.00m, InstantHelper.GetTodayPlusDaysAtMidnightUtc(4)),
            };
            var newPrices = new List<Point>
            {
                new(0, 3.50m, InstantHelper.GetTodayPlusDaysAtMidnightUtc(2)),
                new(0, 4.50m, InstantHelper.GetTodayPlusDaysAtMidnightUtc(3)),
            };

            var sut = new ChargeBuilder()
                .WithStartDate(InstantHelper.GetTodayPlusDaysAtMidnightUtc(0))
                .WithPoints(points)
                .Build();

            // Act
            sut.UpdatePrices(
                InstantHelper.GetTodayPlusDaysAtMidnightUtc(2),
                InstantHelper.GetTodayPlusDaysAtMidnightUtc(4),
                newPrices,
                Guid.NewGuid().ToString());

            // Assert
            sut.Points.Count.Should().Be(5);
            var actualPriceFirstUpdated = sut.Points.Single(x => x.Time == InstantHelper.GetTodayPlusDaysAtMidnightUtc(2));
            var actualPriceSecondUpdated = sut.Points.Single(x => x.Time == InstantHelper.GetTodayPlusDaysAtMidnightUtc(3));
            actualPriceFirstUpdated.Price.Should().Be(3.50m);
            actualPriceSecondUpdated.Price.Should().Be(4.50m);
        }

        [Fact]
        public void UpdatePrices_WhenNoPointsExistBetweenStartAndStopDate_PointsUpdated()
        {
            // Arrange
            var points = new List<Point>
            {
                new(0, 1.00m, InstantHelper.GetTodayPlusDaysAtMidnightUtc(0)),
                new(0, 2.00m, InstantHelper.GetTodayPlusDaysAtMidnightUtc(1)),
                new(0, 3.00m, InstantHelper.GetTodayPlusDaysAtMidnightUtc(2)),
                new(0, 4.00m, InstantHelper.GetTodayPlusDaysAtMidnightUtc(3)),
                new(0, 5.00m, InstantHelper.GetTodayPlusDaysAtMidnightUtc(4)),
            };
            var newPrices = new List<Point>
            {
                new(0, 6.00m, InstantHelper.GetTodayPlusDaysAtMidnightUtc(5)),
                new(0, 7.00m, InstantHelper.GetTodayPlusDaysAtMidnightUtc(6)),
            };

            var sut = new ChargeBuilder()
                .WithStartDate(InstantHelper.GetTodayPlusDaysAtMidnightUtc(0))
                .WithPoints(points)
                .Build();

            // Act
            sut.UpdatePrices(
                InstantHelper.GetTodayPlusDaysAtMidnightUtc(5),
                InstantHelper.GetTodayPlusDaysAtMidnightUtc(7),
                newPrices,
                Guid.NewGuid().ToString());

            // Assert
            sut.Points.Count.Should().Be(7);
            var actualPriceFirstAdded = sut.Points.Single(x => x.Time == InstantHelper.GetTodayPlusDaysAtMidnightUtc(5));
            var actualPriceSecondAdded = sut.Points.Single(x => x.Time == InstantHelper.GetTodayPlusDaysAtMidnightUtc(6));
            actualPriceFirstAdded.Price.Should().Be(6.00m);
            actualPriceSecondAdded.Price.Should().Be(7.00m);
        }

        [Fact]
        public void Create_WithEndDate_ThrowsExceptionWithInvalidRule()
        {
            // Act / Assert
            var ex = Assert.Throws<ChargeOperationFailedException>(() => Charge.Create(
                "id1",
                "name",
                "desc",
                "senderId",
                Guid.NewGuid(),
                ChargeType.Fee,
                Resolution.P1D,
                TaxIndicator.NoTax,
                VatClassification.NoVat,
                false,
                InstantHelper.GetTodayAtMidnightUtc(),
                InstantHelper.GetTomorrowAtMidnightUtc()));
            ex.InvalidRules.Should().Contain(r =>
                r.ValidationRule.ValidationRuleIdentifier == ValidationRuleIdentifier.CreateChargeIsNotAllowedATerminationDate);
        }

        [Fact]
        public void Create_WithNoEndDate_Succeed()
        {
            // Act
            var charge = Charge.Create(
                "id1",
                "name",
                "desc",
                "senderId",
                Guid.NewGuid(),
                ChargeType.Fee,
                Resolution.P1D,
                TaxIndicator.NoTax,
                VatClassification.NoVat,
                false,
                InstantHelper.GetTodayAtMidnightUtc());

            // Assert
            charge.Should().NotBeNull();
        }

        private static List<ChargePeriod> AddTwoPeriods()
        {
            return new List<ChargePeriod>
            {
                new ChargePeriodBuilder()
                    .WithName("SecondPeriod")
                    .WithStartDateTime(InstantHelper.GetTomorrowAtMidnightUtc())
                    .Build(),
                new ChargePeriodBuilder()
                    .WithName("ThirdPeriod")
                    .WithStartDateTime(InstantHelper.GetTodayPlusDaysAtMidnightUtc(2))
                    .Build(),
            };
        }
    }
}
