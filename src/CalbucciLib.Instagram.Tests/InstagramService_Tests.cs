using Microsoft.VisualStudio.TestTools.UnitTesting;
using CalbucciLib.Instagram;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalbucciLib.Instagram.Model;

namespace CalbucciLib.Instagram.Tests
{
    [TestClass()]
    public class InstagramService_Tests
    {
        protected InstagramService Igs;
        [TestInitialize]
        public void Init()
        {
            Igs = new InstagramService(ConfigurationManager.AppSettings["InstagramAuthToken"]);
        }

        [TestMethod()]
        public void GetSelf_Test()
        {
            var self = Igs.GetSelf();
            Assert.IsNotNull(self);
            Assert.AreEqual(self.Username, "listpediatest");
        }


        [TestMethod()]
        public void GetUser_Test()
        {
            var userId = 807711; // calbucci
            var user = Igs.GetUser(userId);
            Assert.IsNotNull(user);
            Assert.AreEqual("calbucci", user.Username);
            Assert.AreEqual("Marcelo Calbucci", user.FullName);
            Assert.AreEqual(userId, user.Id);
            Assert.IsNotNull(user.Bio);
            Assert.IsNotNull(user.Counts);
            Assert.IsNotNull(user.ProfilePicture);
            Assert.IsTrue(Uri.IsWellFormedUriString(user.ProfilePicture, UriKind.Absolute));
            Assert.IsNotNull(user.Website);

            Assert.IsTrue(user.Counts.FollowedBy > 100);
            Assert.IsTrue(user.Counts.Follows > 100);
            Assert.IsTrue(user.Counts.Media > 100);

        }

        [TestMethod()]
        public void GetMostRecentMedia_Test()
        {
            var rms = Igs.GetMostRecentMedia(807711);
            Assert.IsTrue(rms != null && rms.Count > 0);
            var rm = rms[0];
            AssertMedia(rm);
        }

        protected void AssertMedia(InstagramMedia m)
        {
            Assert.IsNotNull(m.Comments);
            Assert.IsTrue(m.CreatedTime > DateTime.MinValue);
            if (m.Caption != null)
            {
                Assert.IsNotNull(m.Caption.Text);
                Assert.IsNotNull(m.Caption.From);
                Assert.AreEqual(m.User.Username, m.Caption.From.Username);
                Assert.IsTrue(m.Caption.CreatedTime > DateTime.MinValue);
            }
            Assert.IsNotNull(m.Id);
            Assert.IsNotNull(m.Images);
            AssertImage(m.Images.LowResolution);
            AssertImage(m.Images.StandardResolution);
            AssertImage(m.Images.Thumbnail);
            Assert.IsNotNull(m.Link);
            Assert.IsNotNull(m.Type);
            Assert.IsNotNull(m.User);
            Assert.IsNotNull(m.User.Username);
        }

        protected void AssertImage(InstagramImage im)
        {
            Assert.IsNotNull(im);
            Assert.IsNotNull(im.Url);
            Assert.IsTrue(Uri.IsWellFormedUriString(im.Url, UriKind.Absolute));
            Assert.IsTrue(im.Height > 0);
            Assert.IsTrue(im.Width > 0);
        }

        [TestMethod()]
        public void ListMostRecentLiked_Test()
        {
            var rms = Igs.ListMostRecentLiked();
            Assert.IsTrue(rms != null && rms.Count > 0);
            var rm = rms[0];
            Assert.IsNotNull(rm);
        }

        [TestMethod()]
        public void SearchUser_Test()
        {
            var users = Igs.SearchUser("listpedia");
            Assert.IsNotNull(users);
            Assert.IsTrue(users.Count > 0);
        }

        [TestMethod()]
        public void LookupUserId_Test()
        {
            List<Tuple<string, long>> tests = new List<Tuple<string, long>>
            {
                new Tuple<string, long>("calbucci", 807711),
                new Tuple<string, long>("_karen", 835240),
                new Tuple<string, long>("listpediatest", 3195546986),
                new Tuple<string, long>("listpedia", 3062081591),
                new Tuple<string, long>("hyperNotFound77539WhyNot", 0)
            };

            foreach (var test in tests)
            {
                var id = Igs.LookupUserId(test.Item1);
                Assert.AreEqual(test.Item2, id);
            }
        }


        [TestMethod()]
        public void ListFollows_Test()
        {
            // Just to guarantee it
            Igs.Unfollow(3062081591); // @listpedia
            var rel = Igs.GetRelationship(3062081591);
            Assert.AreEqual(rel.OutgoingStatus, "none");

            var fr = Igs.Follow(3062081591);
            Assert.AreEqual(fr.OutgoingStatus, "follows");

            rel = Igs.GetRelationship(3062081591);
            Assert.AreEqual(rel.OutgoingStatus, "follows");

            // Clean up
            Igs.Unfollow(3062081591);

        }

        [TestMethod()]
        public void ListFollowedBy_Test()
        {
            var users = Igs.ListFollowedBy();
            Assert.IsTrue(users != null && users.Count > 0);
        }

        //[TestMethod()]
        //public void ListRequestedBy_Test()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void GetRelationship_Test()
        //{
        //}

        //[TestMethod()]
        //public void SetRelationship_Test()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void Follow_Test()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void Unfollow_Test()
        //{
        //    Assert.Fail();
        //}

        [TestMethod()]
        public void GetMedia_Test()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetMediaByShortCode_Test()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SearchMedia_Test()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ListComments_Test()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PostComment_Test()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DeleteComment_Test()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ListLikes_Test()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void LikeMedia_Test()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void UnlikeMedia_Test()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetTagInformation_Test()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ListRecentTagged_Test()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SearchTags_Test()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetLocation_Test()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ListMediaByLocation_Test()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SearchLocation_Test()
        {
            Assert.Fail();
        }

    }
}