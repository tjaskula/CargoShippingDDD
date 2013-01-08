using System;
using BookingApi.Domain.Cargo;
using Xunit;

namespace BookingApi.UnitTests.Domain.Cargo
{
    public class TrackingIdTests
    {
        [Fact]
        public void should_throw_exception_with_null_id()
        {
            Assert.Throws<ArgumentNullException>(
                    delegate
                        {
                            new TrackingId(null);
                        }
                );
        }

        [Fact]
        public void should_throw_exception_with_empty_id()
        {
            Assert.Throws<ArgumentNullException>(
                    delegate
                        {
                            new TrackingId(string.Empty);
                        }
                );    
        }

        [Fact]
        public void should_create_instance_with_valid_id()
        {
            Assert.NotNull(new TrackingId("ABCD"));
        }
    }
}