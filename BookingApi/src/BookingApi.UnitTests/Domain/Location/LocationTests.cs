using System;
using BookingApi.Domain.Location;
using Xunit;

namespace BookingApi.UnitTests.Domain.Location
{
	public class LocationTests
	{
		[Fact]
		public void should_equal_string_and_instance_tostring()
		{
			Assert.Equal("Varsovie [ABCDE]", new BookingApi.Domain.Location.Location(new UnLocode("ABCDE"), "Varsovie").ToString());
		}

		[Fact]
		public void should_equal_same_unlocode()
		{
			Assert.Equal(new BookingApi.Domain.Location.Location(new UnLocode("ABCDE"), "Varsovie"), new BookingApi.Domain.Location.Location(new UnLocode("ABCDE"), "Varsovie"));
		}

		[Fact]
		public void should_not_equal_different_unlocode()
		{
			Assert.NotEqual(new BookingApi.Domain.Location.Location(new UnLocode("ABCDE"), "Varsovie"), new BookingApi.Domain.Location.Location(new UnLocode("ABCDF"), "Varsovie"));
		}

		[Fact]
		public void should_always_equal_to_itself()
		{
			var location = new BookingApi.Domain.Location.Location(new UnLocode("ABCDE"), "Varsovie");
			Assert.True(location.Equals(location));
		}

		[Fact]
		public void should_never_equal_to_null()
		{
			var location = new BookingApi.Domain.Location.Location(new UnLocode("ABCDE"), "Varsovie");
			Assert.False(location.Equals(null));
		}

		[Fact]
		public void should_equal_unknown_location_to_itself()
		{
			Assert.True(BookingApi.Domain.Location.Location.Unknown.Equals(BookingApi.Domain.Location.Location.Unknown));
		}

		[Fact]
		public void should_throw_an_exception_when_code_is_null()
		{
			Assert.Throws<ArgumentNullException>(
													delegate
														{
															new BookingApi.Domain.Location.Location(null, "name");
														}
												);
		}

		[Fact]
		public void should_throw_an_exception_when_name_is_null()
		{
			Assert.Throws<ArgumentNullException>(
													delegate
													{
														new BookingApi.Domain.Location.Location(new UnLocode("ABCDE"), null);
													}
												);
		}

		[Fact]
		public void should_throw_an_exception_when_name_is_empty()
		{
			Assert.Throws<ArgumentNullException>(
													delegate
													{
														new BookingApi.Domain.Location.Location(new UnLocode("ABCDE"), string.Empty);
													}
												);
		}
	}
}