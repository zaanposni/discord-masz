using System.Text.RegularExpressions;

namespace MASZ.Utils;

public class StringTimeSpan
{
    public static TimeSpan ParseDateRange(string input)
    {
        int weeks = 0, days = 0, hours = 0, minutes = 0;

        // Use regular expression to match components like "3w", "1d", "2h", "3m"
        foreach (Match match in Regex.Matches(input, @"(\d+w|\d+d|\d+h|\d+m)"))
        {
            int value = int.Parse(match.Value.Substring(0, match.Value.Length - 1));
            char unit = match.Value[match.Value.Length - 1];

            switch (unit)
            {
                case 'w':
                    weeks += value;
                    break;
                case 'd':
                    days += value;
                    break;
                case 'h':
                    hours += value;
                    break;
                case 'm':
                    minutes += value;
                    break;
            }
        }

        return new TimeSpan(days + (weeks * 7), hours, minutes, 0);
    }
}