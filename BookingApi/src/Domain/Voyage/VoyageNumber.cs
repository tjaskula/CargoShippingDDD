using System;
using System.Collections.Generic;

namespace BookingApi.Domain.Voyage
{
#pragma warning disable 660,661
	/// <summary>
	/// A unique voyage number.
	/// </summary>
	public class VoyageNumber : ValueObject //Equals and GetHashCode are overridden in ValueObject class.
#pragma warning restore 660,661
	{
		private readonly string _number;

		/// <summary>
		/// Creates new voyage number using specified string.
		/// </summary>
		/// <param name="number">String represantation of voyage number.</param>
		public VoyageNumber(string number)
		{
			if (number == null)
				throw new ArgumentNullException("number");

			_number = number;
		}

		/// <summary>
		/// Gets string representation of this voyage number.
		/// </summary>
		public virtual string NumberString
		{
			get { return _number; }
		}

		/// <summary>
		/// Compares two <see cref="VoyageNumber"/>s for equality.
		/// </summary>
		/// <param name="left">First <see cref="VoyageNumber"/>.</param>
		/// <param name="right">Other <see cref="VoyageNumber"/> to compare.</param>
		/// <returns>Returns true if both <see cref="VoyageNumber"/>s are equal.</returns>
		public static bool operator ==(VoyageNumber left, VoyageNumber right)
		{
			return EqualOperator(left, right);
		}

		/// <summary>
		/// Compares two <see cref="VoyageNumber"/>s for inequality.
		/// </summary>
		/// <param name="left">First <see cref="VoyageNumber"/>.</param>
		/// <param name="right">Other <see cref="VoyageNumber"/> to compare.</param>
		/// <returns>Returns true if both <see cref="VoyageNumber"/>s are not equal.</returns>
		public static bool operator !=(VoyageNumber left, VoyageNumber right)
		{
			return NotEqualOperator(left, right);
		}

		protected override IEnumerable<object> GetAtomicValues()
		{
			yield return _number;
		}

		protected VoyageNumber()
		{
		}

		public override string ToString()
		{
			return _number;
		}
	}
}