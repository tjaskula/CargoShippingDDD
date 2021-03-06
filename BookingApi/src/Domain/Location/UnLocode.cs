﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BookingApi.Domain.Location
{
#pragma warning disable 660,661
	/// <summary>
	/// United nations location code.
	/// 
	/// http://www.unece.org/cefact/locode/
	/// http://www.unece.org/cefact/locode/DocColumnDescription.htm#LOCODE
	/// </summary>
	public class UnLocode : ValueObject //Equals and GetHashCode are overridden in ValueObject class.
#pragma warning restore 660,661
	{
		private static readonly Regex CodePattern = new Regex("^[a-zA-Z]{2}[a-zA-Z2-9]{3}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		private readonly string _code;

		/// <summary>
		/// Creates new <see cref="UnLocode"/> object.
		/// </summary>
		/// <param name="code">String representation of location code.</param>
		public UnLocode(string code)
		{
			if (code == null)
				throw new ArgumentNullException("code");

			if (!CodePattern.Match(code).Success)
				throw new ArgumentException(string.Format("Provided code does not comply with a UnLocode pattern ({0})", CodePattern), "code");

			_code = code.ToUpper();
		}

		/// <summary>
		/// Returns a string representation of this UnLocode consisting of 5 characters (all upper):
		/// 2 chars of ISO country code and 3 describing location.
		/// </summary>
		public virtual string CodeString
		{
			get { return _code; }
		}

		protected override IEnumerable<object> GetAtomicValues()
		{
			yield return _code;
		}

		/// <summary>
		/// Compares two <see cref="UnLocode"/>s for equality.
		/// </summary>
		/// <param name="left">First <see cref="UnLocode"/>.</param>
		/// <param name="right">Other <see cref="UnLocode"/> to compare.</param>
		/// <returns>Returns <c>true</c> if both <see cref="UnLocode"/>s are equal.</returns>
		public static bool operator ==(UnLocode left, UnLocode right)
		{
			return EqualOperator(left, right);
		}

		/// <summary>
		/// Compares two <see cref="UnLocode"/>s for inequality.
		/// </summary>
		/// <param name="left">First <see cref="UnLocode"/>.</param>
		/// <param name="right">Other <see cref="UnLocode"/> to compare.</param>
		/// <returns>Returns <c>true</c> if both <see cref="UnLocode"/>s are not equal.</returns>
		public static bool operator !=(UnLocode left, UnLocode right)
		{
			return NotEqualOperator(left, right);
		}

		/// <summary>
		/// For NHibernate.
		/// </summary>
		protected UnLocode()
		{
		}

		public override string ToString()
		{
			return _code;
		}
	}
}