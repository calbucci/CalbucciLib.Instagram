using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CalbucciLib.Instagram.Model
{
    public class InstagramMedia
    {
        public string Attribution { get; set; }
        public string Type { get; set; }
        [JsonProperty("users_in_photo")]
        public List<InstagramUserInPhoto> UsersInPhoto { get; set; }
        public string Filter { get; set; }
        public string[] Tags { get; set; }
        public InstagramCount Comments { get; set; }
        public InstagramCount Likes { get; set; }
        public InstagramCaption Caption { get; set; }
        public string Link { get; set; }
        public InstagramBaseUser User { get; set; }
        public DateTime CreatedTime { get; set; }
        public InstagramMediaSet Images { get; set; }
        public InstagramMediaSet Videos { get; set; }
        public string Id { get; set; }
        public double? Distance { get; set; }
        [JsonProperty("user_has_liked")]
        public bool UserHasLiked { get; set; }

        public InstagramLocation Location { get; set; }

        public string created_time
        {
            get { return InstagramUtils.ToEpoch(CreatedTime); }
            set { CreatedTime = InstagramUtils.FromEpoch(value); }
        }


    }

    public class InstagramUserInPhoto
    {
        public InstagramBaseUser User { get; set; }
        public InstagramPosition Position { get; set; }
    }

    public class InstagramPosition
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class InstagramCount
    {
        public int Count { get; set; }
    }

    public class InstagramImage
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class InstagramMediaSet
    {
        [JsonProperty("low_resolution")]
        public InstagramImage LowResolution { get; set; }
        public InstagramImage Thumbnail { get; set; }
        [JsonProperty("standard_resolution")]
        public InstagramImage StandardResolution { get; set; }
    }
    


}
