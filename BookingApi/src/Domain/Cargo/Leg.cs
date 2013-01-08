using System;
using System.Collections.Generic;

namespace BookingApi.Domain.Cargo
{
#pragma warning disable 660,661
	/// <summary>
	/// Represents one step of an itinerary.
	/// </summary>
	public class Leg : ValueObject //Equals and GetHashCode are overridden in ValueObject class.
#pragma warning restore 660,661
	{
		private readonly Voyage.Voyage _voyage;

		private readonly Location.Location _loadLocation;
		private readonly Location.Location _unloadLocation;

		private readonly DateTime _loadDate;
		private readonly DateTime _unloadDate;

		/// <summary>
		/// Creates new leg instance.
		/// </summary>
		/// <param name="voyage"></param>
		/// <param name="loadLocation">Location where cargo is supposed to be loaded.</param>
		/// <param name="loadDate">Date and time when cargo is supposed to be loaded.</param>
		/// <param name="unloadLocation">Location where cargo is supposed to be unloaded.</param>
		/// <param name="unloadDate">Date and time when cargo is supposed to be unloaded.</param>
		public Leg(Voyage.Voyage voyage, Location.Location loadLocation, DateTime loadDate, Location.Location unloadLocation, DateTime unloadDate)
		{
			if (voyage == null)
				throw new ArgumentNullException("voyage", "The voyage cannot be null");
			if (loadLocation == null)
				throw new ArgumentNullException("loadLocation", "The load location cannot be null");
			if (unloadLocation == null)
				throw new ArgumentNullException("unloadLocation", "The unload location cannot be null");
			if (loadDate == default(DateTime))
				throw new ArgumentException("The load date is not correct.", "loadDate");
			if (unloadDate == default(DateTime))
				throw new ArgumentException("The unloadDate date is not correct.", "unloadDate");

			_voyage = voyage;
			_loadLocation = loadLocation;
			_unloadDate = unloadDate;
			_unloadLocation = unloadLocation;
			_loadDate = loadDate;
		}

		/// <summary>
		/// Gets the voyage of the cargo.
		/// </summary>
		public Voyage.Voyage Voyage
		{
			get { return _voyage; }
		}

		/// <summary>
		/// Gets location where cargo is supposed to be loaded.
		/// </summary>
		public Location.Location LoadLocation
		{
			get { return _loadLocation; }
		}

		/// <summary>
		/// Gets location where cargo is supposed to be unloaded.
		/// </summary>
		public Location.Location UnloadLocation
		{
			get { return _unloadLocation; }
		}

		/// <summary>
		/// Gets date and time when cargo is supposed to be loaded.
		/// </summary>
		public DateTime LoadDate
		{
			get { return _loadDate; }
		}

		/// <summary>
		/// Gets date and time when cargo is supposed to be unloaded.
		/// </summary>
		public DateTime UnloadDate
		{
			get { return _unloadDate; }
		}

		#region Infrastructure

		protected override IEnumerable<object> GetAtomicValues()
		{
			yield return _voyage;
			yield return _loadLocation;
			yield return _unloadLocation;
			yield return _loadDate;
			yield return _unloadDate;
		}

		/// <summary>
		/// Equality operator between <see cref="Leg"/>s objects.
		/// </summary>
		/// <param name="left">Frist <see cref="Leg"/> to which the right <see cref="Leg"/> will be compared.</param>
		/// <param name="right">Second <see cref="Leg"/> to which the left <see cref="Leg"/> will be compared.</param>
		/// <returns>Returns <c>true</c> whenever both <see cref="Leg"/> are equal.</returns>
		public static bool operator ==(Leg left, Leg right)
		{
			return EqualOperator(left, right);
		}

		/// <summary>
		/// Inequality operator between <see cref="Leg"/>s objects.
		/// </summary>
		/// <param name="left">Frist <see cref="Leg"/> to which the right <see cref="Leg"/> will be compared.</param>
		/// <param name="right">Second <see cref="Leg"/> to which the left <see cref="Leg"/> will be compared.</param>
		/// <returns>Returns <c>true</c> whenever both <see cref="Leg"/> are not equal.</returns>
		public static bool operator !=(Leg left, Leg right)
		{
			return NotEqualOperator(left, right);
		}

		protected Leg()
		{
		}

		#endregion
	}
}