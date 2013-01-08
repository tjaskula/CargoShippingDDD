using System;
using System.Collections.Generic;

namespace BookingApi.Domain.Voyage
{
#pragma warning disable 660,661
	/// <summary>
	/// A carrier movement is a vessel voyage from one location to another.
	/// </summary>
	public class CarrierMovement : ValueObject //Equals and GetHashCode are overridden in ValueObject class.
#pragma warning restore 660,661
	{
		private readonly Location.Location _departureLocation;
		private readonly Location.Location _arrivalLocation;
		private readonly DateTime _departureTime;
		private readonly DateTime _arrivalTime;

		/// <summary>
		/// Default empty <see cref="CarrierMovement"/>.
		/// </summary>
		public static CarrierMovement Empty = new CarrierMovement(Location.Location.Unknown, Location.Location.Unknown, DateTime.Now, DateTime.Now);

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="departureLocation">Location of departure.</param>
		/// <param name="arrivalLocation">Location of arrival.</param>
		/// <param name="departureTime">Time of departure.</param>
		/// <param name="arrivalTime">Time of arrival.</param>
		public CarrierMovement(Location.Location departureLocation,
							   Location.Location arrivalLocation,
							   DateTime departureTime,
							   DateTime arrivalTime)
		{
			if (departureLocation == null)
			{
				throw new ArgumentNullException("departureLocation");
			}

			if (arrivalLocation == null)
			{
				throw new ArgumentNullException("arrivalLocation");
			}

			if (departureTime == default(DateTime))
			{
				throw new ArgumentException("departureTime is not initialized");
			}

			if (arrivalTime == default(DateTime))
			{
				throw new ArgumentException("arrivalTime is not initialized");
			}

			_departureLocation = departureLocation;
			_arrivalLocation = arrivalLocation;
			_departureTime = departureTime;
			_arrivalTime = arrivalTime;
		}

		/// <summary>
		/// Gets the departure location.
		/// </summary>
		public Location.Location DepartureLocation { get { return _departureLocation; } }

		/// <summary>
		/// Gets the arrival location.
		/// </summary>
		public Location.Location ArrivalLocation { get { return _arrivalLocation; } }

		/// <summary>
		/// Gets the departure time.
		/// </summary>
		public DateTime DepartureTime { get { return _departureTime; } }

		/// <summary>
		/// Gets the arrival time.
		/// </summary>
		public DateTime ArrivalTime { get { return _arrivalTime; } }

		/// <summary>
		/// Compares two <see cref="CarrierMovement"/>s for equality.
		/// </summary>
		/// <param name="left">First <see cref="CarrierMovement"/>.</param>
		/// <param name="right">Other <see cref="CarrierMovement"/> to compare.</param>
		/// <returns>Returns <c>true</c> if both <see cref="CarrierMovement"/>s are equal.</returns>
		public static bool operator ==(CarrierMovement left, CarrierMovement right)
		{
			return EqualOperator(left, right);
		}

		/// <summary>
		/// Compares two <see cref="CarrierMovement"/>s for inequality.
		/// </summary>
		/// <param name="left">First <see cref="CarrierMovement"/>.</param>
		/// <param name="right">Other <see cref="CarrierMovement"/> to compare.</param>
		/// <returns>Returns <c>true</c> if both <see cref="CarrierMovement"/>s are not equal.</returns>
		public static bool operator !=(CarrierMovement left, CarrierMovement right)
		{
			return NotEqualOperator(left, right);
		}

		protected override IEnumerable<object> GetAtomicValues()
		{
			yield return _departureLocation;
			yield return _arrivalLocation;
			yield return _departureTime;
			yield return _arrivalTime;
		}
	}
}