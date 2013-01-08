using System;
using System.Collections.Generic;

namespace BookingApi.Domain.Cargo
{
#pragma warning disable 660,661
	/// <summary>
	/// Contains information about a route: its origin, destination and arrival deadline.
	/// </summary>
	public class RouteSpecification : ValueObject //Equals and GetHashCode are overridden in ValueObject class.
#pragma warning restore 660,661
	{
		private readonly Location.Location _origin;
		private readonly Location.Location _destination;
		private readonly DateTime _arrivalDeadline;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="origin">Origin location - can't be the same as the destination.</param>
		/// <param name="destination">Destination location - can't be the same as the origin</param>
		/// <param name="arrivalDeadline">Arrival deadline.</param>
		public RouteSpecification(Location.Location origin, Location.Location destination, DateTime arrivalDeadline)
		{
			if (origin == null)
				throw new ArgumentNullException("origin");

			if (destination == null)
				throw new ArgumentNullException("destination");

			if (origin == destination)
				throw new ArgumentException("Origin and destination can't be the same.");

			if (arrivalDeadline == default(DateTime))
				throw new ArgumentException("Arrival deadline is required.", "arrivalDeadline");

			_origin = origin;
			_arrivalDeadline = arrivalDeadline;
			_destination = destination;
		}

		/// <summary>
		/// Checks whether provided itinerary (a description of transporting steps) satisfies this
		/// specification.
		/// </summary>
		/// <param name="itinerary">An itinerary.</param>
		/// <returns>True, if cargo can be transported from <see cref="Origin"/> to <see cref="Destination"/>
		/// before <see cref="ArrivalDeadline"/> using provided itinerary.
		/// </returns>
		public virtual bool IsSatisfiedBy(Itinerary itinerary)
		{
			return itinerary != null &&
				   _origin == itinerary.InitialDepartureLocation &&
				   _destination == itinerary.FinalArrivalLocation &&
				   _arrivalDeadline > itinerary.FinalArrivalDate;
		}

		/// <summary>
		/// Date of expected cargo arrival.
		/// </summary>
		public DateTime ArrivalDeadline
		{
			get { return _arrivalDeadline; }
		}

		/// <summary>
		/// Location where cargo should be delivered.
		/// </summary>
		public Location.Location Destination
		{
			get { return _destination; }
		}

		/// <summary>
		/// Location where cargo should be picked up.
		/// </summary>
		public Location.Location Origin
		{
			get { return _origin; }
		}

		/// <summary>
		/// Equality operator between <see cref="RouteSpecification"/>s objects.
		/// </summary>
		/// <param name="left">Frist <see cref="RouteSpecification"/> to which the right <see cref="RouteSpecification"/> will be compared.</param>
		/// <param name="right">Second <see cref="RouteSpecification"/> to which the left <see cref="RouteSpecification"/> will be compared.</param>
		/// <returns>Returns <c>true</c> whenever both <see cref="RouteSpecification"/> are equal.</returns>
		public static bool operator ==(RouteSpecification left, RouteSpecification right)
		{
			return EqualOperator(left, right);
		}

		/// <summary>
		/// Inequality operator between <see cref="RouteSpecification"/>s objects.
		/// </summary>
		/// <param name="left">Frist <see cref="RouteSpecification"/> to which the right <see cref="RouteSpecification"/> will be compared.</param>
		/// <param name="right">Second <see cref="RouteSpecification"/> to which the left <see cref="RouteSpecification"/> will be compared.</param>
		/// <returns>Returns <c>true</c> whenever both <see cref="RouteSpecification"/> are not equal.</returns>
		public static bool operator !=(RouteSpecification left, RouteSpecification right)
		{
			return NotEqualOperator(left, right);
		}

		protected override IEnumerable<object> GetAtomicValues()
		{
			yield return _origin;
			yield return _destination;
			yield return _arrivalDeadline;
		}

		/// <summary>
		/// For NHibernate.
		/// </summary>
		protected RouteSpecification()
		{
		}
	}
}