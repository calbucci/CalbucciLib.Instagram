using Microsoft.VisualStudio.TestTools.UnitTesting;
using CalbucciLib.Instagram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalbucciLib.Instagram.Model;

namespace CalbucciLib.Instagram.Tests
{
    [TestClass()]
    public class InstagramAuth_Tests
    {
        [TestMethod()]
        public void GetAuthorizationUrl_Test()
        {
            InstagramAuth ia = new InstagramAuth("123", "abc", "http://calbucci.com/return?a=1");

            List<Tuple<InstagramScope, string, string, string>> tests = new List
                <Tuple<InstagramScope, string, string, string>>
            {
                new Tuple<InstagramScope, string, string, string>(0, null, null,
                    "https://api.instagram.com/oauth/authorize/?response_type=code&client_id=123&redirect_uri=http%3a%2f%2fcalbucci.com%2freturn%3fa%3d1"),
                new Tuple<InstagramScope, string, string, string>(InstagramScope.Basic, null, null,
                    "https://api.instagram.com/oauth/authorize/?response_type=code&client_id=123&redirect_uri=http%3a%2f%2fcalbucci.com%2freturn%3fa%3d1&scope=basic"),
                new Tuple<InstagramScope, string, string, string>(
                    InstagramScope.Basic | InstagramScope.PublicContent | InstagramScope.Relationships, null, null,
                    "https://api.instagram.com/oauth/authorize/?response_type=code&client_id=123&redirect_uri=http%3a%2f%2fcalbucci.com%2freturn%3fa%3d1&scope=basic+public_content+relationships"),
                new Tuple<InstagramScope, string, string, string>(0, "state 1", null,
                    "https://api.instagram.com/oauth/authorize/?response_type=code&client_id=123&redirect_uri=http%3a%2f%2fcalbucci.com%2freturn%3fa%3d1&state=state+1"),
                new Tuple<InstagramScope, string, string, string>(0, null, "http://calbucci.com/alternate?b=1",
                    "https://api.instagram.com/oauth/authorize/?response_type=code&client_id=123&redirect_uri=http%3a%2f%2fcalbucci.com%2falternate%3fb%3d1"),
                new Tuple<InstagramScope, string, string, string>(InstagramScope.Basic | InstagramScope.PublicContent | InstagramScope.Relationships, "state 1", "http://calbucci.com/alternate?b=1",
                "https://api.instagram.com/oauth/authorize/?response_type=code&client_id=123&redirect_uri=http%3a%2f%2fcalbucci.com%2falternate%3fb%3d1&scope=basic+public_content+relationships&state=state+1"
                )
            };

            foreach (var t in tests)
            {
                var authUrl = ia.GetAuthorizationUrl(t.Item1, t.Item2, t.Item3);
                Assert.AreEqual(t.Item4, authUrl);
            }

        }
    }
}