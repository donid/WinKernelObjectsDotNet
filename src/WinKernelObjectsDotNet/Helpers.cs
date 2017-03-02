using System;
using System.Linq;
using System.Runtime.InteropServices;


namespace WinKernelObjectsDotNet
{
	/// <summary>
	/// This class contains static helper methods
	/// </summary>
	internal static class Helpers
	{
		public static string MarshalUnicodeString(UNICODE_STRING unicodeString)
		{
			if (unicodeString.Length == 0)
			{
				return string.Empty;
			}
			return Marshal.PtrToStringUni(unicodeString.Buffer, unicodeString.Length >> 1); //  '>> 1' is essentially '/ 2'
		}

		public static int RoundUp(int num, int multiple)
		{
			if (multiple == 0)
			{
				throw new ArgumentException("multiple must not be 0.", nameof(multiple));
			}
			int add = multiple / Math.Abs(multiple);
			return ((num + multiple - add) / multiple) * multiple;
		}
	}
}