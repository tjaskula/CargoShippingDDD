using System;
using System.Collections.Generic;
using System.Text;
using BookingApi.Domain.Cargo;

namespace BookingApi.Domain.Handling
{
#pragma warning disable 660,661
	/// <summary>
	/// Single cargo handling event.
	/// 
	/// A HandlingEvent is used to register the event when, for instance,
	/// a cargo is unloaded from a carrier at a some loacation at a given time.
	/// <p/>
	/// The HandlingEvent's are sent from different Incident Logging Applications
	/// some time after the event occured and contain information about the
	/// <see cref="TrackingId"/>, <see cref="Domain.Location.Location"/>, timestamp of the completion of the event,
	/// and possibly, if applicable a <see cref="Voyage"/>.
	/// <p/>
	/// This class is the only member, and consequently the root, of the HandlingEvent aggregate. 
	/// <p/>
	/// HandlingEvent's could contain information about a <see cref="Voyage"/> and if so,
	/// the event type must be either <see cref="HandlingEventType"/> LOAD or <see cref="HandlingEventType"/> UNLOAD.
	/// <p/>
	/// All other events must be of <see cref="HandlingEventType"/> RECEIVE, <see cref="HandlingEventType"/> CLAIM or <see cref="HandlingEventType"/> CUSTOMS.
	/// </summary>   
	public class HandlingEvent : ValueObject //Equals and GetHashCode are overridden in ValueObject class.
#pragma warning restore 660,661
	{
		private readonly HandlingEventType _eventType;
		private readonly Location.Location _location;
		private readonly DateTime _registrationDate;
		private readonly DateTime _completionDate;
		private readonly Cargo.Cargo _cargo;
		private readonly Voyage.Voyage _voyage;

		/// <summary>
		/// Creates new event.
		/// </summary>
		/// <param name="eventType">Type of the event.</param>
		/// <param name="location">The location where the event took place.</param>
		/// <param name="registrationDate">Registration time, the time the message is received.</param>
		/// <param name="completionDate">Completion time, the reported time that the event actually happened (e.g. the receive took place).</param>
		/// <param name="cargo">Cargo.</param>
		/// <param name="voyage">The voyage.</param>
		public HandlingEvent(HandlingEventType eventType, Location.Location location, DateTime registrationDate, DateTime completionDate, Cargo.Cargo cargo, Voyage.Voyage voyage)
		{
			if (cargo == null)
				throw new ArgumentNullException("cargo", "Cargo is required.");
			if (location == null)
				throw new ArgumentNullException("location", "Location is required.");
			if (voyage == null)
				throw new ArgumentNullException("voyage", "Voyage is required.");
			if (registrationDate == default(DateTime))
				throw new ArgumentException("The registration date is required.", "registrationDate");
			if (completionDate == default(DateTime))
				throw new ArgumentException("The completion date is required.", "completionDate");

			if (eventType.ProhibitsVoyage())
				throw new ArgumentException("Voyage is not allowed with event type : " + eventType, "eventType");

			_eventType = eventType;
			_completionDate = completionDate;
			_registrationDate = registrationDate;
			_location = location;
			_cargo = cargo;
			_voyage = voyage;
		}

		/// <summary>
		/// Creates new event.
		/// </summary>
		/// <param name="eventType">Type of the event.</param>
		/// <param name="location">The location where the event took place.</param>
		/// <param name="registrationDate">Registration time, the time the message is received.</param>
		/// <param name="completionDate">Completion time, the reported time that the event actually happened (e.g. the receive took place).</param>
		/// <param name="cargo">Cargo.</param>
		public HandlingEvent(HandlingEventType eventType, Location.Location location, DateTime registrationDate, DateTime completionDate, Cargo.Cargo cargo)
		{
			if (cargo == null)
				throw new ArgumentNullException("cargo", "Cargo is required.");
			if (location == null)
				throw new ArgumentNullException("location", "Location is required.");
			if (registrationDate == default(DateTime))
				throw new ArgumentException("The registration date is required.", "registrationDate");
			if (completionDate == default(DateTime))
				throw new ArgumentException("The completion date is required.", "completionDate");

			if (eventType.RequiresVoyage())
				throw new ArgumentException("Voyage is required for event type event type : " + eventType, "eventType");

			_eventType = eventType;
			_completionDate = completionDate;
			_registrationDate = registrationDate;
			_location = location;
			_cargo = cargo;
			_voyage = null;
		}

		/// <summary>
		/// Date when action represented by the event was completed.
		/// </summary>
		public DateTime CompletionDate
		{
			get { return _completionDate; }
		}

		/// <summary>
		/// Date when event was registered.
		/// </summary>
		public DateTime RegistrationDate
		{
			get { return _registrationDate; }
		}

		/// <summary>
		/// Location where event occured.
		/// </summary>
		public Location.Location Location
		{
			get { return _location; }
		}

		/// <summary>
		/// Type of the event.
		/// </summary>
		public HandlingEventType EventType
		{
			get { return _eventType; }
		}

		/// <summary>
		/// Gets the cargo.
		/// </summary>
		public Cargo.Cargo Cargo
		{
			get { return _cargo; }
		}

		/// <summary>
		/// Gets the voyage.
		/// </summary>
		public Voyage.Voyage Voyage
		{
			get { return _voyage ?? Domain.Voyage.Voyage.Empty; }
		}

		#region Infrastructure

		protected override IEnumerable<object> GetAtomicValues()
		{
			yield return _eventType;
			yield return _location;
			yield return _completionDate;
			yield return _cargo;
			yield return _voyage;
		}

		/// <summary>
		/// Equality operator between <see cref="Domain.Handling.HandlingEvent"/>s objects.
		/// </summary>
		/// <param name="left">Frist <see cref="HandlingEvent"/> to which the right <see cref="HandlingEvent"/> will be compared.</param>
		/// <param name="right">Second <see cref="HandlingEvent"/> to which the left <see cref="HandlingEvent"/> will be compared.</param>
		/// <returns>Returns <c>true</c> whenever both <see cref="HandlingEvent"/> are equal.</returns>
		public static bool operator ==(HandlingEvent left, HandlingEvent right)
		{
			return EqualOperator(left, right);
		}

		/// <summary>
		/// Inequality operator between <see cref="HandlingEvent"/>s objects.
		/// </summary>
		/// <param name="left">Frist <see cref="HandlingEvent"/> to which the right <see cref="HandlingEvent"/> will be compared.</param>
		/// <param name="right">Second <see cref="HandlingEvent"/> to which the left <see cref="HandlingEvent"/> will be compared.</param>
		/// <returns>Returns <c>true</c> whenever both <see cref="HandlingEvent"/> are not equal.</returns>
		public static bool operator !=(HandlingEvent left, HandlingEvent right)
		{
			return NotEqualOperator(left, right);
		}

		#endregion

		/// <summary>
		/// Required by NHibernate.
		/// </summary>
		protected HandlingEvent()
		{
		}

		public override string ToString()
		{
			var stringBuilder = new StringBuilder(Environment.NewLine + "--- Handling event ---" + Environment.NewLine);
			stringBuilder.AppendLine("Cargo : ").Append(_cargo.TrackingId);
			stringBuilder.AppendLine("Type : ").Append(_eventType);
			stringBuilder.AppendLine("Location : ").Append(_location);
			stringBuilder.AppendLine("Completed on : ").Append(_completionDate);
			stringBuilder.AppendLine("Registered on : ").Append(_registrationDate);

			if (_voyage != null)
				stringBuilder.AppendLine("Voyage : ").Append(_voyage.Number);

			return stringBuilder.ToString();
		}
	}
}