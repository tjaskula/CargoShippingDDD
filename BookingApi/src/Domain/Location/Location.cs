using System;

namespace BookingApi.Domain.Location
{
	/// <summary>
	/// A location is our model is stops on a journey, such as cargo
	/// origin or destination, or carrier movement endpoints.
	/// 
	/// It is uniquely identified by a UN Locode.
	/// </summary>
	public class Location
	{
		/// <summary>
		/// Gets the <see cref="UnLocode"/> for this location.
		/// </summary>
		public virtual UnLocode UnLocode { get; protected set; }

		/// <summary>
		/// Gets the name of this location, e.g. Paris.
		/// </summary>
		public virtual string Name { get; protected set; }

		/// <summary>
		/// Returns an instance indicating an unknown location.
		/// </summary>
		public static Location Unknown
		{
			get { return new Location(new UnLocode("XXXXX"), "Unknown location"); }
		}

		/// <summary>
		/// Creates new location.
		/// </summary>
		/// <param name="locode"><see cref="UnLocode"/> for this location.</param>
		/// <param name="name">Name.</param>
		public Location(UnLocode locode, string name)
		{
			if (locode == null)
				throw new ArgumentNullException("locode", "The UN locode should not be null");
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name", "The name cannot be null or empty");

			UnLocode = locode;
			Name = name;
		}

		/// <summary>
		/// For NHibernate.
		/// </summary>
		protected Location()
		{
		}

		/// <summary>
		/// Compares two <see cref="Location"/>s for equality.
		/// </summary>
		/// <param name="left">First <see cref="Location"/>.</param>
		/// <param name="right">Other <see cref="Location"/> to compare.</param>
		/// <returns>Returns true if both <see cref="Location"/>s are equal.</returns>
		public static bool operator ==(Location left, Location right)
		{
			return EqulaOperator(left, right);
		}

		/// <summary>
		/// Compares two <see cref="Location"/>s for inequality.
		/// </summary>
		/// <param name="left">First <see cref="Location"/>.</param>
		/// <param name="right">Other <see cref="Location"/> to compare.</param>
		/// <returns>Returns true if both <see cref="Location"/>s are not equal.</returns>
		public static bool operator !=(Location left, Location right)
		{
			return !EqulaOperator(left, right);
		}

		private static bool EqulaOperator(Location left, Location right)
		{
			if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
			{
				return false;
			}

			return ReferenceEquals(left, null) || left.Equals(right);
		}

		/// <summary>
		/// Compares the current instance with another instance.
		/// </summary>
		/// <param name="obj">Object to compare.</param>
		/// <returns>Since this is an entiy this will be true iff UN locodes are equal.</returns>
		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != GetType())
				return false;

			var other = obj as Location;

			return UnLocode == other.UnLocode;
		}

		/// <summary>
		/// Gets an hash from UnLocode.
		/// </summary>
		/// <returns>Hash code of UN locode.</returns>
		public override int GetHashCode()
		{
			return UnLocode.GetHashCode();
		}

		public override string ToString()
		{
			return Name + " [" + UnLocode + "]";
		}
	}
}