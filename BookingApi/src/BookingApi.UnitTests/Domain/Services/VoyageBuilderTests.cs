using System;
using System.Collections.Generic;
using BookingApi.Domain.Location;
using BookingApi.Domain.Services;
using BookingApi.Domain.Voyage;
using Ploeh.AutoFixture;
using Xunit;

namespace BookingApi.UnitTests.Domain.Services
{
	public class VoyageBuilderTests
	{
		[Fact]
		public void creation_with_empty_voyagenumber_should_throw_exception()
		{
			Assert.Throws<ArgumentNullException>(
					delegate
					{
						new VoyageBuilder(null, new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), ""));
					}
				);
		}

		[Fact]
		public void creation_with_empty_location_should_throw_exception()
		{
			Assert.Throws<ArgumentNullException>(
				   delegate
				   {
					   new VoyageBuilder(new VoyageNumber(""), null);
				   }
			   );
		}

		[Fact]
		public void creation_with_empty_voyagenumber_and_location_should_throw_exception()
		{
			Assert.Throws<ArgumentNullException>(
				   delegate
				   {
					   new VoyageBuilder(null, null);
				   }
			   );
		}

		[Fact]
		public void addMovement_returns_valid_instance_with_valid_inputs()
		{
			var fixture = new Fixture();
			fixture.Register(UnLocodeHelpers.GetNewUnLocode);
			var voyageBuilder = fixture.CreateAnonymous<VoyageBuilder>();
			var arrivalLocation = fixture.CreateAnonymous<BookingApi.Domain.Location.Location>();
			var departureTime = fixture.CreateAnonymous<DateTime>();
			var arrivalTime = fixture.CreateAnonymous<DateTime>();
			Assert.NotNull(voyageBuilder.AddMovement(arrivalLocation, departureTime, arrivalTime));
		}

		[Fact]
		public void addMovement_changes_arrivalLocation_to_departureLocation_for_next_movement()
		{
			var fixture = new Fixture();
			fixture.Register(UnLocodeHelpers.GetNewUnLocode);
			var voyageBuilder = fixture.CreateAnonymous<VoyageBuilder>();
			var arrivalLocation = fixture.CreateAnonymous<BookingApi.Domain.Location.Location>();
			var departureTime = fixture.CreateAnonymous<DateTime>();
			var arrivalTime = fixture.CreateAnonymous<DateTime>();

			voyageBuilder.AddMovement(arrivalLocation, departureTime, arrivalTime)
				.AddMovement(new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "GDANSK"), DateTime.Now, DateTime.Now)
				.AddMovement(new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "HAMBOURG"), DateTime.Now, DateTime.Now)
				.AddMovement(new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "LE HAVRE"), DateTime.Now, DateTime.Now);

			var voyage = voyageBuilder.Build();

			Assert.True(voyage.Schedule.CarrierMouvements[1].DepartureLocation == voyage.Schedule.CarrierMouvements[0].ArrivalLocation);
			Assert.True(voyage.Schedule.CarrierMouvements[2].DepartureLocation == voyage.Schedule.CarrierMouvements[1].ArrivalLocation);
		}

		[Fact]
		public void build_returns_valid_voyage_instance_with_valid_inputs()
		{
			// AUTOFIXTURE
			//var fixture = new Fixture();

			//var departureLocation = fixture.CreateAnonymous<Location>();
			//var arrivalLocation = fixture.CreateAnonymous<Location>();
			//var departureTime = fixture.CreateAnonymous<DateTime>();
			//var arrivalTime = fixture.CreateAnonymous<DateTime>().AddDays(11);

			//fixture.Register(UnLocodeHelpers.GetNewUnLocode);
			//var voyageNumber = fixture.CreateAnonymous<VoyageNumber>();
			//fixture.Register(() => new VoyageBuilder(voyageNumber, departureLocation));
			//var voyageBuilder = fixture.CreateAnonymous<VoyageBuilder>();

			//fixture.Register(() => new CarrierMovement(departureLocation, arrivalLocation, departureTime, arrivalTime));
			//var carrierMovements = fixture.CreateMany<CarrierMovement>(1).ToList();
			//fixture.Register(() => new Schedule(carrierMovements));
			//var schedule = fixture.CreateAnonymous<Schedule>();

			//var voyage = voyageBuilder.AddMovement(arrivalLocation, departureTime, arrivalTime).Build();

			//Assert.True(voyage.Number.Equals(voyageNumber));
			//Assert.True(voyage.Schedule.Equals(schedule));

			var voyageNumber = new VoyageNumber("12");
			var departureLocation = new BookingApi.Domain.Location.Location(new UnLocode("AZ23H"), "HAMBOURG");
			var arrivalLocation = new BookingApi.Domain.Location.Location(new UnLocode("XE44K"), "TUNIS");
			var departureDataTime = new DateTime(2010, 4, 10);
			var arrivalDataTime = new DateTime(2010, 5, 15);
			var schedule =
				new Schedule(new List<CarrierMovement>
								 {
									 new CarrierMovement(departureLocation, arrivalLocation, departureDataTime,
														 arrivalDataTime)
								 });

			BookingApi.Domain.Voyage.Voyage voyage = new VoyageBuilder(voyageNumber, departureLocation)
				.AddMovement(arrivalLocation, departureDataTime, arrivalDataTime).Build();

			Assert.True(voyage.Number.Equals(voyageNumber));
			Assert.True(voyage.Schedule.Equals(schedule));
		}

		[Fact]
		public void build_throws_exception_if_called_without_valid_addMovement()
		{
			Assert.Throws<ArgumentException>(
				   delegate
				   {
					   var voyageNumber = new VoyageNumber("12");
					   var departureLocation = new BookingApi.Domain.Location.Location(new UnLocode("AZ23H"), "HAMBOURG");
					   var arrivalLocation = new BookingApi.Domain.Location.Location(new UnLocode("XE44K"), "TUNIS");
					   var departureDataTime = new DateTime(2010, 4, 10);
					   var arrivalDataTime = new DateTime(2010, 5, 15);
					   var schedule =
						   new Schedule(new List<CarrierMovement>
								 {
									 new CarrierMovement(departureLocation, arrivalLocation, departureDataTime,
														 arrivalDataTime)
								 });

					   BookingApi.Domain.Voyage.Voyage voyage = new VoyageBuilder(voyageNumber, departureLocation).Build();

					   voyage.Number.Equals(voyageNumber);
					   voyage.Schedule.Equals(schedule);
				   }
			   );
		}

		[Fact]
		public void build_is_idepotent()
		{
			var voyageNumber = new VoyageNumber("12");
			var departureLocation = new BookingApi.Domain.Location.Location(new UnLocode("AZ23H"), "HAMBOURG");
			var arrivalLocation = new BookingApi.Domain.Location.Location(new UnLocode("XE44K"), "TUNIS");
			var departureDataTime = new DateTime(2010, 4, 10);
			var arrivalDataTime = new DateTime(2010, 5, 15);

			var voyageBuilder = new VoyageBuilder(voyageNumber, departureLocation).AddMovement(arrivalLocation, departureDataTime, arrivalDataTime);

			var voyage1 = voyageBuilder.Build();
			var voyage2 = voyageBuilder.Build();

			Assert.True(voyage1.Number == voyage2.Number);
			Assert.True(voyage1.Schedule == voyage2.Schedule);
		}
	}
}