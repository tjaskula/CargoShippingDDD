using System;
using System.Collections.Generic;
using BookingApi.Domain.Cargo;
using BookingApi.Domain.Handling;
using BookingApi.Domain.Services;
using BookingApi.Domain.Voyage;
using Xunit;

namespace BookingApi.UnitTests.Domain.Cargo
{
	public class CargoTests
	{
		private BookingApi.Domain.Voyage.Voyage _voyage;
		private readonly BookingApi.Domain.Location.Location _stockholm;
		private readonly BookingApi.Domain.Location.Location _hambourg;
		private readonly BookingApi.Domain.Location.Location _hongkong;
		private readonly BookingApi.Domain.Location.Location _melbourne;

		public CargoTests()
		{
			_stockholm = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "STOCKHOLM");
			_hambourg = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "HAMBOURG");
			_hongkong = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "HONGKONG");
			_melbourne = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "MELBOURNE");

			_voyage = new VoyageBuilder(new VoyageNumber("0123"), _stockholm)
				.AddMovement(_hambourg, DateTime.Now, DateTime.Now)
				.AddMovement(_hongkong, DateTime.Now, DateTime.Now)
				.AddMovement(_melbourne, DateTime.Now, DateTime.Now)
				.Build();
		}

		[Fact]
		public void should_construct_correctly_cargo()
		{
			var trackingId = new TrackingId("XYZ");
			var arrivalDeadline = new DateTime(2009, 3, 13);
			var routeSpecification = new RouteSpecification(_stockholm, _melbourne, arrivalDeadline);

			var cargo = new BookingApi.Domain.Cargo.Cargo(trackingId, routeSpecification);

			Assert.Equal(RoutingStatus.NotRouted, cargo.Delivery.RoutingStatus);
			Assert.Equal(TransportStatus.NotReceived, cargo.Delivery.TransportStatus);
			Assert.Equal(BookingApi.Domain.Location.Location.Unknown, cargo.Delivery.LastKnownLocation);
			Assert.Equal(BookingApi.Domain.Voyage.Voyage.Empty, cargo.Delivery.CurrentVoyage);
		}

		[Fact]
		public void should_correctly_route_cargo()
		{
			var cargo = new BookingApi.Domain.Cargo.Cargo(new TrackingId("XYZ"), new RouteSpecification(_stockholm, _melbourne, DateTime.Now));
			var good = new Itinerary(new List<Leg> { new Leg(BookingApi.Domain.Voyage.Voyage.Empty, _stockholm, DateTime.Now, _melbourne, DateTime.Now) }); // modif refactoring
			var bad = new Itinerary(new List<Leg> { new Leg(BookingApi.Domain.Voyage.Voyage.Empty, _hongkong, DateTime.Now, _hambourg, DateTime.Now) }); // modif refactoring
			var acceptOnlyGood = new FakeRouteSpecification(cargo.Origine, cargo.RouteSpecification.Destination, DateTime.Now, good);
			
			cargo.SpecifyNewRoute(acceptOnlyGood);
			Assert.Equal(RoutingStatus.NotRouted, cargo.Delivery.RoutingStatus);

			cargo.AssignToRoute(bad);
			Assert.Equal(RoutingStatus.Misrouted, cargo.Delivery.RoutingStatus);

			cargo.AssignToRoute(good);
			Assert.Equal(RoutingStatus.Routed, cargo.Delivery.RoutingStatus);
		}

		[Fact]
		public void should_set_unknown_location_as_last_known_location_when_no_events()
		{
			var cargo = new BookingApi.Domain.Cargo.Cargo(new TrackingId("XYZ"), new RouteSpecification(_stockholm, _melbourne, DateTime.Now));

			Assert.Equal(BookingApi.Domain.Location.Location.Unknown, cargo.Delivery.LastKnownLocation);
		}

		[Fact]
		public void should_set_received_location_as_last_known_location()
		{
			var cargo = new BookingApi.Domain.Cargo.Cargo(new TrackingId("XYZ"), new RouteSpecification(_stockholm, _melbourne, DateTime.Now));

			var handlingHistory = new HandlingHistory(new List<HandlingEvent>
			                                          	{
			                                          		new HandlingEvent(HandlingEventType.Receive, _stockholm, DateTime.Now, new DateTime(2007, 12, 1), cargo)
			                                          	});
			cargo.DeriveDeliveryProgress(handlingHistory);

			Assert.Equal(_stockholm, cargo.Delivery.LastKnownLocation);
		}

		[Fact]
		public void should_set_claimed_location_as_last_known_location()
		{
			var cargo = new BookingApi.Domain.Cargo.Cargo(new TrackingId("XYZ"), new RouteSpecification(_stockholm, _melbourne, DateTime.Now));

			var handlingHistory = new HandlingHistory(new List<HandlingEvent>
			                                          	{
															new HandlingEvent(HandlingEventType.Load, _stockholm, DateTime.Now, new DateTime(2007, 12, 1), cargo, _voyage),
															new HandlingEvent(HandlingEventType.Unload, _hambourg, DateTime.Now, new DateTime(2007, 12, 2), cargo, _voyage),
															new HandlingEvent(HandlingEventType.Load, _hambourg, DateTime.Now, new DateTime(2007, 12, 3), cargo, _voyage),
															new HandlingEvent(HandlingEventType.Unload, _hongkong, DateTime.Now, new DateTime(2007, 12, 4), cargo, _voyage),
															new HandlingEvent(HandlingEventType.Load, _hongkong, DateTime.Now, new DateTime(2007, 12, 5), cargo, _voyage),
															new HandlingEvent(HandlingEventType.Unload, _melbourne, DateTime.Now, new DateTime(2007, 12, 7), cargo, _voyage),
			                                          		new HandlingEvent(HandlingEventType.Claim, _melbourne, DateTime.Now, new DateTime(2007, 12, 9), cargo)
			                                          	});
			cargo.DeriveDeliveryProgress(handlingHistory);

			Assert.Equal(_melbourne, cargo.Delivery.LastKnownLocation);
		}

		[Fact]
		public void should_set_off_location_as_last_known_location()
		{
			var cargo = new BookingApi.Domain.Cargo.Cargo(new TrackingId("XYZ"), new RouteSpecification(_stockholm, _melbourne, DateTime.Now));

			var handlingHistory = new HandlingHistory(new List<HandlingEvent>
			                                          	{
			                                          		new HandlingEvent(HandlingEventType.Load, _stockholm, DateTime.Now, new DateTime(2007, 12, 1), cargo, _voyage),
															new HandlingEvent(HandlingEventType.Unload, _hambourg, DateTime.Now, new DateTime(2007, 12, 2), cargo, _voyage),
															new HandlingEvent(HandlingEventType.Load, _hambourg, DateTime.Now, new DateTime(2007, 12, 3), cargo, _voyage),
															new HandlingEvent(HandlingEventType.Unload, _hongkong, DateTime.Now, new DateTime(2007, 12, 4), cargo, _voyage)
			                                          	});

			cargo.DeriveDeliveryProgress(handlingHistory);

			Assert.Equal(_hongkong, cargo.Delivery.LastKnownLocation);
		}

		[Fact]
		public void should_set_on_location_as_last_known_location()
		{
			var cargo = new BookingApi.Domain.Cargo.Cargo(new TrackingId("XYZ"), new RouteSpecification(_stockholm, _melbourne, DateTime.Now));

			var handlingHistory = new HandlingHistory(new List<HandlingEvent>
			                                          	{
			                                          		new HandlingEvent(HandlingEventType.Load, _stockholm, DateTime.Now, new DateTime(2007, 12, 1), cargo, _voyage),
															new HandlingEvent(HandlingEventType.Unload, _hambourg, DateTime.Now, new DateTime(2007, 12, 2), cargo, _voyage),
															new HandlingEvent(HandlingEventType.Load, _hambourg, DateTime.Now, new DateTime(2007, 12, 3), cargo, _voyage)
			                                          	});

			cargo.DeriveDeliveryProgress(handlingHistory);

			Assert.Equal(_hambourg, cargo.Delivery.LastKnownLocation);
		}

		[Fact]
		public void should_pass_equality()
		{
			var spec1 = new RouteSpecification(_stockholm, _hongkong, DateTime.Now);
			var spec2 = new RouteSpecification(_stockholm, _melbourne, DateTime.Now);

			var cargo1 = new BookingApi.Domain.Cargo.Cargo(new TrackingId("ABC"), spec1);
			var cargo2 = new BookingApi.Domain.Cargo.Cargo(new TrackingId("CBA"), spec1);
			var cargo3 = new BookingApi.Domain.Cargo.Cargo(new TrackingId("ABC"), spec2);
			var cargo4 = new BookingApi.Domain.Cargo.Cargo(new TrackingId("ABC"), spec1);

			Assert.True(cargo1.Equals(cargo4));
			Assert.True(cargo1.Equals(cargo3));
			Assert.True(cargo3.Equals(cargo4));
			Assert.False(cargo1.Equals(cargo2));
		}

		[Fact]
		public void should_be_unloaded_at_final_destination()
		{
			var hangzou = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "HANGZOU");
			var tokyo = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "TOKYO");
			var newyork = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "NEW YORK");

			var cargo = SetUpCargoWithItinerary(hangzou, tokyo, newyork);

			Assert.False(cargo.Delivery.IsUnloadedAtDestination);

			// Adding an event unrelated to unloading at final destination
			var events = new List<HandlingEvent>();
			events.Add(new HandlingEvent(HandlingEventType.Receive, hangzou, DateTime.Now, new DateTime(10), cargo));
			cargo.DeriveDeliveryProgress(new HandlingHistory(events));
			Assert.False(cargo.Delivery.IsUnloadedAtDestination);

			var voyage = new VoyageBuilder(new VoyageNumber("0123"), hangzou)
				.AddMovement(newyork, DateTime.Now, DateTime.Now)
				.Build();

			// Adding an unload event, but not at the final destination
			events.Add(new HandlingEvent(HandlingEventType.Unload, tokyo, DateTime.Now, new DateTime(20), cargo, voyage));
			cargo.DeriveDeliveryProgress(new HandlingHistory(events));
			Assert.False(cargo.Delivery.IsUnloadedAtDestination);

			// Adding an event in the final destination, but not unload
			events.Add(new HandlingEvent(HandlingEventType.Customs, newyork, DateTime.Now, new DateTime(30), cargo));
			cargo.DeriveDeliveryProgress(new HandlingHistory(events));
			Assert.False(cargo.Delivery.IsUnloadedAtDestination);

			// Finally, cargo is unloaded at final destination
			events.Add(new HandlingEvent(HandlingEventType.Unload, newyork, DateTime.Now, new DateTime(40), cargo, voyage));
			cargo.DeriveDeliveryProgress(new HandlingHistory(events));
			Assert.True(cargo.Delivery.IsUnloadedAtDestination);
		}

		[Fact]
		public void should_test_misdirection()
		{
			var shanghai = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "SHANGHAI");
			var gothenburg = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "GOTHENBURG");
			var rotterdam = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "ROTTERDAM");
			var hangzou = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "HANGZOU");

			//A cargo with no itinerary is not misdirected
			var cargo = new BookingApi.Domain.Cargo.Cargo(new TrackingId("TRKID"), new RouteSpecification(shanghai, gothenburg, DateTime.Now));
			Assert.False(cargo.Delivery.IsMisdirected);

			cargo = SetUpCargoWithItinerary(shanghai, rotterdam, gothenburg);

			//A cargo with no handling events is not misdirected
			Assert.False(cargo.Delivery.IsMisdirected);

			var events = new List<HandlingEvent>();
			
			//Happy path
			events.Add(new HandlingEvent(HandlingEventType.Receive, shanghai, new DateTime(10), new DateTime(20), cargo));
			events.Add(new HandlingEvent(HandlingEventType.Load, shanghai, new DateTime(30), new DateTime(40), cargo, _voyage));
			events.Add(new HandlingEvent(HandlingEventType.Unload, rotterdam, new DateTime(50), new DateTime(60), cargo, _voyage));
			events.Add(new HandlingEvent(HandlingEventType.Load, rotterdam, new DateTime(70), new DateTime(80), cargo, _voyage));
			events.Add(new HandlingEvent(HandlingEventType.Unload, gothenburg, new DateTime(90), new DateTime(100), cargo, _voyage));
			events.Add(new HandlingEvent(HandlingEventType.Claim, gothenburg, new DateTime(110), new DateTime(120), cargo));
			events.Add(new HandlingEvent(HandlingEventType.Customs, gothenburg, new DateTime(130), new DateTime(140), cargo));

			cargo.DeriveDeliveryProgress(new HandlingHistory(events));
			Assert.False(cargo.Delivery.IsMisdirected);

			//Try a couple of failing ones
			cargo = SetUpCargoWithItinerary(shanghai, rotterdam, gothenburg);
			events.Add(new HandlingEvent(HandlingEventType.Receive, hangzou, DateTime.Now, DateTime.Now, cargo));

			cargo.DeriveDeliveryProgress(new HandlingHistory(events));
			Assert.True(cargo.Delivery.IsMisdirected);

			cargo = SetUpCargoWithItinerary(shanghai, rotterdam, gothenburg);
			events.Add(new HandlingEvent(HandlingEventType.Receive, shanghai, new DateTime(10), new DateTime(20), cargo));
			events.Add(new HandlingEvent(HandlingEventType.Load, shanghai, new DateTime(30), new DateTime(40), cargo, _voyage));
			events.Add(new HandlingEvent(HandlingEventType.Unload, rotterdam, new DateTime(50), new DateTime(60), cargo, _voyage));
			events.Add(new HandlingEvent(HandlingEventType.Load, rotterdam, new DateTime(70), new DateTime(80), cargo, _voyage));

			cargo.DeriveDeliveryProgress(new HandlingHistory(events));
			Assert.True(cargo.Delivery.IsMisdirected);

			cargo = SetUpCargoWithItinerary(shanghai, rotterdam, gothenburg);
			events.Add(new HandlingEvent(HandlingEventType.Receive, shanghai, new DateTime(10), new DateTime(20), cargo));
			events.Add(new HandlingEvent(HandlingEventType.Load, shanghai, new DateTime(30), new DateTime(40), cargo, _voyage));
			events.Add(new HandlingEvent(HandlingEventType.Unload, rotterdam, new DateTime(50), new DateTime(60), cargo, _voyage));
			events.Add(new HandlingEvent(HandlingEventType.Claim, rotterdam, DateTime.Now, DateTime.Now, cargo));

			cargo.DeriveDeliveryProgress(new HandlingHistory(events));
			Assert.True(cargo.Delivery.IsMisdirected);
		}

		public class FakeRouteSpecification : RouteSpecification
		{
			private readonly Itinerary _compareToThis;

			public FakeRouteSpecification(BookingApi.Domain.Location.Location origin, BookingApi.Domain.Location.Location destination, DateTime arrivalDeadline, Itinerary compareToThis)
				: base(origin, destination, arrivalDeadline)
			{
				_compareToThis = compareToThis;
			}

			public override bool IsSatisfiedBy(Itinerary itinerary)
			{
				return itinerary == _compareToThis;
			}
		}

		private BookingApi.Domain.Cargo.Cargo SetUpCargoWithItinerary(BookingApi.Domain.Location.Location origin, BookingApi.Domain.Location.Location midpoint, BookingApi.Domain.Location.Location destination)
		{
			var cargo = new BookingApi.Domain.Cargo.Cargo(new TrackingId("CARGO1"), new RouteSpecification(origin, destination, DateTime.Now));

			var itinerary = new Itinerary(new List<Leg>
			                              	{
			                              		new Leg(_voyage, origin, DateTime.Now, midpoint, DateTime.Now),
			                              		new Leg(_voyage, midpoint, DateTime.Now, destination, DateTime.Now)
			                              	});

			cargo.AssignToRoute(itinerary);

			return cargo;
		}
	}
}