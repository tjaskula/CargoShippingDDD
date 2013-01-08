using System;
using System.Collections.Generic;
using BookingApi.Domain.Cargo;
using BookingApi.Domain.Location;
using BookingApi.Domain.Voyage;
using Xunit;

namespace BookingApi.UnitTests.Domain.Cargo
{
	public class LegTests
	{
		[Fact]
		public void should_throw_an_exception_when_voyage_is_null()
		{
			Assert.Throws<ArgumentNullException>(
													delegate
													{
														new Leg(null, 
																new BookingApi.Domain.Location.Location(new UnLocode("ABCDE"), "test-name"), 
																DateTime.Now, 
																new BookingApi.Domain.Location.Location(new UnLocode("ERTYU"), "test-name2"), 
																DateTime.Now );
													}
												);
		}

		[Fact]
		public void should_throw_an_exception_when_load_location_is_null()
		{
			Assert.Throws<ArgumentNullException>(
													delegate
													{
														new Leg(new BookingApi.Domain.Voyage.Voyage(new VoyageNumber("1235"), new Schedule(new List<CarrierMovement> { new CarrierMovement(new BookingApi.Domain.Location.Location(new UnLocode("ABCDE"), "start"), new BookingApi.Domain.Location.Location(new UnLocode("BDERT"), "stop"), DateTime.Now, DateTime.Now)})), 
															null, 
															DateTime.Now, 
															new BookingApi.Domain.Location.Location(new UnLocode("ERTYU"), "test-name2"), 
															DateTime.Now);
													}
												);
		}

		[Fact]
		public void should_throw_an_exception_when_unload_location_is_null()
		{
			Assert.Throws<ArgumentNullException>(
													delegate
													{
														new Leg(new BookingApi.Domain.Voyage.Voyage(new VoyageNumber("1235"), new Schedule(new List<CarrierMovement> { new CarrierMovement(new BookingApi.Domain.Location.Location(new UnLocode("ABCDE"), "start"), new BookingApi.Domain.Location.Location(new UnLocode("BDERT"), "stop"), DateTime.Now, DateTime.Now) })),
															new BookingApi.Domain.Location.Location(new UnLocode("ABCDE"), "test-name"), 
															DateTime.Now, 
															null, 
															DateTime.Now);
													}
												);
		}

		[Fact]
		public void should_throw_an_exception_when_load_date_is_null()
		{
			Assert.Throws<ArgumentException>(
													delegate
													{
														new Leg(new BookingApi.Domain.Voyage.Voyage(new VoyageNumber("1235"), new Schedule(new List<CarrierMovement> { new CarrierMovement(new BookingApi.Domain.Location.Location(new UnLocode("ABCDE"), "start"), new BookingApi.Domain.Location.Location(new UnLocode("BDERT"), "stop"), DateTime.Now, DateTime.Now) })),
															new BookingApi.Domain.Location.Location(new UnLocode("ABCDE"), "test-name"),
															new DateTime(), 
															new BookingApi.Domain.Location.Location(new UnLocode("ERTYU"), "test-name2"),
															DateTime.Now);
													}
												);
		}

		[Fact]
		public void should_throw_an_exception_when_unload_date_is_null()
		{
			Assert.Throws<ArgumentException>(
													delegate
													{
														new Leg(new BookingApi.Domain.Voyage.Voyage(new VoyageNumber("1235"), new Schedule(new List<CarrierMovement> { new CarrierMovement(new BookingApi.Domain.Location.Location(new UnLocode("ABCDE"), "start"), new BookingApi.Domain.Location.Location(new UnLocode("BDERT"), "stop"), DateTime.Now, DateTime.Now) })),
															new BookingApi.Domain.Location.Location(new UnLocode("ABCDE"), "test-name"),
															DateTime.Now,
															new BookingApi.Domain.Location.Location(new UnLocode("ERTYU"), "test-name2"),
															new DateTime());
													}
												);
		}

		[Fact]
		public void should_equal_the_same_legs()
		{
			var loadTime = DateTime.Now;
			var unloadTime = DateTime.Now;
			var voyage = new BookingApi.Domain.Voyage.Voyage(new VoyageNumber("1235"),
			                                                 new Schedule(new List<CarrierMovement>
			                                                              	{
			                                                              		new CarrierMovement(new BookingApi.Domain.Location.Location(new UnLocode("ABCDE"), "start"), new BookingApi.Domain.Location.Location(new UnLocode("BDERT"), "stop"), loadTime, unloadTime)
			                                                              	}));
			var leg = new Leg(voyage,
															new BookingApi.Domain.Location.Location(new UnLocode("ABCDE"), "test-name"),
															loadTime,
															new BookingApi.Domain.Location.Location(new UnLocode("ERTYU"), "test-name2"),
															unloadTime);
			var leg2 = new Leg(voyage,
															new BookingApi.Domain.Location.Location(new UnLocode("ABCDE"), "test-name"),
															loadTime,
															new BookingApi.Domain.Location.Location(new UnLocode("ERTYU"), "test-name2"),
															unloadTime);

			Assert.True(leg.Equals(leg2));
		}
	}
}