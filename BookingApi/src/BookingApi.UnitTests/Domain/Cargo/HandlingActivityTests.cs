using System;
using BookingApi.Domain.Cargo;
using BookingApi.Domain.Handling;
using BookingApi.Domain.Services;
using BookingApi.Domain.Voyage;
using Xunit;

namespace BookingApi.UnitTests.Domain.Cargo
{
	public class HandlingActivityTests
	{
		[Fact]
		public void should_throw_an_exception_when_location_is_null()
		{
			Assert.Throws<ArgumentNullException>(
													delegate
														{
															new HandlingActivity(HandlingEventType.Load, null);
														}
												);

			var voyage = new VoyageBuilder(new VoyageNumber("VOY01"),
			                               new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "CHICAGO"))
										   .AddMovement(new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "HAMBOURG"), DateTime.Now, DateTime.Now)
										   .Build();

			Assert.Throws<ArgumentNullException>(
													delegate
													{
														new HandlingActivity(HandlingEventType.Load, null, voyage);
													}
												);
		}

		[Fact]
		public void should_throw_an_exception_when_voyage_is_null()
		{
			Assert.Throws<ArgumentNullException>(
													delegate
														{
															new HandlingActivity(HandlingEventType.Load, new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "CHICAGO"), null);
														}
												);
		}

		[Fact]
		public void should_the_same_handling_activities()
		{
			var voyage = new VoyageBuilder(new VoyageNumber("VOY01"),
			                               new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "CHICAGO"))
										   .AddMovement(new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "HAMBOURG"), DateTime.Now, DateTime.Now)
										   .Build();
			var chicago = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "CHICAGO");

			var firstHandlingActivity = new HandlingActivity(HandlingEventType.Load, chicago, voyage);
			var secondHandlingActivity = new HandlingActivity(HandlingEventType.Load, chicago, voyage);

			Assert.Equal(firstHandlingActivity, secondHandlingActivity);
		}
	}
}