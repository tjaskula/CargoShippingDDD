namespace BookingApi.Domain.Cargo.Events
{
	/// <summary>
	/// Raised after cargo has arrived assigned to route.
	/// </summary>
	public sealed class CargoHasBeenAssignedToRouteEvent : DomainEvent<Cargo>
	{
		private readonly Itinerary _oldItinerary;

		/// <summary>
		/// Creates an event which happens when a cargo has been assigned to route.
		/// </summary>
		/// <param name="source">The cargo that's beening assigned to the route.</param>
		/// <param name="oldItinerary">Cargo's old itinerary.</param>
		public CargoHasBeenAssignedToRouteEvent(Cargo source, Itinerary oldItinerary) : base(source)
		{
			_oldItinerary = oldItinerary;
		}

		/// <summary>
		/// Gets the route before assigning cargo to a new one.
		/// </summary>
		public Itinerary OldItinerary
		{
			get { return _oldItinerary; }
		}
	}
}