using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EssentialsPlus.Extensions
{
	public static class StringExtensions
	{
		/// <summary>
		/// Performs a case insensitive "Contains"
		/// </summary>
		/// <returns>
		/// true if the substring findText was found in the string, or
		/// false otherwise, or there was an error.
		/// </returns>
		public static bool ContainsInsensitive(this string str, string findText)
		{
			if (String.IsNullOrEmpty(str) || String.IsNullOrEmpty(findText))
				return false;
			return str.IndexOf(findText, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		/// <summary>
		/// Gets the Color with a name that matches the current string.
		/// Uses StringComparison.OrdinalIgnoreCase.
		/// </summary>
		/// <param name="str"></param>
		/// <returns>
		/// The Color which name matches the string, or
		/// null if no matches were found.
		/// </returns>
		public static Color? ColorFromName(this string str)
		{
			var property = typeof(Color).GetProperties().FirstOrDefault(c =>
				String.Equals(c.Name, str, StringComparison.OrdinalIgnoreCase));
			if (property == null)
				return null;
			else
				return property.GetValue(null) as Color?;
		}
	}
}
