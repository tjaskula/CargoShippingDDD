using System;
using BookingApi.Domain.Location;
using Xunit;
using Xunit.Extensions;

namespace BookingApi.UnitTests.Domain.Location
{
	public class UnLocodeTests
	{
		[Fact]
		public void should_throw_an_exception_when_code_is_null()
		{
			Assert.Throws<ArgumentNullException>(
					delegate
						{
							new UnLocode(null);
						}
				);
		}

		[Theory]
		[InlineData("AAAA")]
		[InlineData("AAAAAA")]
		[InlineData("AAAA")]
		[InlineData("AAAAAA")]
		[InlineData("22AAA")]
		[InlineData("AA111")]
		public void should_throw_an_exception_when_code_is_invalid(string code)
		{
			Assert.Throws<ArgumentException>(delegate
												{
													new UnLocode(code);
												}
											);
		}

		[Theory]
		[InlineData("AA234")]
		[InlineData("AAA9B")]
		[InlineData("AAAAA")]
		public void should_create_instance_when_code_is_valid(string code)
		{
			var unlocode = new UnLocode(code);

			Assert.Equal(code, unlocode.CodeString);
		}

		[Fact]
		public void should_equal_string_and_instance_tostring()
		{
			Assert.Equal("ABCDE", new UnLocode("ABCDE").ToString());
		}

		[Fact]
		public void should_not_be_case_sensitive()
		{
			Assert.Equal("ABCDE", new UnLocode("aBcDe").ToString());
		}

		[Theory]
		[InlineData("ABCDE")]
		[InlineData("aBcDe")]
		public void should_equal_mixed_cases_of_the_same_code(string code)
		{
			var unLocode = new UnLocode(code);
			Assert.Equal(unLocode, new UnLocode("aBcDe"));
			Assert.Equal(unLocode, new UnLocode("ABCDE"));
		}

		[Fact]
		public void codeString_should_send_code()
		{
			Assert.Equal("ABCDE", new UnLocode("aBcDe").CodeString);
		}
	}
}