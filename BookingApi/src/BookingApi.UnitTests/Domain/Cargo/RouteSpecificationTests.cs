using System;
using System.Collections.Generic;
using BookingApi.Domain.Cargo;
using BookingApi.Domain.Services;
using BookingApi.Domain.Voyage;
using Xunit;

namespace BookingApi.UnitTests.Domain.Cargo
{
	public class RouteSpecificationTests : IUseFixture<RouteSpecificationFixture>
	{
		private BookingApi.Domain.Voyage.Voyage _hongKongTokyoNewYork;
		private BookingApi.Domain.Voyage.Voyage _dallasNewYorkChicago;
		private Itinerary _itinerary;

		private readonly BookingApi.Domain.Location.Location _hongKongLocation = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "HONGKONG");
		private readonly BookingApi.Domain.Location.Location _newYorkLocation = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "NEWYORK");
		private readonly BookingApi.Domain.Location.Location _chicagoLocation = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "CHICAGO");

		public void SetFixture(RouteSpecificationFixture data)
		{
			_hongKongTokyoNewYork = data.InitializeBuilder(new VoyageNumber("V001"), _hongKongLocation)
										.AddMovement(new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "TOKYO"), new DateTime(2009, 2, 1), new DateTime(2009, 2, 5))
										.AddMovement(_newYorkLocation, new DateTime(2009, 2, 6), new DateTime(2009, 2, 10))
										.AddMovement(_hongKongLocation, new DateTime(2009, 2, 11), new DateTime(2009, 2, 14)).Build();

			_dallasNewYorkChicago = data.InitializeBuilder(new VoyageNumber("V002"), new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "DALLAS"))
										.AddMovement(_newYorkLocation, new DateTime(2009, 2, 6), new DateTime(2009, 2, 7))
										.AddMovement(_chicagoLocation, new DateTime(2009, 2, 12), new DateTime(2009, 2, 20)).Build();

			// TODO: it shouldn't be possible to create Legs that have load/unload locations and/or dates that don't match the voyage's carrier movements.
			_itinerary = data.CreateItinerary(new [] {new Leg(_hongKongTokyoNewYork, _hongKongLocation, new DateTime(2009, 2, 1), _newYorkLocation, new DateTime(2009, 2, 10)), 
													  new Leg(_dallasNewYorkChicago, _newYorkLocation, new DateTime(2009, 2, 12), _chicagoLocation, new DateTime(2009, 2, 20))});
		}

		[Fact]
		public void should_throw_exception_with_null_origin()
		{
			Assert.Throws<ArgumentNullException>(
												delegate
												{
													new RouteSpecification(null, new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "test"), DateTime.Now);
												}
											);
		}

		[Fact]
		public void should_throw_exception_with_null_destination()
		{
			Assert.Throws<ArgumentNullException>(
												delegate
												{
													new RouteSpecification(new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "test"), null, DateTime.Now);
												}
											);
		}

		[Fact]
		public void should_throw_exception_with_null_arrivalDeadline()
		{
			Assert.Throws<ArgumentException>(
												delegate
												{
													new RouteSpecification(new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "test"),
														new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "test"), new DateTime());
												}
											);
		}

		[Fact]
		public void should_throw_exception_if_origine_equals_destination()
		{
			var code = UnLocodeHelpers.GetNewUnLocode();
			Assert.Throws<ArgumentException>(
												delegate
												{
													new RouteSpecification(new BookingApi.Domain.Location.Location(code, "test"),
														new BookingApi.Domain.Location.Location(code, "test"), DateTime.Now);
												}
											);
		}

		[Fact]
		public void routeSpecification_should_be_satisfied_by_correct_route()
		{
			var routeSpecification = new RouteSpecification(_hongKongLocation, _chicagoLocation, new DateTime(2009, 3, 1));

			Assert.True(routeSpecification.IsSatisfiedBy(_itinerary));
		}

		[Fact]
		public void routeSpecification_should_not_be_satisfied_by_wrong_origin()
		{
			var hangzouLocation = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "HANGZOU");
			var routeSpecification = new RouteSpecification(hangzouLocation, _chicagoLocation, new DateTime(2009, 3, 1));

			Assert.False(routeSpecification.IsSatisfiedBy(_itinerary));
		}

		[Fact]
		public void routeSpecification_should_not_be_satisfied_by_wrong_destination()
		{
			var dallasLocation = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "DALLAS");
			var routeSpecification = new RouteSpecification(_hongKongLocation, dallasLocation, new DateTime(2009, 3, 1));

			Assert.False(routeSpecification.IsSatisfiedBy(_itinerary));
		}

		[Fact]
		public void routeSpecification_should_not_be_satisfied_by_missed_deadline()
		{
			var routeSpecification = new RouteSpecification(_hongKongLocation, _chicagoLocation, new DateTime(2009, 2, 15));

			Assert.False(routeSpecification.IsSatisfiedBy(_itinerary));
		}
	}

	public class RouteSpecificationFixture
	{
		public VoyageBuilder InitializeBuilder(VoyageNumber voyageNumber, BookingApi.Domain.Location.Location departureLocation)
		{
			return new VoyageBuilder(voyageNumber, departureLocation);
		}

		public Itinerary CreateItinerary(IEnumerable<Leg> legs)
		{
			return new Itinerary(legs);
		}
	}
}