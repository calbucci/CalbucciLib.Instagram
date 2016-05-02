using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalbucciLib.Instagram.Model
{
    public class InstagramComment
    {
        public DateTime CreatedTime { get; set; }
        public string Text { get; set; }
        public InstagramBaseUser From { get; set; }
        public string Id { get; set; }

        
    }
}
