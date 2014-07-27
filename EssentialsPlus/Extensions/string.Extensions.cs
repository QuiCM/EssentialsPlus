using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EssentialsPlus.Extensions {
	public static class StringExtensions {
		/// <summary>
		/// Performs a case insensitive "Contains"
		/// </summary>
		/// <returns>
		/// true if the substring findText was found in the string, or
		/// false otherwise, or there was an error.
		/// </returns>
		public static bool ContainsInsensitive(this string str, string findText)
		{
			if (string.IsNullOrEmpty(str) == true
				|| string.IsNullOrEmpty(findText) == true) {
					return false;
			}

			return str.IndexOf(findText, StringComparison.OrdinalIgnoreCase) >= 0;
		}
	}
}
