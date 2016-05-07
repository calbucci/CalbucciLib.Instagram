using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CalbucciLib.Instagram.Model
{
    public class InstagramBaseUser
    {
        public long Id { get; set; }
        public string Username { get; set; }
        [JsonProperty("full_name")]
        public string FullName { get; set; }
        [JsonProperty("profile_picture")]
        public string ProfilePicture { get; set; }
        public string Bio { get; set; }
        public string Website { get; set; }

        public override string ToString()
        {
            return $"{Id}:{Username}";
        }

        public override bool Equals(object obj)
        {
            var rhs = obj as InstagramBaseUser;
            if (rhs == null || rhs.Id == 0)
                return false;
            return rhs.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

    }

    public class InstagramUser : InstagramBaseUser
    {
        public InstagramUserCounts Counts { get; set; }
    }

    public class InstagramUserCounts
    {
        public int Media { get; set; }
        public int Follows { get; set; }
        [JsonProperty("followed_by")]
        public int FollowedBy { get; set; }
    }
}
