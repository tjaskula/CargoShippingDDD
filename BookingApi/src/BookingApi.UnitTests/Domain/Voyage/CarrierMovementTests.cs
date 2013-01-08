using System;
using BookingApi.Domain.Voyage;
using Xunit;

namespace BookingApi.UnitTests.Domain.Voyage
{
	public class CarrierMovementTests
	{
		[Fact]
		public void should_not_accept_null_departure_location()
		{
			Assert.Throws<ArgumentNullException>(
													delegate
														{
															new CarrierMovement(null,
															                    new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "CHICAGO"),
															                    DateTime.Now, DateTime.Now);
														}
												);
		}

		[Fact]
		public void should_not_accept_null_arrival_location()
		{
			Assert.Throws<ArgumentNullException>(
													delegate
													{
														new CarrierMovement(new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "CHICAGO"),
																			null,
																			DateTime.Now, DateTime.Now);
													}
												);
		}

		[Fact]
		public void should_not_accept_null_departure_time()
		{
			Assert.Throws<ArgumentException>(
													delegate
													{
														new CarrierMovement(new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "CHICAGO"),
																			new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "HAMBOURG"),
																			new DateTime(), DateTime.Now);
													}
												);
		}

		[Fact]
		public void should_not_accept_null_arrival_time()
		{
			Assert.Throws<ArgumentException>(
													delegate
													{
														new CarrierMovement(new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "CHICAGO"),
																			new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "HAMBOURG"),
																			DateTime.Now, new DateTime());
													}
												);
		}

		[Fact]
		public void should_create_instance_with_legal_parameters()
		{
			var sut = new CarrierMovement(new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "CHICAGO"),
			                              new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "HAMBOURG"),
			                              DateTime.Now, DateTime.Now);

			Assert.NotNull(sut);
		}

		[Fact]
		public void should_equal()
		{
			var chicago = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "CHICAGO");
			var hambourg = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "HAMBOURG");
			var departureTime = DateTime.Now;
			var arrivalTime = DateTime.Now;

			var cm1 = new CarrierMovement(chicago, hambourg, departureTime, arrivalTime);
			var cm2 = new CarrierMovement(chicago, hambourg, departureTime, arrivalTime);
			var cm3 = new CarrierMovement(hambourg, chicago, departureTime, arrivalTime);
			var cm4 = new CarrierMovement(hambourg, chicago, departureTime, arrivalTime);

			Assert.True(cm1.Equals(cm2));
			Assert.False(cm2.Equals(cm3));
			Assert.True(cm3.Equals(cm4));

			Assert.True(cm1== cm2);
			Assert.False(cm2 == cm3);
			Assert.True(cm3 == cm4);

			Assert.True(cm1.GetHashCode() == cm2.GetHashCode());
			Assert.False(cm2.GetHashCode() == cm3.GetHashCode());
			Assert.True(cm3.GetHashCode() == cm4.GetHashCode());
		}
	}
}