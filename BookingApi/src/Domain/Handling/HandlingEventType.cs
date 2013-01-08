using System;

namespace BookingApi.Domain.Handling
{
	/// <summary>
	/// Handling event type. Either requires or prohibits a carrier movement association, it's never optional.
	/// </summary>
	public enum HandlingEventType
	{
		/// <summary>
		/// Loading the cargo.
		/// </summary>
		Load,

		/// <summary>
		/// Unloading the cargo.
		/// </summary>
		Unload,

		/// <summary>
		/// Receiving the cargo.
		/// </summary>
		Receive,

		/// <summary>
		/// Issuing the claim.
		/// </summary>
		Claim,

		/// <summary>
		/// Cargo at customs.
		/// </summary>
		Customs
	}

	/// <summary>
	/// <see cref="HandlingEventType"/> extnesion which helps in defining if an event type is requiring a <see cref="Voyage"/>.
	/// </summary>
	public static class HandlingEventTypeExtensions
	{
		/// <summary>
		/// Definies is an event type requires a voyage.
		/// </summary>
		/// <param name="type">The type of event for which a voyage association is required.</param>
		/// <returns>Returns true if a voyage association is required for this event type.</returns>
		public static bool RequiresVoyage(this HandlingEventType type)
		{
			if (type == HandlingEventType.Load) return true;
			if (type == HandlingEventType.Unload) return true;
			if (type == HandlingEventType.Claim) return false;
			if (type == HandlingEventType.Receive) return false;
			if (type == HandlingEventType.Customs) return false;

			throw new InvalidOperationException("Unknown handling event type.");
		}

		/// <summary>
		/// Definies is an event type prohibits a voyage.
		/// </summary>
		/// <param name="type">The type of event for which a voyage association is prohibited.</param>
		/// <returns>Returns true if a voyage association is prohibited for this event type.</returns>
		public static bool ProhibitsVoyage(this HandlingEventType type)
		{
			return !type.RequiresVoyage();
		}
	}
}