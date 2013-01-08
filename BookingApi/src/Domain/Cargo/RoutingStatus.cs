namespace BookingApi.Domain.Cargo
{
    /// <summary>
    /// Describes status of cargo routing.
    /// </summary>
    public enum RoutingStatus
    {
        /// <summary>
        /// Cargo hasn't been routed yet.
        /// </summary>
        NotRouted,

        /// <summary>
        /// Cargo is misrouted.
        /// </summary>
        Misrouted,

        /// <summary>
        /// Cargo is on its route.
        /// </summary>
        Routed
    }
}