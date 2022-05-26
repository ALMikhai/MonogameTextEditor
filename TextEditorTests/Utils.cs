using System;
using System.Text;

namespace TextEditorTests
{
	public class Utils
	{
		public static string BuildRandomString(int length)
		{
			var strBuild = new StringBuilder();
			var random = new Random();
			for (int i = 0; i < length; i++) {
				double flt = random.NextDouble();
				int shift = Convert.ToInt32(Math.Floor(25 * flt));
				strBuild.Append(Convert.ToChar(shift + 65));
			}

			return strBuild.ToString();
		}
	}
}
