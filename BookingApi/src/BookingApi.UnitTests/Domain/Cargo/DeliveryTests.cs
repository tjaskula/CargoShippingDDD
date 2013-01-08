using System;
using System.Collections.Generic;
using BookingApi.Domain.Cargo;
using BookingApi.Domain.Handling;
using BookingApi.Domain.Voyage;
using Xunit;

namespace BookingApi.UnitTests.Domain.Cargo
{
	public class DeliveryTests
	{
		private readonly BookingApi.Domain.Location.Location _chicago;
		private readonly BookingApi.Domain.Location.Location _hambourg;
		private readonly BookingApi.Domain.Location.Location _gdansk;
		private readonly Itinerary _itineraryCHtoHAM;
		private readonly Itinerary _itineraryHAMtoCH;
		private readonly Itinerary _itineraryCHtoGD;
		private readonly BookingApi.Domain.Cargo.Cargo _cargo;
		private readonly BookingApi.Domain.Voyage.Voyage _voyage;
		private readonly BookingApi.Domain.Voyage.Voyage _voyage2;

		public DeliveryTests()
		{
			_chicago = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "CHICAGO");
			_hambourg = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "HAMBOURG");
			_gdansk = new BookingApi.Domain.Location.Location(UnLocodeHelpers.GetNewUnLocode(), "GDANSK");

			_voyage = new BookingApi.Domain.Voyage.Voyage(new VoyageNumber("CM01"), new Schedule(new List<CarrierMovement>
			                                                                                     	{
			                                                                                     		new CarrierMovement(_chicago,
			                                                                                     		                    _hambourg,
			                                                                                     		                    DateTime.
			                                                                                     		                    	Now,
			                                                                                     		                    DateTime.
			                                                                                     		                    	Now)
			                                                                                     	}));

			_voyage2 = new BookingApi.Domain.Voyage.Voyage(new VoyageNumber("CM03"), new Schedule(new List<CarrierMovement>
			                                                                                     	{
			                                                                                     		new CarrierMovement(_chicago,
			                                                                                     		                    _hambourg,
			                                                                                     		                    DateTime.
			                                                                                     		                    	Now,
			                                                                                     		                    DateTime.
			                                                                                     		                    	Now),
																										new CarrierMovement(_hambourg,
			                                                                                     		                    _gdansk,
			                                                                                     		                    DateTime.
			                                                                                     		                    	Now,
			                                                                                     		                    DateTime.
			                                                                                     		                    	Now)
			                                                                                     	}));

			_itineraryCHtoHAM = new Itinerary(new List<Leg>
			                           					{
			                           						new Leg(_voyage, 
																		_chicago, 
																		DateTime.Now, 
																		_hambourg, 
																		DateTime.Now)
			                           					});

			_itineraryCHtoGD = new Itinerary(new List<Leg>
			                           					{
			                           						new Leg(_voyage2, 
																		_chicago, 
																		DateTime.Now, 
																		_hambourg, 
																		DateTime.Now),
															new Leg(_voyage2, 
																		_hambourg, 
																		DateTime.Now, 
																		_gdansk, 
																		DateTime.Now)
			                           					});

			_itineraryHAMtoCH = new Itinerary(new List<Leg>
			                           	{
			                           		new Leg(new BookingApi.Domain.Voyage.Voyage(new VoyageNumber("CM02"), new Schedule(new List<CarrierMovement>
			                           			                                                 									{
			                           			                                                 										new CarrierMovement(_hambourg,
																																							_chicago,
			                           			                                                 															DateTime.Now,
			                           			                                                 															DateTime.Now)
			                           			                                                 									})),
																																	_hambourg, 
																																	DateTime.Now,
																																	_chicago, 
																																	DateTime.Now)
			                           	});

