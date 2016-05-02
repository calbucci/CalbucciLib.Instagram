using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalbucciLib.Instagram.Model
{
    [Flags]
    public enum InstagramScope
    {
        Basic = 0x01,
        PublicContent = 0x02,
        FollowerList = 0x04,
        Comments = 0x08,
        Relationships = 0x10,
        Likes = 0x20
    }
}
