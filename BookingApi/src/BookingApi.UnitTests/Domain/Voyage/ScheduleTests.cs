using System;
using System.Collections.Generic;
using BookingApi.Domain.Location;
using BookingApi.Domain.Voyage;
using Xunit;

namespace BookingApi.UnitTests.Domain.Voyage
{
	public class ScheduleTests
	{
		[Fact]
		public void should_throw_exception_with_null_collection()
		{
			Assert.Throws<ArgumentNullException>(
					delegate
					{
						new Schedule(null);
					}
				);
		}

		[Fact]
		public void should_throw_exception_with_collection_containing_null_values()
		{
			Assert.Throws<ArgumentException>(
					delegate
					{
						var carrierMovements = new List<CarrierMovement>();
						var deparureUnLocode = new UnLocode("AB44D");
						var departureLocation = new BookingApi.Domain.Location.Location(deparureUnLocode, "HAMBOURG");
						var arrivalUnLocode = new UnLocode("XX44D");
						var arrivalLocation = new BookingApi.Domain.Location.Location(arrivalUnLocode, "TUNIS");
						var departureDate = new DateTime(2010, 3, 15);
						var arrivalDate = new DateTime(2010, 5, 12);
						var carrierMovement1 = new CarrierMovement(departureLocation, arrivalLocation, departureDate,
																				  arrivalDate);
						CarrierMovement carrierMovement2 = null;
						carrierMovements.Add(carrierMovement1);
						carrierMovements.Add(carrierMovement2);

						new Schedule(carrierMovements);
					}
				);
		}

		[Fact]
		public void should_throw_exception_with_empty_collection()
		{
			Assert.Throws<ArgumentException>(
					delegate
					{
						var carrierMovements = new List<CarrierMovement>();

						new Schedule(carrierMovements);
					}
				);
		}

		[Fact]
		public void should_create_instance_with_correct_ctor_parameters()
		{
			// AUTOFIXTURE
			//var fixture = new Fixture();

			//fixture.Register(UnLocodeHelpers.GetNewUnLocode);

			//fixture.Register<IList<CarrierMovement>>(() => fixture.CreateMany<CarrierMovement>(50).ToList());

			////fixture.Register(() => fixture.CreateMany<CarrierMovement>());

			//var schedule = fixture.CreateAnonymous<Schedule>();

			//Assert.NotNull(schedule);

			var carrierMovements = new List<CarrierMovement>();
			
			var deparureUnLocode1 = new UnLocode("AB44D");
			var departureLocation1 = new BookingApi.Domain.Location.Location(deparureUnLocode1, "HAMBOURG");
			var arrivalUnLocode1 = new UnLocode("XX44D");
			var arrivalLocation1 = new BookingApi.Domain.Location.Location(arrivalUnLocode1, "TUNIS");
			var departureDate1 = new DateTime(2010, 3, 15);
			var arrivalDate1 = new DateTime(2010, 5, 12);

			var carrierMovement1 = new CarrierMovement(departureLocation1, arrivalLocation1, departureDate1, arrivalDate1);

			var deparureUnLocode2 = new UnLocode("CXRET");
			var departureLocation2 = new BookingApi.Domain.Location.Location(deparureUnLocode2, "GDANSK");
			var arrivalUnLocode2 = new UnLocode("ZEZD4");
			var arrivalLocation2 = new BookingApi.Domain.Location.Location(arrivalUnLocode2, "LE HAVRE");
			var departureDate2 = new DateTime(2010, 3, 18);
			var arrivalDate2 = new DateTime(2010, 3, 31);

			var carrierMovement2 = new CarrierMovement(departureLocation2, arrivalLocation2, departureDate2, arrivalDate2);

			carrierMovements.Add(carrierMovement1);
			carrierMovements.Add(carrierMovement2);

			Assert.NotNull(new Schedule(carrierMovements));
		}
	}
}