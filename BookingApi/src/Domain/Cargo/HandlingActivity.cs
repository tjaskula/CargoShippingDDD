using System;
using System.Collections.Generic;
using BookingApi.Domain.Handling;

namespace BookingApi.Domain.Cargo
{
#pragma warning disable 660,661
	/// <summary>
	/// A handling activity represents how and where a cargo can be handled,
	/// and can be used to express predictions about what is expected to
	/// happen to a cargo in the future.
	/// </summary>
	public class HandlingActivity : ValueObject //Equals and GetHashCode are overridden in ValueObject class.
#pragma warning restore 660,661
	{
		private readonly HandlingEventType _eventType;
		private readonly Location.Location _location;
		private readonly Voyage.Voyage _voyage;

		/// <summary>
		/// Creates an instance of <see cref="HandlingActivity"/> from the handling event type and the location.
		/// </summary>
		/// <param name="eventType">The handling event type.</param>
		/// <param name="location">The location where the handling is done.</param>
		public HandlingActivity(HandlingEventType eventType, Location.Location location)
		{
			if (location == null)
				throw new ArgumentNullException("location");

			_eventType = eventType;
			_location = location;
		}

		/// <summary>
		/// Creates an instance of <see cref="HandlingActivity"/> from the handling event type and the location.
		/// </summary>
		/// <param name="eventType">The handling event type.</param>
		/// <param name="location">The location where the handling is done.</param>
		/// <param name="voyage"></param>
		public HandlingActivity(HandlingEventType eventType, Location.Location location, Voyage.Voyage voyage)
		{
			if (location == null)
				throw new ArgumentNullException("location");
			if (voyage == null)
				throw new ArgumentNullException("voyage");

			_eventType = eventType;
			_location = location;
			_voyage = voyage;
		}

		/// <summary>
		/// Gets the handling event type.
		/// </summary>
		public HandlingEventType EventType
		{
			get { return _eventType; }
		}

		/// <summary>
		/// Gets the location of the handling activity.
		/// </summary>
		public Location.Location Location
		{
			get { return _location; }
		}

		/// <summary>
		/// Gets the location of the handling activity.
		/// </summary>
		public Voyage.Voyage Voyage
		{
			get { return _voyage; }
		}

		protected override IEnumerable<object> GetAtomicValues()
		{
			yield return _eventType;
			yield return _location;
			yield return _voyage;
		}

		/// <summary>
		/// Compares two <see cref="HandlingActivity"/>s for equality.
		/// </summary>
		/// <param name="left">First <see cref="HandlingActivity"/>.</param>
		/// <param name="right">Other <see cref="HandlingActivity"/> to compare.</param>
		/// <returns>Returns <c>true</c> if both <see cref="HandlingActivity"/>s are equal.</returns>
		public static bool operator ==(HandlingActivity left, HandlingActivity right)
		{
			return EqualOperator(left, right);
		}

		/// <summary>
		/// Compares two <see cref="HandlingActivity"/>s for inequality.
		/// </summary>
		/// <param name="left">First <see cref="HandlingActivity"/>.</param>
		/// <param name="right">Other <see cref="HandlingActivity"/> to compare.</param>
		/// <returns>Returns <c>true</c> if both <see cref="HandlingActivity"/>s are not equal.</returns>
		public static bool operator !=(HandlingActivity left, HandlingActivity right)
		{
			return NotEqualOperator(left, right);
		}

		public override string ToString()
		{
			return "Event type : " + _eventType.ToString() + "| Location : " + Location + " | Voyage : " + _voyage;
		}

		protected HandlingActivity()
		{
		}
	}
}