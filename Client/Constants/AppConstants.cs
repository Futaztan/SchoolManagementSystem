using System.Collections.ObjectModel;
using System.Globalization;

namespace Client.Constants;

public static class AppConstants
{
    public static ReadOnlyCollection<int> Months = Enumerable.Range(1, 12).ToList().AsReadOnly();
    public static string GetMonthName(int monthNumber)
    {
        return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthNumber);
    }
}