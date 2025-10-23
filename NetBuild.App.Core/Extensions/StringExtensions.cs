using System.Globalization;
namespace NetBuild.App.Core.Extensions
{
    public static class StringExtensions
    {
        public static string GetCultureNameOrDefault(this string cultureName, string defaultValue)
        {
            try
            {
                return CultureInfo.GetCultureInfo(cultureName).Name;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }
    }
}
