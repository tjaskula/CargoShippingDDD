﻿namespace BookingApi.Domain.Handling
{
    /// <summary>
    /// Signals that a cargo was handled.
    /// </summary>
    public class CargoWasHandledEvent : DomainEvent<HandlingEvent>
    {
        /// <summary>
        /// Creates new event instance.
        /// </summary>
        /// <param name="source"></param>
        public CargoWasHandledEvent(HandlingEvent source)
            : base(source)
        {
        }
    }
}