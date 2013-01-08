using System;
using System.Collections.Generic;
using BookingApi.Domain.Cargo;
using BookingApi.Domain.Handling;
using BookingApi.Domain.Services;
using BookingApi.Domain.Voyage;
using Xunit;

namespace BookingApi.UnitTests.Domain.Cargo
{
	public class ItineraryTests
	{
		private readonly BookingApi.Domain.Location.Location _shanghai = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "SHANGHAI");
		private readonly BookingApi.Domain.Location.Location _rotterdam = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "ROTTERDAM");
		private readonly BookingApi.Domain.Location.Location _gothenburg = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "GOTHENBURG");
		private readonly BookingApi.Domain.Location.Location _newyork = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "NEW YORK");
		private readonly BookingApi.Domain.Location.Location _helsinki = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "HELSINKI");

		private readonly BookingApi.Domain.Voyage.Voyage _voyage;
		private readonly BookingApi.Domain.Voyage.Voyage _wrongVoyage;
		private readonly TrackingId _trackingId;
		private readonly RouteSpecification _routeSpecification;
		private readonly BookingApi.Domain.Cargo.Cargo _cargo;
		private readonly Itinerary _itinerary;

		public ItineraryTests()
		{
			_voyage = new VoyageBuilder(new VoyageNumber("0123"), _shanghai)
										.AddMovement(_rotterdam, DateTime.Now, DateTime.Now)
										.AddMovement(_gothenburg, DateTime.Now, DateTime.Now)
										.Build();
			_wrongVoyage = new VoyageBuilder(new VoyageNumber("666"), _newyork)
										.AddMovement(new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "STOCKHOLM"), DateTime.Now, DateTime.Now)
										.AddMovement(_helsinki, DateTime.Now, DateTime.Now)
										.Build();
			_trackingId = new TrackingId("CARGO1");
			_routeSpecification = new RouteSpecification(_shanghai, _gothenburg, DateTime.Now);
			_cargo = new BookingApi.Domain.Cargo.Cargo(_trackingId, _routeSpecification);
			_itinerary = new Itinerary(new List<Leg>
			                           	{
			                           		new Leg(_voyage, _shanghai, DateTime.Now, _rotterdam, DateTime.Now),
											new Leg(_voyage, _rotterdam, DateTime.Now, _gothenburg, DateTime.Now)
			                           	});
		}

		[Fact]
		public void should_throw_an_exception_when_legs_are_empty()
		{
			Assert.Throws<ArgumentException>(
													delegate
													{
														new Itinerary(new List<Leg>());
													}
												);
		}

		[Fact]
		public void should_throw_an_exception_when_legs_are_null()
		{
			Assert.Throws<ArgumentNullException>(
													delegate
													{
														new Itinerary(null);
													}
												);
		}

		[Fact]
		public void should_pass_the_happy_path()
		{
			var handlingEvent = new HandlingEvent(HandlingEventType.Receive, _shanghai, DateTime.Now, DateTime.Now, _cargo);
			Assert.True(_itinerary.IsExpected(handlingEvent));

			handlingEvent = new HandlingEvent(HandlingEventType.Load, _shanghai, DateTime.Now, DateTime.Now, _cargo, _voyage);
			Assert.True(_itinerary.IsExpected(handlingEvent));

			handlingEvent = new HandlingEvent(HandlingEventType.Unload, _rotterdam, DateTime.Now, DateTime.Now, _cargo, _voyage);
			Assert.True(_itinerary.IsExpected(handlingEvent));

			handlingEvent = new HandlingEvent(HandlingEventType.Load, _rotterdam, DateTime.Now, DateTime.Now, _cargo, _voyage);
			Assert.True(_itinerary.IsExpected(handlingEvent));

			handlingEvent = new HandlingEvent(HandlingEventType.Unload, _gothenburg, DateTime.Now, DateTime.Now, _cargo, _voyage);
			Assert.True(_itinerary.IsExpected(handlingEvent));

			handlingEvent = new HandlingEvent(HandlingEventType.Claim, _gothenburg, DateTime.Now, DateTime.Now, _cargo);
			Assert.True(_itinerary.IsExpected(handlingEvent));
		}

		[Fact]
		public void should_change_nothing_custom_event()
		{
			var handlingEvent = new HandlingEvent(HandlingEventType.Customs, _gothenburg, DateTime.Now, DateTime.Now, _cargo);
			Assert.True(_itinerary.IsExpected(handlingEvent));
		}

		[Fact]
		public void should_handle_receive_at_wrong_location()
		{
			var handlingEvent = new HandlingEvent(HandlingEventType.Receive, new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "HANGZOU"), DateTime.Now, DateTime.Now, _cargo);
			Assert.False(_itinerary.IsExpected(handlingEvent));
		}

		[Fact]
		public void should_handle_when_loaded_onto_the_wrong_ship_correct_location()
		{
			var handlingEvent = new HandlingEvent(HandlingEventType.Load, _rotterdam, DateTime.Now, DateTime.Now, _cargo, _wrongVoyage);
			Assert.False(_itinerary.IsExpected(handlingEvent));
		}

		[Fact]
		public void should_handle_unloaded_from_the_wrong_ship_wrong_location()
		{
			var handlingEvent = new HandlingEvent(HandlingEventType.Unload, _helsinki, DateTime.Now, DateTime.Now, _cargo, _wrongVoyage);
			Assert.False(_itinerary.IsExpected(handlingEvent));

			handlingEvent = new HandlingEvent(HandlingEventType.Claim, _rotterdam, DateTime.Now, DateTime.Now, _cargo);
			Assert.False(_itinerary.IsExpected(handlingEvent));
		}
	}
}