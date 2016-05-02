using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CalbucciLib.Instagram.Model
{
    public class InstagramToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        public InstagramBaseUser User { get; set; }

    }
}
