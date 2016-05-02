using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CalbucciLib.Instagram.Model
{
    public class InstagramTagInformation
    {
        public string Name { get; set; }
        [JsonProperty("media_count")]
        public int MediaCount { get; set; }
        
    }
}
