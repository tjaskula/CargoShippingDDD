using System;
using System.Text;
using System.Threading;
using BookingApi.Domain.Location;

namespace BookingApi.UnitTests.Domain
{
	public static class UnLocodeHelpers
	{
		public static UnLocode GetNewUnLocode()
		{
			return new UnLocode(UnLocodeString());
		}

		private static string UnLocodeString()
		{
			//var stringBuilder = new StringBuilder();

			//for (int i = 0; i < 5; i++)
			//    stringBuilder.Append(GetRandomUpperCaseCharacter(i));

			//return stringBuilder.ToString();

			return RandomString(5, false);
		}

		private static char GetRandomUpperCaseCharacter(int seed)
		{
			return ((char)((short)'A' + new Random(seed).Next(26)));
		}

		/// <summary>
		/// Generates a random string with the given length
		/// </summary>
		/// <param name="size">Size of the string</param>
		/// <param name="lowerCase">If true, generate lowercase string</param>
		/// <returns>Random string</returns>
		private static string RandomString(int size, bool lowerCase)
		{
			var builder = new StringBuilder();
			var random = new Random();
			char ch;
			
			for (int i = 0; i < size; i++)
			{
				ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
				builder.Append(ch);
			}

			// as random numbers are generated per actual date time, we have to wait a litte because in a batch execution of unit test several different instances
			// find themeselves assigned with a same code.
			Thread.Sleep(20);

			if (lowerCase)
				return builder.ToString().ToLower();
			return builder.ToString();
		}
	}
}