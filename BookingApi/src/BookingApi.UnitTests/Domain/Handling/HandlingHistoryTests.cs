using System;
using System.Collections.Generic;
using System.Linq;
using BookingApi.Domain.Cargo;
using BookingApi.Domain.Handling;
using BookingApi.Domain.Services;
using BookingApi.Domain.Voyage;
using Xunit;

namespace BookingApi.UnitTests.Domain.Handling
{
	public class HandlingHistoryTests
	{
		private readonly BookingApi.Domain.Cargo.Cargo _cargo;
		private readonly BookingApi.Domain.Voyage.Voyage _voyage;
		private readonly HandlingEvent _handlingEvent1;
		private readonly HandlingEvent _handlingEvent1Duplicate;
		private readonly HandlingEvent _handlingEvent2;
		private readonly HandlingHistory _handlingHistory;

		public HandlingHistoryTests()
		{
			var shanghai = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "SHANGHAI");
			var dallas = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "DALLAS");
			var hongkong = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "HONGKONG");

			_cargo = new BookingApi.Domain.Cargo.Cargo(new TrackingId("ABC"), new RouteSpecification(shanghai, dallas, new DateTime(2009, 4, 1)));
			_voyage = new VoyageBuilder(new VoyageNumber("X25"), hongkong)
																		.AddMovement(shanghai, DateTime.Now, DateTime.Now)
																		.AddMovement(dallas, DateTime.Now, DateTime.Now)
																		.Build();

			_handlingEvent1 = new HandlingEvent(HandlingEventType.Load, shanghai, new DateTime(100), new DateTime(2009, 3, 5), _cargo, _voyage);
			_handlingEvent1Duplicate = new HandlingEvent(HandlingEventType.Load, shanghai, new DateTime(200), new DateTime(2009, 3, 5), _cargo, _voyage);
			_handlingEvent2 = new HandlingEvent(HandlingEventType.Unload, dallas, new DateTime(150), new DateTime(2009, 3, 10), _cargo, _voyage);

			_handlingHistory = new HandlingHistory(new List<HandlingEvent> {_handlingEvent2, _handlingEvent1, _handlingEvent1Duplicate});
		}

		[Fact]
		public void should_throw_if_null_list_of_handling_events_is_passed()
		{
			Assert.Throws<ArgumentNullException>(delegate
													{
														new HandlingHistory(null);
													}
											);
		}

		[Fact]
		public void should_handle_distinct_events_by_completion_time()
		{
			IEnumerable<HandlingEvent> expectedHandlingEvent = new List<HandlingEvent> {_handlingEvent1, _handlingEvent2};
			Assert.True(Enumerable.SequenceEqual(expectedHandlingEvent, _handlingHistory.DistinctEventsByCompletionTime));
		}

		[Fact]
		public void should_handle_most_recent_completed_event()
		{
			Assert.Equal(_handlingEvent2, _handlingHistory.MostRecentlyCompletedEvent);
		}
	}
}