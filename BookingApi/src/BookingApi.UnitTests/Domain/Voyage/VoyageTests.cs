using System;
using System.Collections.Generic;
using BookingApi.Domain.Location;
using BookingApi.Domain.Voyage;
using Xunit;

namespace BookingApi.UnitTests.Domain.Voyage
{
    public class VoyageTests
    {
        [Fact]
        public void should_throw_an_exception_when_number_is_null()
        {
            Assert.Throws<ArgumentNullException>(
                    delegate
                    {
                        new BookingApi.Domain.Voyage.Voyage(null,
                                       new Schedule(new List<CarrierMovement>
                                                        {
                                                            new CarrierMovement(
                                                                new BookingApi.Domain.Location.Location(new UnLocode("AS23D"), "NAME"),
                                                                new BookingApi.Domain.Location.Location(new UnLocode("AZ32D"), "DESTINATION"),
                                                                new DateTime(2010, 3, 15), new DateTime(2010, 4, 15))
                                                        }));
                    }
                );
        }

        [Fact]
        public void should_throw_an_exception_when_schedule_is_null()
        {
            Assert.Throws<ArgumentNullException>(
                    delegate
                    {
                        new BookingApi.Domain.Voyage.Voyage(new VoyageNumber("ABCD"), null);
                    }
                );
        }
    }
}