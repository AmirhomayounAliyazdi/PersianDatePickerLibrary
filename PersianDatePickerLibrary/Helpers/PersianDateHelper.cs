using System;
using System.Globalization;

namespace PersianDatePickerLibrary.Helpers
{
    /// <summary>
    /// Provides methods to convert between Gregorian and Persian dates.
    /// </summary>
    public static class PersianDateHelper
    {
        private static readonly PersianCalendar _persianCalendar = new PersianCalendar();

        /// <summary>
        /// Converts a Persian date string (yyyy/MM/dd) to a Gregorian DateTime.
        /// </summary>
        /// <param name="persianDate">Date string in Persian format.</param>
        /// <returns>Equivalent Gregorian DateTime or null if parsing fails.</returns>
        public static DateTime? ToGregorianDate(string persianDate)
        {
            if (string.IsNullOrWhiteSpace(persianDate))
                return null;

            var parts = persianDate.Split('/');
            if (parts.Length != 3) return null;

            if (int.TryParse(parts[0], out int year) &&
                int.TryParse(parts[1], out int month) &&
                int.TryParse(parts[2], out int day))
            {
                try
                {
                    return _persianCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);
                }
                catch
                {
                    // Invalid date
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// Converts a Gregorian DateTime to a Persian date string (yyyy/MM/dd).
        /// </summary>
        /// <param name="date">Gregorian DateTime.</param>
        /// <returns>Equivalent Persian date string.</returns>
        public static string ToPersianDate(DateTime date)
        {
            return $"{_persianCalendar.GetYear(date):0000}/{_persianCalendar.GetMonth(date):00}/{_persianCalendar.GetDayOfMonth(date):00}";
        }
    }
}
