﻿using System;
using System.Collections.Generic;
using System.Linq;
using BookingApi.Domain.Handling;

namespace BookingApi.Domain.Cargo
{
#pragma warning disable 660,661
	/// <summary>
	/// Description of delivery status.
	/// </summary>
	public class Delivery : ValueObject //Equals and GetHashCode are overridden in ValueObject class.
#pragma warning restore 660,661
	{
		private readonly TransportStatus _transportStatus;
		private readonly Location.Location _lastKnownLocation;
		private readonly Voyage.Voyage _currentVoyage;
		private readonly bool _misdirected;
		private readonly DateTime? _eta;
		private readonly HandlingActivity _nextExpectedActivity;
		private readonly bool _isUnloadedAtDestination;
		private readonly RoutingStatus _routingStatus;
		private readonly DateTime _calculatedAt;
		private readonly HandlingEvent _lastEvent;


		/// <summary>
		/// Gets next expected activity.
		/// </summary>
		public HandlingActivity NextExpectedActivity
		{
			get { return _nextExpectedActivity; }
		}

		/// <summary>
		/// Gets status of cargo routing.
		/// </summary>
		public RoutingStatus RoutingStatus
		{
			get { return _routingStatus; }
		}

		/// <summary>
		/// Gets time when this delivery status was calculated.
		/// </summary>
		public DateTime CalculatedAt
		{
			get { return _calculatedAt; }
		}

		/// <summary>
		/// Gets if this cargo has been unloaded at its destination.
		/// </summary>
		public bool IsUnloadedAtDestination
		{
			get { return _isUnloadedAtDestination; }
		}

		/// <summary>
		/// Gets estimated time of arrival. Returns null if information cannot be obtained (cargo is misrouted).
		/// </summary>
		public DateTime? EstimatedTimeOfArrival
		{
			get { return _eta; }
		}

		/// <summary>
		/// Gets last known location of this cargo or Location.Unknown if the delivery history is empty.
		/// </summary>
		public Location.Location LastKnownLocation
		{
			get { return _lastKnownLocation ?? Location.Location.Unknown; }
		}

		/// <summary>
		/// Gets the current voyage.
		/// </summary>
		public Voyage.Voyage CurrentVoyage
		{
			get { return _currentVoyage ?? Voyage.Voyage.Empty; }
		}

		/// <summary>
		/// Gets status of cargo transport.
		/// </summary>
		public TransportStatus TransportStatus
		{
			get { return _transportStatus; }
		}

		/// <summary>
		/// Gets if this cargo was misdirected.
		/// - A cargo is misdirected if it is in a location that's not in the itinerary.
		/// - A cargo with no itinerary can not be misdirected.
		/// - A cargo that has received no handling events can not be misdirected.
		/// </summary>
		public bool IsMisdirected
		{
			get { return _misdirected; }
		}

		/// <summary>
		/// Creates a new delivery snapshot based on the complete handling history of a cargo, as well 
		/// as its route specification and itinerary.
		/// </summary>
		/// <param name="specification">Current route specification.</param>
		/// <param name="itinerary">Current itinerary.</param>
		/// <param name="handlingHistory">Delivery history.</param>
		/// <returns>Delivery status description.</returns>
		public static Delivery DerivedFrom(RouteSpecification specification, Itinerary itinerary, HandlingHistory handlingHistory)
		{
			if (specification == null)
				throw new ArgumentNullException("specification", "Route specification is required");
			if (handlingHistory == null)
				throw new ArgumentNullException("handlingHistory", "Handling history is required");

			var lastHandlingEvent = handlingHistory.MostRecentlyCompletedEvent;
			return new Delivery(lastHandlingEvent, itinerary, specification);
		}

		/// <summary>
		/// Creates a new delivery snapshot to reflect changes in routing, i.e. when the route 
		/// specification or the itinerary has changed but no additional handling of the 
		/// cargo has been performed.
		/// </summary>
		/// <param name="routeSpecification">Current route specification.</param>
		/// <param name="itinerary">Current itinerary.</param>
		/// <returns>New delivery status description.</returns>
		public Delivery UpdateOnRouting(RouteSpecification routeSpecification, Itinerary itinerary)
		{
			if (routeSpecification == null)
				throw new ArgumentNullException("Route specification is required.", "routeSpecification");

			return new Delivery(_lastEvent, itinerary, routeSpecification);
		}

		private Delivery(HandlingEvent lastHandlingEvent, Itinerary itinerary, RouteSpecification specification)
		{
			_calculatedAt = DateTime.Now;
			_lastEvent = lastHandlingEvent;

			_misdirected = CalculateMisdirectionStatus(itinerary);
			_routingStatus = CalculateRoutingStatus(itinerary, specification);
			_transportStatus = CalculateTransportStatus();
			_lastKnownLocation = CalculateLastKnownLocation();
			_currentVoyage = CalculateCurrentVoyage();
			_eta = CalculateEta(itinerary);
			_nextExpectedActivity = CalculateNextExpectedActivity(specification, itinerary);
			_isUnloadedAtDestination = CalculateUnloadedAtDestination(specification);
		}

		private Voyage.Voyage CalculateCurrentVoyage()
		{
			if (_transportStatus == TransportStatus.OnboardCarrier && _lastEvent != null)
				return _lastEvent.Voyage;

			return null;
		}

		private bool CalculateUnloadedAtDestination(RouteSpecification specification)
		{
			return LastEvent != null &&
					 LastEvent.EventType == HandlingEventType.Unload &&
					 specification.Destination == LastEvent.Location;
		}

		private DateTime? CalculateEta(Itinerary itinerary)
		{
			return OnTrack ? itinerary.FinalArrivalDate : null;
		}

		private Location.Location CalculateLastKnownLocation()
		{
			return LastEvent != null ? LastEvent.Location : null;
		}

		private TransportStatus CalculateTransportStatus()
		{
			if (LastEvent == null)
				return TransportStatus.NotReceived;

			switch (LastEvent.EventType)
			{
				case HandlingEventType.Load:
					return TransportStatus.OnboardCarrier;
				case HandlingEventType.Unload:
				case HandlingEventType.Receive:
				case HandlingEventType.Customs:
					return TransportStatus.InPort;
				case HandlingEventType.Claim:
					return TransportStatus.Claimed;
				default:
					return TransportStatus.Unknown;
			}
		}

		private HandlingActivity CalculateNextExpectedActivity(RouteSpecification routeSpecification, Itinerary itinerary)
		{
			if (!OnTrack)
				return null;

			if (LastEvent == null)
				return new HandlingActivity(HandlingEventType.Receive, routeSpecification.Origin);

			switch (LastEvent.EventType)
			{
				case HandlingEventType.Load:

					Leg firstOrDefaultLeg = itinerary.Legs.FirstOrDefault(x => x.LoadLocation == LastEvent.Location);
					return firstOrDefaultLeg != null ? new HandlingActivity(HandlingEventType.Unload, firstOrDefaultLeg.UnloadLocation, firstOrDefaultLeg.Voyage) : null;

				case HandlingEventType.Unload:
					IEnumerator<Leg> enumerator = itinerary.Legs.GetEnumerator();
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.UnloadLocation == LastEvent.Location)
						{
							Leg currentLeg = enumerator.Current;
							return enumerator.MoveNext() ? new HandlingActivity(HandlingEventType.Load, enumerator.Current.LoadLocation) : new HandlingActivity(HandlingEventType.Claim, currentLeg.UnloadLocation);
						}
					}
					return null;

				case HandlingEventType.Receive:
					Leg firstLeg = itinerary.Legs.First();
					return new HandlingActivity(HandlingEventType.Load, firstLeg.LoadLocation, firstLeg.Voyage);
				default:
					return null;
			}
		}

		private static RoutingStatus CalculateRoutingStatus(Itinerary itinerary, RouteSpecification specification)
		{
			if (itinerary == null)
				return RoutingStatus.NotRouted;
			
			return specification.IsSatisfiedBy(itinerary) ? RoutingStatus.Routed : RoutingStatus.Misrouted;
		}

		private bool CalculateMisdirectionStatus(Itinerary itinerary)
		{
			if (_lastEvent == null)
				return false;

			if (itinerary == null)
				return false;
			
			return !itinerary.IsExpected(_lastEvent);
		}

		private bool OnTrack
		{
			get { return _routingStatus == RoutingStatus.Routed && !IsMisdirected; }
		}

		private HandlingEvent LastEvent
		{
			get { return _lastEvent; }
		}

		#region Infrastructure
		protected Delivery()
		{
		}

		protected override IEnumerable<object> GetAtomicValues()
		{
			yield return _calculatedAt;
			yield return _eta;
			yield return _lastEvent;
			yield return _isUnloadedAtDestination;
			yield return _isUnloadedAtDestination;
			yield return _lastKnownLocation;
			yield return _misdirected;
			yield return _routingStatus;
			yield return _transportStatus;
			yield return _currentVoyage;
		}

		/// <summary>
		/// Compares two <see cref="Delivery"/>s for equality.
		/// </summary>
		/// <param name="left">First <see cref="Delivery"/>.</param>
		/// <param name="right">Other <see cref="Delivery"/> to compare.</param>
		/// <returns>Returns <c>true</c> if both <see cref="Delivery"/>s are equal.</returns>
		public static bool operator ==(Delivery left, Delivery right)
		{
			return EqualOperator(left, right);
		}

		/// <summary>
		/// Compares two <see cref="Delivery"/>s for inequality.
		/// </summary>
		/// <param name="left">First <see cref="Delivery"/>.</param>
		/// <param name="right">Other <see cref="Delivery"/> to compare.</param>
		/// <returns>Returns <c>true</c> if both <see cref="Delivery"/>s are not equal.</returns>
		public static bool operator !=(Delivery left, Delivery right)
		{
			return NotEqualOperator(left, right);
		}
		#endregion
	}
}