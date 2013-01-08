using System;
using BookingApi.Domain.Voyage;
using Xunit;

namespace BookingApi.UnitTests.Domain.Voyage
{
    public class VoyageNumberTests
    {
        [Fact]
        public void should_throw_an_exception_when_number_is_null()
        {
            Assert.Throws<ArgumentNullException>(
                    delegate
                    {
                        new VoyageNumber(null);
                    }
                );
        }

        [Fact]
        public void should_not_throw_an_exception_when_number_is_empty()
        {
            Assert.NotNull(new VoyageNumber(""));
        }

        [Fact]
        public void two_instances_wtih_the_same_number_should_be_equal()
        {
            var instance1 = new VoyageNumber("ABC");
            var instance2 = new VoyageNumber("ABC");

            Assert.True((instance1 == instance2));
            Assert.False((instance1 != instance2));
        }

        [Fact]
        public void two_instances_wtih_different_numbers_should_not_be_equal()
        {
            var instance1 = new VoyageNumber("ABC");
            var instance2 = new VoyageNumber("ABD");

            Assert.False((instance1 == instance2));
            Assert.True((instance1 != instance2));
        }
    }
}