using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalbucciLib.Instagram.Model
{
    public class InstagramCaption
    {
        public DateTime CreatedTime { get; set; }

        public string Text { get; set; }

        public InstagramBaseUser From { get; set; }
        public string Id { get; set; }


        public string created_time
        {
            get { return InstagramUtils.ToEpoch(CreatedTime); }
            set { CreatedTime = InstagramUtils.FromEpoch(value); }
        }


    }
}
