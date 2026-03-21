using Microsoft.AspNetCore.Http;

namespace doanweb.Helpers
{
    public static class SessionExtensions
    {
        /// <summary>
        /// Get string value from session safely
        /// </summary>
        public static string GetSessionString(this ISession session, string key)
        {
            if (session.TryGetValue(key, out byte[] value))
            {
                return System.Text.Encoding.UTF8.GetString(value);
            }
            return null;
        }

        /// <summary>
        /// Set string value to session
        /// </summary>
        public static void SetSessionString(this ISession session, string key, string value)
        {
            session.SetString(key, value ?? "");
        }
    }
}
