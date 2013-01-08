using System;
using System.Collections.Generic;
using System.Linq;
using BookingApi.Domain.Cargo;

namespace BookingApi.Domain.Handling
{
#pragma warning disable 660,661
	/// <summary>
	/// Contains information about cargo handling history. Enables registration of cargo
	/// handling events.
	/// </summary>
	public class HandlingHistory : ValueObject
#pragma warning restore 660,661
	{
		private readonly IList<HandlingEvent> _events;

		/// <summary>
		/// Empty handling history.
		/// </summary>
		public static HandlingHistory EmptyHistory = new HandlingHistory(new List<HandlingEvent>());

		/// <summary>
		/// Creates an instance of <see cref="BookingApi.Domain.Handling.HandlingHistory"/>.
		/// </summary>
		public HandlingHistory(IEnumerable<HandlingEvent> events)
		{
			if (events == null)
				throw new ArgumentNullException("events", "Handling events cannot be null.");

			_events = new List<HandlingEvent>(events);
		}

		/// <summary>
		/// Gets a distinct list (no duplicate registrations) of handling events, ordered by completion time.
		/// </summary>
		public virtual IEnumerable<HandlingEvent> DistinctEventsByCompletionTime
		{
			get { return _events.Distinct().OrderBy(x => x.CompletionDate); }
		}

		/// <summary>
		/// Gets the most recently completed event, or null if the delivery history is empty.
		/// </summary>
		public virtual HandlingEvent MostRecentlyCompletedEvent
		{
			get 
			{ 
				var distinctEvents = DistinctEventsByCompletionTime;
				return distinctEvents.LastOrDefault();
			}
		}

		/// <summary>
		/// Gets tracking id of cargo which this history object belongs to.
		/// </summary>
		public virtual TrackingId TrackingId { get; protected set; }

		/// <summary>
		/// Compares two instance of <see cref="HandlingHistory"/> for equality.
		/// </summary>
		/// <param name="left">First <see cref="HandlingHistory"/>.</param>
		/// <param name="right">Second <see cref="HandlingHistory"/>.</param>
		/// <returns>Returns true if both <see cref="HandlingHistory"/>s are equal.</returns>
		public static bool operator ==(HandlingHistory left, HandlingHistory right)
		{
			return EqualOperator(left, right);
		}

		/// <summary>
		/// Compares two instance of <see cref="HandlingHistory"/> for inequality.
		/// </summary>
		/// <param name="left">First <see cref="HandlingHistory"/>.</param>
		/// <param name="right">Second <see cref="HandlingHistory"/>.</param>
		/// <returns>Returns true if both <see cref="HandlingHistory"/>s are not equal.</returns>
		public static bool operator !=(HandlingHistory left, HandlingHistory right)
		{
			return NotEqualOperator(left, right);
		}

		protected override IEnumerable<object> GetAtomicValues()
		{
			return _events;
		}

		/// <summary>
		/// Required by NHibernate.
		/// </summary>
		protected HandlingHistory()
		{
		}
	}
}