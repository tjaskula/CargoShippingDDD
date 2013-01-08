/**
 * A Cargo. This is the central class in the domain model,
 * and it is the root of the Cargo-Itinerary-Leg-Delivery-RouteSpecification aggregate.
 *
 * A cargo is identified by a unique tracking id, and it always has an origin
 * and a route specification. The life cycle of a cargo begins with the booking procedure,
 * when the tracking id is assigned. During a (short) period of time, between booking
 * and initial routing, the cargo has no itinerary.
 *
 * The booking clerk requests a list of possible routes, matching the route specification,
 * and assigns the cargo to one route. The route to which a cargo is assigned is described
 * by an itinerary.
 *
 * A cargo can be re-routed during transport, on demand of the customer, in which case
 * a new route is specified for the cargo and a new route is requested. The old itinerary,
 * being a value object, is discarded and a new one is attached.
 *
 * It may also happen that a cargo is accidentally misrouted, which should notify the proper
 * personnel and also trigger a re-routing procedure.
 *
 * When a cargo is handled, the status of the delivery changes. Everything about the delivery
 * of the cargo is contained in the Delivery value object, which is replaced whenever a cargo
 * is handled by an asynchronous event triggered by the registration of the handling event.
 *
 * The delivery can also be affected by routing changes, i.e. when a the route specification
 * changes, or the cargo is assigned to a new route. In that case, the delivery update is performed
 * synchronously within the cargo aggregate.
 *
 * The life cycle of a cargo ends when the cargo is claimed by the customer.
 *
 * The cargo aggregate, and the entre domain model, is built to solve the problem
 * of booking and tracking cargo. All important business rules for determining whether
 * or not a cargo is misdirected, what the current status of the cargo is (on board carrier,
 * in port etc), are captured in this aggregate.
 *
 */

using System;
using BookingApi.Domain.Cargo.Events;
using BookingApi.Domain.Handling;

namespace BookingApi.Domain.Cargo
{
	/// <summary>
	/// Cargo.
	/// </summary>
	public class Cargo
	{
		/// <summary>
		/// Gets the tracking id of this cargo.
		/// The tracking id is the identity of this entity, and is unique.
		/// </summary>
		public virtual TrackingId TrackingId { get; protected set; }

		/// <summary>
		/// Gets the origine location of this cargo.
		/// </summary>
		public virtual Location.Location Origine { get; protected set; } // c'est ewpose par la route specificétion (en double)

		/// <summary>
		/// Gets the route specification of this cargo.
		/// </summary>
		public virtual RouteSpecification RouteSpecification { get; protected set; }

		/// <summary>
		/// Gets the itinerary of this cargo. Never null.
		/// </summary>
		public virtual Itinerary Itinerary { get; protected set; }

		/// <summary>
		/// Gets delivery status of this cargo. Never null.
		/// </summary>
		public virtual Delivery Delivery { get; protected set; }

		/// <summary>
		/// Creates new <see cref="Cargo"/> object with provided tracking id and route specification.
		/// </summary>
		/// <param name="trackingId">Tracking id of this cargo.</param>
		/// <param name="routeSpecification">Route specification.</param>
		public Cargo(TrackingId trackingId, RouteSpecification routeSpecification)
		{
			if (trackingId == null)
				throw new ArgumentNullException("trackingId");

			if (routeSpecification == null)
				throw new ArgumentNullException("routeSpecification");

			TrackingId = trackingId;
			// Cargo origin never changes, even if the route specification changes.
			// However, at creation, cargo orgin can be derived from the initial route specification.
			Origine = routeSpecification.Origin;
			RouteSpecification = routeSpecification;
			Delivery = Delivery.DerivedFrom(RouteSpecification, Itinerary, HandlingHistory.EmptyHistory);
		}

		/// <summary>
		/// Specifies a new route for this cargo.
		/// </summary>
		/// <param name="routeSpecification">Route specification.</param>
		public virtual void SpecifyNewRoute(RouteSpecification routeSpecification)
		{
			if (routeSpecification == null)
				throw new ArgumentNullException("routeSpecification", "Route specification is required");

			RouteSpecification = routeSpecification;
			// Handling consistency within the Cargo aggregate synchronously
			Delivery = Delivery.UpdateOnRouting(RouteSpecification, Itinerary);
		}

		/// <summary>
		/// Assigns cargo to a provided route.
		/// Attach a new itinerary to this cargo.
		/// </summary>
		/// <param name="itinerary">New itinerary</param>
		public virtual void AssignToRoute(Itinerary itinerary)
		{
			if (itinerary == null)
				throw new ArgumentNullException("itinerary", "Itinerary is required for assignment");

			CargoHasBeenAssignedToRouteEvent @event = new CargoHasBeenAssignedToRouteEvent(this, Itinerary);
			Itinerary = itinerary;
			// Handling consistency within the Cargo aggregate synchronously
			Delivery = Delivery.UpdateOnRouting(RouteSpecification, Itinerary);
			DomainEvents.Raise(@event);
		}

		/// <summary>
		/// Updates delivery progress information according to handling history.
		/// Updates all aspects of the cargo aggregate status
		/// based on the current route specification, itinerary and handling of the cargo.
		/// <p/>
		/// When either of those three changes, i.e. when a new route is specified for the cargo,
		/// the cargo is assigned to a route or when the cargo is handled, the status must be
		/// re-calculated.
		/// <p/>
		/// <see cref="RouteSpecification" /> and <see cref="Itinerary" /> are both inside the Cargo
		/// aggregate, so changes to them cause the status to be updated <b>synchronously</b>,
		/// but changes to the delivery history (when a cargo is handled) cause the status update
		/// to happen <b>asynchronously</b> since <see cref="HandlingEvent" /> is in a different aggregate.
		/// </summary>
		/// <param name="handlingHistory">Handling history.</param>
		public virtual void DeriveDeliveryProgress(HandlingHistory handlingHistory)
		{
			// Delivery is a value object, so we can simply discard the old one
			// and replace it with a new

			Delivery = Delivery.DerivedFrom(RouteSpecification, Itinerary, handlingHistory);
			
			if (Delivery.IsMisdirected)
			{
				DomainEvents.Raise(new CargoWasMisdirectedEvent(this));
			}
			else if (Delivery.IsUnloadedAtDestination)
			{
				DomainEvents.Raise(new CargoHasArrivedEvent(this));
			}
		}

		protected Cargo()
		{
		}

		/// <summary>
		/// Compares two <see cref="Cargo"/> objects based on its <see cref="TrackingId"/> value.
		/// </summary>
		/// <param name="obj">The second <see cref="Cargo"/> to compare.</param>
		/// <returns>Returns <c>True</c> if two <see cref="TrackingId"/> of cargos are equal.</returns>
		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != GetType())
				return false;

			var other = obj as Cargo;

			return TrackingId == other.TrackingId;
		}

		public override int GetHashCode()
		{
			return TrackingId.GetHashCode();
		}

		public override string ToString()
		{
			return TrackingId.ToString();
		}
	}
}