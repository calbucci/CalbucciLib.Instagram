using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CalbucciLib.Instagram.Model
{
    public class InstagramRelationshipStatus
    {
        [JsonProperty("outgoing_status")]
        public string OutgoingStatus { get; set; }
        [JsonProperty("incoming_status")]
        public string IncomingStatus { get; set; }
    }

    public enum InstagramRelationshipAction
    {
        Follow,

        Unfollow,

        Approve,

        Ignore
    };
}
