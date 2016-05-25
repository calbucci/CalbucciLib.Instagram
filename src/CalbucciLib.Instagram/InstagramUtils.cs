using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CalbucciLib.Instagram
{
    public static class InstagramUtils
    {
        private static DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static string[] InstagramLinks = new string[]
        {
            "https://www.instagram.com",
            "http://instagram.com",
            "https://instagram.com",
            "http://www.instagram.com"
        };

        private static HashSet<string> _NotValidAccounts;

        static InstagramUtils()
        {
            _NotValidAccounts = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
            {
                "developer",
                "explore",
                "accounts",
                "emails",
                "about",
                "press",
                "legal",
                "p" 
            };
        }

        public static DateTime FromEpoch(string epoch)
        {
            if (string.IsNullOrWhiteSpace(epoch))
                return DateTime.MinValue;

            long l;
            if (!long.TryParse(epoch, out l))
                return DateTime.MinValue;

            return Epoch.AddSeconds(l);
        }

        public static string ToEpoch(DateTime dateTime)
        {
            var seconds = (long)(dateTime - Epoch).TotalSeconds;

            return seconds.ToString();
        }

        public static bool IsValidUsername(string username)
        {
            if (username == null)
                return false;

            if (username.Length == 0 || username.Length > 30)
                return false;

            foreach (var c in username)
            {
                if (char.IsLetterOrDigit(c))
                    continue;
                if (c == '_' || c == '.')
                    continue;
                return false;
            }

            if (_NotValidAccounts.Contains(username))
                return false;
            return true;
        }

        public static bool IsInstagramLink(string link)
        {
            if (string.IsNullOrWhiteSpace(link))
                return false;

            foreach (var prefix in InstagramLinks)
            {
                if (link.StartsWith(prefix, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        public static string ExtractUsername(string linkOrScreenname)
        {
            if (string.IsNullOrWhiteSpace(linkOrScreenname))
                return null;

            linkOrScreenname = linkOrScreenname.Trim();
            if (linkOrScreenname.StartsWith("@"))
                linkOrScreenname = linkOrScreenname.Substring(1);

            string username = linkOrScreenname;
            if (IsValidUsername(username))
                return username;

            if (linkOrScreenname.Length > 16)
            {
                // It's probably a link
                if (!IsInstagramLink(linkOrScreenname))
                    return null;

                try
                {
                    Uri link;
                    if (!Uri.TryCreate(linkOrScreenname, UriKind.Absolute, out link))
                        return null;

                    if (link.Segments.Length == 0)
                        return null;

                    username = link.Segments[1];
                    if (string.IsNullOrEmpty(username))
                        return null;

                    if (username.EndsWith("/"))
                        username = username.Substring(0, username.Length - 1);

                }
                catch (Exception)
                {
                    return null;
                }
            }

            if (!IsValidUsername(username))
                return null;

            return username;
        }
    }
}
