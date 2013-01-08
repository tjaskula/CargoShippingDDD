using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApi.Domain.Voyage
{
#pragma warning disable 660,661
	/// <summary>
	/// A voyage schedule.
	/// </summary>
	public class Schedule : ValueObject //Equals and GetHashCode are overridden in ValueObject class.
#pragma warning restore 660,661
	{
		private readonly IList<CarrierMovement> _carrierMouvements;

		/// <summary>
		/// Empty voyage.
		/// </summary>
		public static Schedule Empty = new Schedule(); // Null object pattern

		/// <summary>
		/// Creates an instance of <see cref="Schedule"/> from carrier mouvements.
		/// </summary>
		/// <param name="carrierMovements"></param>
		public Schedule(IList<CarrierMovement> carrierMovements)
		{
			// check if not null
			if (carrierMovements == null)
				throw new ArgumentNullException("carrierMovements");    

			// no null elements
			if (carrierMovements.Count() > 0 && carrierMovements.ToList().Exists(carrierMovement => carrierMovement == null))
				throw new ArgumentException("carrierMovements contains null elements.");

			// no elements
			if (carrierMovements.Count() == 0)
				throw new ArgumentException("carrierMovements has no elements.");

			_carrierMouvements = carrierMovements;
		}

		/// <summary>
		/// Gets the carrier movements.
		/// </summary>
		public IList<CarrierMovement> CarrierMouvements
		{
			get { return _carrierMouvements; }
		}

		/// <summary>
		/// Compares two <see cref="Schedule"/>s for equality.
		/// </summary>
		/// <param name="left">First <see cref="Schedule"/>.</param>
		/// <param name="right">Other <see cref="Schedule"/> to compare.</param>
		/// <returns>Returns <c>true</c> if both <see cref="Schedule"/>s are equal.</returns>
		public static bool operator ==(Schedule left, Schedule right)
		{
			return EqualOperator(left, right);
		}

		/// <summary>
		/// Compares two <see cref="Schedule"/>s for inequality.
		/// </summary>
		/// <param name="left">First <see cref="Schedule"/>.</param>
		/// <param name="right">Other <see cref="Schedule"/> to compare.</param>
		/// <returns>Returns <c>true</c> if both <see cref="Schedule"/>s are not equal.</returns>
		public static bool operator !=(Schedule left, Schedule right)
		{
			return NotEqualOperator(left, right);
		}

		protected override IEnumerable<object> GetAtomicValues()
		{
			foreach (var movement in _carrierMouvements)
			{
				yield return movement;
			}
		}

		// For NHibernate.
		protected Schedule()
		{
		}
	}
}