using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CalbucciLib.Instagram.Model
{
    public class InstagramResponse<T>
    {
        public InstagramResponseMeta Meta { get; set; }
        public T Data { get; set; }
        public InstagramPagination Pagination { get; set; }
    }

    public class InstagramResponseMeta
    {
        [JsonProperty("error_type")]
        public string ErrorType { get; set; }
        public int Code { get; set; }
        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }
        
    }

    public class InstagramPagination
    {
        [JsonProperty("next_url")]
        public string NextUrl { get; set; }
        [JsonProperty("next_max_id")]
        public string NextMaxId { get; set; }
    }
}
