using System.Reflection;
using System.Web;

namespace SupervisorMobility.Client.Data.Helpers
{
    public static class QueryHelper
    {
        public static string ToQueryString(object obj)
        {
            if (obj == null)
                return string.Empty;

            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var query = string.Join("&", properties
                .Where(p => p.GetValue(obj) != null)
                .Select(p =>
                {
                    var value = p.GetValue(obj);

                    // Format DateTime
                    if (value is DateTime dt)
                    {
                        value = dt.ToString("yyyy-MM-dd"); // adjust format as needed
                    }

                    // URL encode
                    return $"{HttpUtility.UrlEncode(p.Name)}={HttpUtility.UrlEncode(value.ToString())}";
                }));

            return query;
        }
    }
}