			_cargo = new BookingApi.Domain.Cargo.Cargo(new TrackingId("CAR01"), new RouteSpecification(_chicago, _hambourg, DateTime.Now));
		}

		[Fact]
		public void should_throw_for_null_route_specification_when_derivedFrom()
		{
			Assert.Throws<ArgumentNullException>(delegate
													{
														Delivery.DerivedFrom(
															null,
																_itineraryCHtoHAM, null);
													}
											);
		}

		[Fact]
		public void should_throw_for_null_handlingHistory_when_derivedFrom()
		{
			Assert.Throws<ArgumentNullException>(delegate
													{
														Delivery.DerivedFrom(
															new RouteSpecification(
																_chicago,
																_hambourg, DateTime.Now),
																null, null);
													}
											);
		}

		[Fact]
		public void should_not_misdirect_cargo_with_no_handling_events()
		{
			var snapshot = Delivery.DerivedFrom(new RouteSpecification(_chicago, _hambourg, DateTime.Now), _itineraryCHtoHAM, new HandlingHistory(new List<HandlingEvent>()));
			Assert.False(snapshot.IsMisdirected);
		}

		[Fact]
		public void should_not_misdirect_cargo_with_no_itinerary()
		{
			var snapshot = Delivery.DerivedFrom(new RouteSpecification(_chicago, _hambourg, DateTime.Now), null, new HandlingHistory(new List<HandlingEvent>
			                                                                                                                         	{
			                                                                                                                         		new HandlingEvent(HandlingEventType.Receive, 
																																				_chicago, 
																																				DateTime.Now, 
																																				DateTime.Now, _cargo)
			                                                                                                                         	}));
			Assert.False(snapshot.IsMisdirected);
		}

		[Fact]
		public void should_misdirect_cargo_if_in_location_not_from_itinerary()
		{
			var snapshot = Delivery.DerivedFrom(new RouteSpecification(_chicago, _hambourg, DateTime.Now), _itineraryCHtoHAM, new HandlingHistory(new List<HandlingEvent>
			                                                                                                                         	{
			                                                                                                                         		new HandlingEvent(HandlingEventType.Receive, 
																																				_hambourg, 
																																				DateTime.Now, 
																																				DateTime.Now, 
																																				_cargo)
			                                                                                                                         	}));
			Assert.True(snapshot.IsMisdirected);
		}

		[Fact]
		public void should_set_notrouted_status_if_no_itinerary_supplied()
		{
			var snapshot = Delivery.DerivedFrom(new RouteSpecification(_chicago, _hambourg, DateTime.Now), null, new HandlingHistory(new List<HandlingEvent>
			                                                                                                                         	{
			                                                                                                                         		new HandlingEvent(HandlingEventType.Receive, 
																																				_hambourg, 
																																				DateTime.Now, 
																																				DateTime.Now, 
																																				_cargo)
			                                                                                                                         	}));
			Assert.Equal(RoutingStatus.NotRouted, snapshot.RoutingStatus);
		}

		[Fact]
		public void should_set_misrouted_status_if_specification_not_satisfied_by_itinerary()
		{
			var snapshot = Delivery.DerivedFrom(new RouteSpecification(_chicago, _hambourg, DateTime.Now), _itineraryHAMtoCH, new HandlingHistory(new List<HandlingEvent>
			                                                                                                                         	{
			                                                                                                                         		new HandlingEvent(HandlingEventType.Receive, 
																																				_hambourg, 
																																				DateTime.Now, 
																																				DateTime.Now, 
																																				_cargo)
			                                                                                                                         	}));
			Assert.Equal(RoutingStatus.Misrouted, snapshot.RoutingStatus);
		}

		[Fact]
		public void should_set_routed_status_if_specification_is_satisfied_by_itinerary()
		{
			var snapshot = Delivery.DerivedFrom(new RouteSpecification(_chicago, _hambourg, DateTime.Now.AddDays(10)), _itineraryCHtoHAM, new HandlingHistory(new List<HandlingEvent>
			                                                                                                                         	{
			                                                                                                                         		new HandlingEvent(HandlingEventType.Receive, 
																																				_hambourg, 
																																				DateTime.Now, 
																																				DateTime.Now, 
																																				_cargo)
			                                                                                                                         	}));
			Assert.Equal(RoutingStatus.Routed, snapshot.RoutingStatus);
		}

		[Fact]
		public void should_set_transport_status_onboardcarrier()
		{
			var snapshot = Delivery.DerivedFrom(new RouteSpecification(_chicago, _hambourg, DateTime.Now), _itineraryCHtoHAM, new HandlingHistory(new List<HandlingEvent>
			                                                                                                                         	{
			                                                                                                                         		new HandlingEvent(HandlingEventType.Load, 
																																				_chicago, 
																																				DateTime.Now, 
																																				DateTime.Now, 
																																				_cargo,
																																				_voyage)
			                                                                                                                         	}));
			Assert.Equal(TransportStatus.OnboardCarrier, snapshot.TransportStatus);
		}

		[Fact]
		public void should_set_transport_status_inport()
		{
			var snapshot = Delivery.DerivedFrom(new RouteSpecification(_chicago, _hambourg, DateTime.Now), _itineraryCHtoHAM, new HandlingHistory(new List<HandlingEvent>
			                                                                                                                         	{
			                                                                                                                         		new HandlingEvent(HandlingEventType.Receive, 
																																				_chicago, 
																																				DateTime.Now, 
																																				DateTime.Now, 
																																				_cargo)
			                                                                                                                         	}));
			Assert.Equal(TransportStatus.InPort, snapshot.TransportStatus);

			var snapshot2 = Delivery.DerivedFrom(new RouteSpecification(_chicago, _hambourg, DateTime.Now), _itineraryCHtoHAM, new HandlingHistory(new List<HandlingEvent>
			                                                                                                                         	{
			                                                                                                                         		new HandlingEvent(HandlingEventType.Unload, 
																																				_chicago, 
																																				DateTime.Now, 
																																				DateTime.Now, 
																																				_cargo,
																																				_voyage)
			                                                                                                                         	}));
			Assert.Equal(TransportStatus.InPort, snapshot2.TransportStatus);

			var snapshot3 = Delivery.DerivedFrom(new RouteSpecification(_chicago, _hambourg, DateTime.Now), _itineraryCHtoHAM, new HandlingHistory(new List<HandlingEvent>
			                                                                                                                         	{
			                                                                                                                         		new HandlingEvent(HandlingEventType.Customs, 
																																				_chicago, 
																																				DateTime.Now, 
																																				DateTime.Now, 
																																				_cargo)
			                                                                                                                         	}));
			Assert.Equal(TransportStatus.InPort, snapshot3.TransportStatus);
		}

		[Fact]
		public void should_set_transport_status_claimed()
		{
			var snapshot = Delivery.DerivedFrom(new RouteSpecification(_chicago, _hambourg, DateTime.Now), _itineraryCHtoHAM, new HandlingHistory(new List<HandlingEvent>
			                                                                                                                         	{
			                                                                                                                         		new HandlingEvent(HandlingEventType.Claim, 
																																				_chicago, 
																																				DateTime.Now, 
																																				DateTime.Now, 
																																				_cargo)
			                                                                                                                         	}));
			Assert.Equal(TransportStatus.Claimed, snapshot.TransportStatus);
		}

		[Fact]
		public void should_get_last_known_location()
		{
			var snapshot = Delivery.DerivedFrom(new RouteSpecification(_chicago, _hambourg, DateTime.Now), _itineraryCHtoHAM, new HandlingHistory(new List<HandlingEvent>
			                                                                                                                         	{
			                                                                                                                         		new HandlingEvent(HandlingEventType.Load, 
																																				_chicago, 
																																				DateTime.Now, 
																																				DateTime.Now, 
																																				_cargo,
																																				_voyage),
																																				new HandlingEvent(HandlingEventType.Unload, 
																																				_hambourg, 
																																				DateTime.Now, 
																																				DateTime.Now, 
																																				_cargo,
																																				_voyage)
			                                                                                                                         	}));
			Assert.Equal(_hambourg, snapshot.LastKnownLocation);

			var snapshot2 = Delivery.DerivedFrom(new RouteSpecification(_chicago, _hambourg, DateTime.Now), _itineraryCHtoHAM, new HandlingHistory(new List<HandlingEvent>()));
			Assert.Equal(BookingApi.Domain.Location.Location.Unknown, snapshot2.LastKnownLocation);
		}

		[Fact]
		public void should_get_the_current_voyage()
		{
			var snapshot = Delivery.DerivedFrom(new RouteSpecification(_chicago, _hambourg, DateTime.Now), _itineraryCHtoHAM, new HandlingHistory(new List<HandlingEvent>
			                                                                                                                         	{
			                                                                                                                         		new HandlingEvent(HandlingEventType.Load, 
																																				_chicago, 
																																				DateTime.Now, 
																																				DateTime.Now, 
																																				_cargo,
																																				_voyage)
			                                                                                                                         	}));
			Assert.Equal(_voyage, snapshot.CurrentVoyage);

			var snapshot2 = Delivery.DerivedFrom(new RouteSpecification(_chicago, _hambourg, DateTime.Now), _itineraryCHtoHAM, new HandlingHistory(new List<HandlingEvent>()));
			Assert.Equal(BookingApi.Domain.Voyage.Voyage.Empty, snapshot2.CurrentVoyage);
		}

		[Fact]
		public void should_calculate_eta_if_on_track()
		{
			var snapshot = Delivery.DerivedFrom(new RouteSpecification(_chicago, _hambourg, DateTime.Now.AddDays(10)), _itineraryCHtoHAM, new HandlingHistory(new List<HandlingEvent>
			                                                                                                                         	{
			                                                                                                                         		new HandlingEvent(HandlingEventType.Load, 
																																				_chicago, 
																																				DateTime.Now, 
																																				DateTime.Now, 
																																				_cargo,
																																				_voyage)
			                                                                                                                         	}));
			Assert.Equal(_itineraryCHtoHAM.FinalArrivalDate, snapshot.EstimatedTimeOfArrival);

			var snapshot2 = Delivery.DerivedFrom(new RouteSpecification(_chicago, _hambourg, DateTime.Now), _itineraryCHtoHAM, new HandlingHistory(new List<HandlingEvent>
			                                                                                                                         	{
			                                                                                                                         		new HandlingEvent(HandlingEventType.Load, 
																																				_hambourg, 
																																				DateTime.Now, 
																																				DateTime.Now, 
																																				_cargo,
																																				_voyage)
			                                                                                                                         	}));
			Assert.Null(snapshot2.EstimatedTimeOfArrival);

			var snapshot3 = Delivery.DerivedFrom(new RouteSpecification(_chicago, _hambourg, DateTime.Now), _itineraryHAMtoCH, new HandlingHistory(new List<HandlingEvent>
			                                                                                                                         	{
			                                                                                                                         		new HandlingEvent(HandlingEventType.Load, 
																																				_chicago, 
																																				DateTime.Now, 
																																				DateTime.Now, 
																																				_cargo,
																																				_voyage)
			                                                                                                                         	}));
			Assert.Null(snapshot3.EstimatedTimeOfArrival);
		}

		[Fact]
		public void should_return_null_for_next_expected_activity_if_not_on_track()
		{
			var snapshot = Delivery.DerivedFrom(new RouteSpecification(_chicago, _hambourg, DateTime.Now), _itineraryHAMtoCH, new HandlingHistory(new List<HandlingEvent>
			                                                                                                                         	{
			                                                                                                                         		new HandlingEvent(HandlingEventType.Load, 
																																				_chicago, 
																																				DateTime.Now, 
																																				DateTime.Now, 
																																				_cargo,
																																				_voyage)
			                                                                                                                         	}));
			Assert.Null(snapshot.NextExpectedActivity);
		}

		[Fact]
		public void should_return_received_handling_activity_if_no_last_event()
		{
			var snapshot = Delivery.DerivedFrom(new RouteSpecification(_chicago, _hambourg, DateTime.Now.AddDays(10)), _itineraryCHtoHAM, new HandlingHistory(new List<HandlingEvent>()));
			Assert.Equal(new HandlingActivity(HandlingEventType.Receive, _chicago), snapshot.NextExpectedActivity);
		}

		[Fact]
		public void should_return_unload_handling_activity_for_load_last_activity()
		{
			var snapshot = Delivery.DerivedFrom(new RouteSpecification(_chicago, _hambourg, DateTime.Now.AddDays(10)), _itineraryCHtoHAM, new HandlingHistory(new List<HandlingEvent>{
			                                                                                                                         		new HandlingEvent(HandlingEventType.Load, 
																																				_chicago, 
																																				DateTime.Now, 
																																				DateTime.Now, 
																																				_cargo,
																																				_voyage)
			                                                                                                                         	}));
			Assert.Equal(new HandlingActivity(HandlingEventType.Unload, _hambourg, _voyage), snapshot.NextExpectedActivity);

			var snapshot2 = Delivery.DerivedFrom(new RouteSpecification(_chicago, _hambourg, DateTime.Now), _itineraryCHtoHAM, new HandlingHistory(new List<HandlingEvent>{
			                                                                                                                         		new HandlingEvent(HandlingEventType.Load, 
																																				_hambourg, 
																																				DateTime.Now, 
																																				DateTime.Now, 
																																				_cargo,
																																				_voyage)
			                                                                                                                         	}));
			Assert.Null(snapshot2.NextExpectedActivity);
		}

		[Fact]
		public void should_return_null_handling_activity_for_unload_event_if_leg_unloadlocation_not_matches_event_location()
		{
			var snapshot = Delivery.DerivedFrom(new RouteSpecification(_chicago, _hambourg, DateTime.Now), _itineraryCHtoHAM, new HandlingHistory(new List<HandlingEvent>{
			                                                                                                                         		new HandlingEvent(HandlingEventType.Unload, 
																																				_chicago, 
																																				DateTime.Now, 
																																				DateTime.Now, 
																																				_cargo,
																																				_voyage)
			                                                                                                                         	}));
			Assert.Null(snapshot.NextExpectedActivity);
		}

		[Fact]
		public void should_return_claim_handling_activity_for_unload_event_if_leg_unloadlocation_matches_event_location_and_no_more_legs()
		{
			var snapshot = Delivery.DerivedFrom(new RouteSpecification(_chicago, _hambourg, DateTime.Now.AddDays(10)), _itineraryCHtoHAM, new HandlingHistory(new List<HandlingEvent>{
			                                                                                                                         		new HandlingEvent(HandlingEventType.Unload, 
																																				_hambourg, 
																																				DateTime.Now, 
																																				DateTime.Now, 
																																				_cargo,
																																				_voyage)
			                                                                                                                         	}));
			Assert.Equal(new HandlingActivity(HandlingEventType.Claim, _hambourg), snapshot.NextExpectedActivity);
		}

		[Fact]
		public void should_return_load_handling_activity_for_unload_event_if_leg_unloadlocation_matches_event_location_and_more_legs()
		{
			var snapshot = Delivery.DerivedFrom(new RouteSpecification(_chicago, _gdansk, DateTime.Now.AddDays(10)), _itineraryCHtoGD, new HandlingHistory(new List<HandlingEvent>{
			                                                                                                                         		new HandlingEvent(HandlingEventType.Unload, 
																																				_hambourg, 
																																				DateTime.Now, 
																																				DateTime.Now, 
																																				_cargo,
																																				_voyage2)
			                                                                                                                         	}));
			Assert.Equal(new HandlingActivity(HandlingEventType.Load, _hambourg), snapshot.NextExpectedActivity);
		}

		[Fact]
		public void should_return_load_handling_activity_for_received_event_()
		{
			var snapshot = Delivery.DerivedFrom(new RouteSpecification(_chicago, _gdansk, DateTime.Now.AddDays(10)), _itineraryCHtoGD, new HandlingHistory(new List<HandlingEvent>{
			                                                                                                                         		new HandlingEvent(HandlingEventType.Receive, 
																																				_chicago, 
																																				DateTime.Now, 
																																				DateTime.Now, 
																																				_cargo)
			                                                                                                                         	}));
			Assert.Equal(new HandlingActivity(HandlingEventType.Load, _chicago, _voyage2), snapshot.NextExpectedActivity);
		}

		[Fact]
		public void should_be_unloaded_at_destination()
		{
			var snapshot = Delivery.DerivedFrom(new RouteSpecification(_chicago, _gdansk, DateTime.Now), _itineraryCHtoGD, new HandlingHistory(new List<HandlingEvent>{
			                                                                                                                         		new HandlingEvent(HandlingEventType.Unload, 
																																				_gdansk, 
																																				DateTime.Now, 
																																				DateTime.Now, 
																																				_cargo,
																																				_voyage2)
			                                                                                                                         	}));
			Assert.True(snapshot.IsUnloadedAtDestination);

			var snapshot2 = Delivery.DerivedFrom(new RouteSpecification(_chicago, _gdansk, DateTime.Now), _itineraryCHtoGD, new HandlingHistory(new List<HandlingEvent>{
			                                                                                                                         		new HandlingEvent(HandlingEventType.Unload, 
																																				_hambourg, 
																																				DateTime.Now, 
																																				DateTime.Now, 
																																				_cargo,
																																				_voyage2)
			                                                                                                                         	}));
			Assert.False(snapshot2.IsUnloadedAtDestination);

			var snapshot3 = Delivery.DerivedFrom(new RouteSpecification(_chicago, _gdansk, DateTime.Now), _itineraryCHtoGD, new HandlingHistory(new List<HandlingEvent>{
			                                                                                                                         		new HandlingEvent(HandlingEventType.Load, 
																																				_gdansk, 
																																				DateTime.Now, 
																																				DateTime.Now, 
																																				_cargo,
																																				_voyage2)
			                                                                                                                         	}));
			Assert.False(snapshot3.IsUnloadedAtDestination);

			var snapshot4 = Delivery.DerivedFrom(new RouteSpecification(_chicago, _gdansk, DateTime.Now), _itineraryCHtoGD, new HandlingHistory(new List<HandlingEvent>()));
			Assert.False(snapshot4.IsUnloadedAtDestination);
		}

		[Fact]
		public void should_return_null_for_claim_and_customs_handling_event()
		{
			var snapshot = Delivery.DerivedFrom(new RouteSpecification(_chicago, _gdansk, DateTime.Now), _itineraryCHtoGD, new HandlingHistory(new List<HandlingEvent>{
			                                                                                                                         		new HandlingEvent(HandlingEventType.Claim, 
																																				_gdansk, 
																																				DateTime.Now, 
																																				DateTime.Now, 
																																				_cargo)
			                                                                                                                         	}));
			Assert.Null(snapshot.NextExpectedActivity);

			var snapshot2 = Delivery.DerivedFrom(new RouteSpecification(_chicago, _gdansk, DateTime.Now), _itineraryCHtoGD, new HandlingHistory(new List<HandlingEvent>{
			                                                                                                                         		new HandlingEvent(HandlingEventType.Customs, 
																																				_gdansk, 
																																				DateTime.Now, 
																																				DateTime.Now, 
																																				_cargo)
			                                                                                                                         	}));
			Assert.Null(snapshot2.NextExpectedActivity);
		}

		[Fact]
		public void should_throw_for_null_route_specification_when_updating_on_routing()
		{
			Assert.Throws<ArgumentNullException>(delegate
			                                     	{
			                                     		Delivery.DerivedFrom(
			                                     			new RouteSpecification(
			                                     				_chicago,
			                                     				_hambourg, DateTime.Now), 
																_itineraryCHtoHAM, null)
			                                     			.UpdateOnRouting(null, null);
			                                     	}
											);
		}

		[Fact]
		public void should_create_delivery_snapshot_when_updating_on_routing()
		{
			var snapshot = Delivery.DerivedFrom(new RouteSpecification(
													_chicago,
													_hambourg, DateTime.Now),
													_itineraryCHtoHAM, new HandlingHistory(new List<HandlingEvent>()))
												.UpdateOnRouting(new RouteSpecification(_hambourg, _chicago, DateTime.Now), _itineraryHAMtoCH);

			Assert.Equal(TransportStatus.NotReceived, snapshot.TransportStatus);
		}
	}
}