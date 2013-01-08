using System;

namespace BookingApi.Domain.Voyage
{
	/// <summary>
	/// A voyage
	/// </summary>
	public class Voyage
	{
		/// <summary>
		/// Empty voyage.
		/// </summary>
		public static Voyage Empty = new Voyage(new VoyageNumber(""), Schedule.Empty); // Null object pattern

		/// <summary>
		/// Gets unique identifier of this voyage.
		/// </summary>
		public virtual VoyageNumber Number { get; protected set; }

		/// <summary>
		/// Gets unique identifier of this voyage.
		/// </summary>
		public virtual Schedule Schedule { get; protected set; }

		/// <summary>
		/// Creates new Voyage object.
		/// </summary>
		/// <param name="number">Identifier of this voyage.</param>
		/// <param name="schedule">Voyage schedule.</param>
		public Voyage(VoyageNumber number, Schedule schedule)
		{
			if (number == null)
				throw new ArgumentNullException("number");

			if (schedule == null)
				throw new ArgumentNullException("schedule");

			Number = number;
			Schedule = schedule;
		}

		// For NHibernate
		protected Voyage()
		{
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != GetType())
				return false;

			var other = obj as Voyage;

			return Number == other.Number;
		}

		public override int GetHashCode()
		{
			return Number.GetHashCode();
		}

		public override string ToString()
		{
			return Number.ToString();
		}
	}
}