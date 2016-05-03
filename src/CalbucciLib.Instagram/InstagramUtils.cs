using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalbucciLib.Instagram
{
    public static class InstagramUtils
    {
        private static DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

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
            return true;
        }
        
    }
}
