using System;
using System.Collections.Generic;
using System.Linq;
using BookingApi.Domain.Handling;

namespace BookingApi.Domain.Cargo
{
	
#pragma warning disable 660,661
	/// <summary>
	/// Specifies steps required to transport a cargo from its origin to destination.
	/// </summary>
	public class Itinerary : ValueObject //Equals and GetHashCode are overridden in ValueObject class.
#pragma warning restore 660,661
	{
		private readonly IList<Leg> _legs = new List<Leg>();

		/// <summary>
		/// Creates new <see cref="Itinerary"/> instance for provided collection of routing steps (legs).
		/// </summary>
		/// <param name="legs">Collection of routing steps (legs).</param>
		public Itinerary(IEnumerable<Leg> legs)
		{
			if (legs == null)
				throw new ArgumentNullException("legs", "The itinerary cannot be null.");
			if (legs.Count() == 0)
				throw new ArgumentException("The itinerary cannot be empty.", "legs");

			_legs = new List<Leg>(legs);
		}

		/// <summary>
		/// Empty itinerary.
		/// </summary>
		public static Itinerary EmptyItinerary = new Itinerary();

		/// <summary>
		/// Gets unmodifiable collection of this itinerary's legs.
		/// </summary>
		public virtual IEnumerable<Leg> Legs
		{
			get { return _legs; }
		}

		/// <summary>
		/// Gets the location of first departure according to this itinerary.
		/// </summary>
		public virtual Location.Location InitialDepartureLocation
		{
			get { return IsEmpty ? Location.Location.Unknown : _legs.First().LoadLocation; }
		}

		/// <summary>
		/// Gets the location of last arrival according to this itinerary.
		/// </summary>
		public virtual Location.Location FinalArrivalLocation
		{
			get { return IsEmpty ? Location.Location.Unknown : _legs.Last().UnloadLocation; }
		}

		/// <summary>
		/// Gets the time of last arrival according to this itinerary. Returns null for empty itinerary.
		/// </summary>
		public virtual DateTime? FinalArrivalDate
		{
			get { return IsEmpty ? (DateTime?)null : _legs.Last().UnloadDate; }
		}

		/// <summary>
		/// Checks whether provided event is expected according to this itinerary specification.
		/// Test if the given handling event is expected when executing this itinerary.
		/// </summary>
		/// <param name="event">A handling event.</param>
		/// <returns>True, if it is expected. Otherwise - false.</returns>
		public virtual bool IsExpected(HandlingEvent @event)
		{
			if (IsEmpty)
				return true;

			if (@event.EventType == HandlingEventType.Receive)
			{
				//Check that the first leg's origin is the event's location
				Leg firstLeg = _legs.First();
				return firstLeg.LoadLocation == @event.Location;
			}

			if (@event.EventType == HandlingEventType.Claim)
			{
				//Check that the last leg's destination is from the event's location
				Leg lastLeg = _legs.Last();
				return lastLeg.UnloadLocation == @event.Location;
			}

			if (@event.EventType == HandlingEventType.Load)
			{
				//Check that the there is one leg with same load location and voyage
				return _legs.Any(x => x.LoadLocation == @event.Location && x.Voyage == @event.Voyage);
			}

			if (@event.EventType == HandlingEventType.Unload)
			{
				//Check that the there is one leg with same unload location and voyage
				return _legs.Any(x => x.UnloadLocation == @event.Location && x.Voyage == @event.Voyage);
			}
			
			return true;
		}

		private bool IsEmpty
		{
			get { return _legs.Count() == 0; }
		}

		#region Infrastructure
		
		protected override IEnumerable<object> GetAtomicValues()
		{
			foreach (Leg leg in _legs)
			{
				yield return leg;
			}
		}

		/// <summary>
		/// Equality operator between <see cref="Itinerary"/>s objects.
		/// </summary>
		/// <param name="left">Frist <see cref="Itinerary"/> to which the right <see cref="Itinerary"/> will be compared.</param>
		/// <param name="right">Second <see cref="Itinerary"/> to which the left <see cref="Itinerary"/> will be compared.</param>
		/// <returns>Returns <c>true</c> whenever both <see cref="Itinerary"/> are equal.</returns>
		public static bool operator ==(Itinerary left, Itinerary right)
		{
			return EqualOperator(left, right);
		}

		/// <summary>
		/// Inequality operator between <see cref="Itinerary"/>s objects.
		/// </summary>
		/// <param name="left">Frist <see cref="Itinerary"/> to which the right <see cref="Itinerary"/> will be compared.</param>
		/// <param name="right">Second <see cref="Itinerary"/> to which the left <see cref="Itinerary"/> will be compared.</param>
		/// <returns>Returns <c>true</c> whenever both <see cref="Itinerary"/> are not equal.</returns>
		public static bool operator !=(Itinerary left, Itinerary right)
		{
			return NotEqualOperator(left, right);
		}

		protected Itinerary()
		{
		}

		#endregion      
	}
}