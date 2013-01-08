using System;
using BookingApi.Domain.Cargo;
using BookingApi.Domain.Handling;
using BookingApi.Domain.Services;
using BookingApi.Domain.Voyage;
using Xunit;
using Xunit.Extensions;

namespace BookingApi.UnitTests.Domain.Handling
{
	public class HandlingEventTests
	{
		private readonly BookingApi.Domain.Cargo.Cargo _cargo;
		private readonly BookingApi.Domain.Location.Location _hongkong;
		private readonly BookingApi.Domain.Location.Location _helsinki;
		private readonly BookingApi.Domain.Location.Location _newYork;
		private readonly BookingApi.Domain.Location.Location _chicago;
		private readonly BookingApi.Domain.Location.Location _hambourg;
		private readonly BookingApi.Domain.Voyage.Voyage _voyage;
		private readonly BookingApi.Domain.Voyage.Voyage _voyage2;
		private readonly BookingApi.Domain.Voyage.Voyage _voyage3;

		public HandlingEventTests()
		{
			var trackingId = new TrackingId("XYZ");
			_hongkong = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "HONGKONG");
			_newYork = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "NEW YORK");
			_helsinki = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "HELSINKI");
			_chicago = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "CHICAGO");
			_hambourg = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "HAMBOURG");
			var routeSpecification = new RouteSpecification(_hongkong, _newYork, DateTime.Now);
			_cargo = new BookingApi.Domain.Cargo.Cargo(trackingId, routeSpecification);
			_voyage = new VoyageBuilder(new VoyageNumber("X25"), _hongkong)
																		.AddMovement(_newYork, DateTime.Now, DateTime.Now)
																		.Build();
			_voyage2 = new VoyageBuilder(new VoyageNumber("CM004"), _newYork)
																		.AddMovement(_chicago, DateTime.Now, DateTime.Now)
																		.Build();
			_voyage3 = new VoyageBuilder(new VoyageNumber("CM005"), _chicago)
																		.AddMovement(_hambourg, DateTime.Now, DateTime.Now)
																		.Build();
		}

		[Fact]
		public void should_throw_for_null_cargo()
		{
			Assert.Throws<ArgumentNullException>(delegate
												{
													new HandlingEvent(HandlingEventType.Load, _hongkong, DateTime.Now, DateTime.Now, null, _voyage);
												}
											);

			Assert.Throws<ArgumentNullException>(delegate
												{
													new HandlingEvent(HandlingEventType.Claim, _hongkong, DateTime.Now, DateTime.Now, null);
												}
											);
		}

		[Fact]
		public void should_throw_for_null_location()
		{
			Assert.Throws<ArgumentNullException>(delegate
												{
													new HandlingEvent(HandlingEventType.Load, null, DateTime.Now, DateTime.Now, _cargo, _voyage);
												}
											);

			Assert.Throws<ArgumentNullException>(delegate
												{
													new HandlingEvent(HandlingEventType.Claim, null, DateTime.Now, DateTime.Now, _cargo);
												}
											);
		}

		[Fact]
		public void should_throw_for_null_voyage()
		{
			Assert.Throws<ArgumentNullException>(delegate
													{
														new HandlingEvent(HandlingEventType.Load, _hongkong, DateTime.Now, DateTime.Now, _cargo, null);
													}
											);
		}

		[Fact]
		public void should_throw_for_null_registration_date()
		{
			Assert.Throws<ArgumentException>(delegate
													{
														new HandlingEvent(HandlingEventType.Load, _hongkong, new DateTime(), DateTime.Now, _cargo, _voyage);
													}
											);

			Assert.Throws<ArgumentException>(delegate
													{
														new HandlingEvent(HandlingEventType.Claim, _hongkong, new DateTime(), DateTime.Now, _cargo);
													}
											);
		}

		[Fact]
		public void should_throw_for_null_completion_date()
		{
			Assert.Throws<ArgumentException>(delegate
													{
														new HandlingEvent(HandlingEventType.Load, _hongkong, DateTime.Now, new DateTime(), _cargo, _voyage);
													}
											);

			Assert.Throws<ArgumentException>(delegate
													{
														new HandlingEvent(HandlingEventType.Claim, _hongkong, DateTime.Now, new DateTime(), _cargo);
													}
											);
		}

		[Fact]
		public void should_equal_location_with_handlingEvent_location()
		{
			var e1 = new HandlingEvent(HandlingEventType.Load, _hongkong, DateTime.Now, DateTime.Now, _cargo, _voyage);
			Assert.Equal(_hongkong, e1.Location);

			var e2 = new HandlingEvent(HandlingEventType.Unload, _newYork, DateTime.Now, DateTime.Now, _cargo, _voyage);
			Assert.Equal(_newYork, e2.Location);
		}

		[Theory]
		[InlineData(HandlingEventType.Claim)]
		[InlineData(HandlingEventType.Receive)]
		[InlineData(HandlingEventType.Customs)]
		public void should_handle_specific_event_types_to_prohibit_carrier_movement_association(HandlingEventType handlingEventType)
		{
			Assert.Throws<ArgumentException>(delegate
			                                 	{
													new HandlingEvent(handlingEventType, _hongkong, DateTime.Now, DateTime.Now, _cargo, _voyage);
			                                 	}
											);
		}

		[Theory]
		[InlineData(HandlingEventType.Load)]
		[InlineData(HandlingEventType.Unload)]
		public void should_handle_specific_event_types_to_require_carrier_movement_association(HandlingEventType handlingEventType)
		{
			Assert.Throws<ArgumentException>(delegate
												{
													new HandlingEvent(handlingEventType, _hongkong, DateTime.Now, DateTime.Now, _cargo);
												}
											);
		}

		[Fact]
		public void should_equal_location_with_handlingEvent_location_for_claim()
		{
			var e1 = new HandlingEvent(HandlingEventType.Claim, _helsinki, DateTime.Now, DateTime.Now, _cargo);
			Assert.Equal(_helsinki, e1.Location);
		}

		[Fact]
		public void should_equal_current_location_with_handlingEvent_location_for_load()
		{
			var e1 = new HandlingEvent(HandlingEventType.Load, _chicago, DateTime.Now, DateTime.Now, _cargo, _voyage2);
			Assert.Equal(_chicago, e1.Location);
		}

		[Fact]
		public void should_equal_current_location_with_handlingEvent_location_for_unload()
		{
			var e1 = new HandlingEvent(HandlingEventType.Unload, _hambourg, DateTime.Now, DateTime.Now, _cargo, _voyage2);
			Assert.Equal(_hambourg, e1.Location);
		}

		[Fact]
		public void should_equal_current_location_with_handlingEvent_location_for_received()
		{
			var e1 = new HandlingEvent(HandlingEventType.Receive, _chicago, DateTime.Now, DateTime.Now, _cargo);
			Assert.Equal(_chicago, e1.Location);
		}

		[Fact]
		public void should_equal_current_location_with_handlingEvent_location_for_claim()
		{
			var e1 = new HandlingEvent(HandlingEventType.Claim, _chicago, DateTime.Now, DateTime.Now, _cargo);
			Assert.Equal(_chicago, e1.Location);
		}

		[Fact]
		public void should_equal_events()
		{
			var timeOccured = DateTime.Now;
			var timeRegistered = DateTime.Now;

			var e1 = new HandlingEvent(HandlingEventType.Load, _chicago, timeRegistered, timeOccured, _cargo, _voyage3);
			var e2 = new HandlingEvent(HandlingEventType.Load, _chicago, timeRegistered, timeOccured, _cargo, _voyage3);

			Assert.True(e1.Equals(e2));
			Assert.True(e2.Equals(e1));

			Assert.True(e1.Equals(e1));

			Assert.False(e2.Equals(null));
			Assert.False(e2.Equals(new object()));
		}
	}
}