using System;

namespace VGStandard.Core.Extensions;

public static class StringExtensions
{
    public static string[] SplitWithEmpty(this string str, string separator)
    {
        return str.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
    }

	public static string PascalToCamelCase(this string str)
	{
		if (string.IsNullOrWhiteSpace(str))
		{
			return str;
		}

		return char.ToLowerInvariant(str[0]) + str[1..];
	}
}
