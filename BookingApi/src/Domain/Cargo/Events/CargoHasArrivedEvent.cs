namespace BookingApi.Domain.Cargo.Events
{
	/// <summary>
	/// Raised after cargo has arrived at destination.
	/// </summary>
	public class CargoHasArrivedEvent : DomainEvent<Cargo>
	{
		/// <summary>
		/// Creates an event when cargo has arrived to its destination.
		/// </summary>
		/// <param name="source">The cargo that arrived to the destination for which an event should be generated.</param>
		public CargoHasArrivedEvent(Cargo source) : base(source)
		{
		}
	}
}