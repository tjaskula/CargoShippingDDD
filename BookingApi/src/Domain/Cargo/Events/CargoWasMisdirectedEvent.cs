namespace BookingApi.Domain.Cargo.Events
{
	/// <summary>
	/// Raised after cargo has been found to be misdirected.
	/// </summary>
	public class CargoWasMisdirectedEvent : DomainEvent<Cargo>
	{
		/// <summary>
		/// Creates an event when the cargo was misdirected.
		/// </summary>
		/// <param name="source">The cargo thats beeing misdirected.</param>
		public CargoWasMisdirectedEvent(Cargo source) : base(source)
		{
		}
	}
}