using System;
using System.Collections.Generic;
using BookingApi.Domain.Voyage;

namespace BookingApi.Domain.Services
{
	/// <summary>
	/// Builder pattern is used for incremental construction
	/// of a Voyage aggregate. This serves as an aggregate factory.
	/// </summary>
	public class VoyageBuilder
	{
		private readonly IList<CarrierMovement> _carrierMovements = new List<CarrierMovement>();
		private readonly VoyageNumber _voyageNumber;
		private Location.Location _departureLocation;

		/// <summary>
		/// Creates a new instance of <see cref="VoyageBuilder"/>.
		/// </summary>
		/// <param name="voyageNumber">Voyage number.</param>
		/// <param name="departureLocation">Departure location.</param>
		public VoyageBuilder(VoyageNumber voyageNumber, Location.Location departureLocation)
		{
			if (voyageNumber == null)
			{
				throw new ArgumentNullException("voyageNumber");
			}

			if (departureLocation == null)
			{
				throw new ArgumentNullException("departureLocation");
			}

			_voyageNumber = voyageNumber;
			_departureLocation = departureLocation;
		}

		/// <summary>
		/// Adds a new <see cref="CarrierMovement"/> to the internal list of carrier movements.
		/// </summary>
		/// <param name="arrivalLocation">Arrival location.</param>
		/// <param name="departureTime">Departure time.</param>
		/// <param name="arrivalTime">Arrival time.</param>
		/// <returns>Returns updated instance of <see cref="VoyageBuilder"/>.</returns>
		public VoyageBuilder AddMovement(Location.Location arrivalLocation, DateTime departureTime, DateTime arrivalTime)
		{
			_carrierMovements.Add(new CarrierMovement(_departureLocation, arrivalLocation, departureTime, arrivalTime));
			_departureLocation = arrivalLocation;
			return this;
		}

		/// <summary>
		/// Builds up a new <see cref="Voyage"/> and assigns it a <see cref="Schedule"/>.
		/// </summary>
		/// <returns>Returns a new instance of <see cref="Voyage"/>.</returns>
		public Voyage.Voyage Build()
		{
			return new Voyage.Voyage(_voyageNumber, new Schedule(_carrierMovements));
		}
	}
}