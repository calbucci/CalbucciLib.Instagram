using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using CalbucciLib.Instagram.Model;
using Newtonsoft.Json;

namespace CalbucciLib.Instagram
{
    public class InstagramService
    {
        public InstagramResponseMeta LastResponseMeta { get; private set; }

        public HttpStatusCode LastStatusCode { get; private set; }

        public InstagramPagination LastPagination { get; private set; }
        public string LastDataJson { get; private set; }
        public object LastData { get; private set; }

        

        // ====================================================================
        //
        //   Constructor
        //
        // ====================================================================
        public InstagramService(string authToken)
        {
            AuthToken = authToken;
        }


        // ====================================================================
        //
        //  Users
        //  https://www.instagram.com/developer/endpoints/users/
        //
        // ====================================================================
        public InstagramUser GetSelf()
        {
            // /users/self
            return ExecuteGet<InstagramUser>($"/users/self");
        }

        public InstagramUser GetUser(long userId)
        {
            // users/user-id
            return ExecuteGet<InstagramUser>($"/users/{userId}");
        }

        public List<InstagramMedia> GetMostRecentSelfMedia(int count = 10, string minId = null,
            string maxId = null)
        {
            var qs = BuildCountMinMax(count, minId, maxId);
            return ExecuteGet<List<InstagramMedia>>($"/users/self/media/recent", qs);
        }

        public List<InstagramMedia> GetMostRecentMedia(long userId, int count = 10, string minId = null, string maxId = null)
        {
            // /users/self/media/recent
            // /users/user-id/media/recent
            var qs = BuildCountMinMax(count, minId, maxId);
            return ExecuteGet<List<InstagramMedia>>($"/users/{userId}/media/recent", qs);
        }


        public List<InstagramMedia> ListMostRecentLiked(int count = 10, string maxLikeId = null)
        {
            // /users/self/media/liked
            var qs = new Dictionary<string, object>();
            qs["count"] = count;
            if (maxLikeId != null)
                qs["max_like_id"] = maxLikeId;
            return ExecuteGet<List<InstagramMedia>>("/users/self/media/liked", qs);
        }

        public List<InstagramBaseUser> SearchUser(string q, int count = 10)
        {
            // GET/users/search
            var qs = new Dictionary<string, object>();
            qs["q"] = q;
            qs["count"] = count;
            return ExecuteGet<List<InstagramBaseUser>>("/users/search", qs);
        }

        public class Lookup_User
        {
            public InstagramBaseUser User { get; set; }

        };

        public long LookupUserId(string username)
        {
            if(!InstagramUtils.IsValidUsername(username))
                return 0;

            try
            {
                using (var wc = new WebClient())
                {
                    // Method 1: Use the https://www.instagram.com/{username}/?__a=1
                    {
                        string url = $"https://www.instagram.com/{username}/?__a=1";
                        string json = wc.DownloadString(url);
                        try
                        {
                            var lu = JsonConvert.DeserializeObject<Lookup_User>(json);
                            if (lu.User.Id > 0)
                                return lu.User.Id;
                        }
                        catch { }
                    }


                    // Method 2: Scrape it from the page
                    {
                        string url = $"https://www.instagram.com/{username}/";
                        string page = wc.DownloadString(url);
                        foreach (var idMark in new[] {"\"owner\": {\"id\":", "profilePage_"})
                        {
                            int pos = page.IndexOf(idMark);
                            if (pos != -1)
                            {
                                pos += idMark.Length;
                                // Find the end of value marker
                                int pos2 = page.IndexOfAny(new[] {',', '}'}, pos + 1);
                                string ids = page.Substring(pos, pos2 - pos).Trim(' ', '\t', '\r', '\n', '\'', '\"');
                                long id = 0;
                                if(long.TryParse(ids, out id) && id > 0)
                                    return id;
                            }
                        }
                    }

                    return 0;
                }
            }
            catch (WebException wex)
            {
                var resp = (HttpWebResponse) wex.Response;
                if (resp.StatusCode == HttpStatusCode.NotFound)
                    return 0;

                throw wex;
            }
        }



        // ====================================================================
        //
        //  Relationships
        //
        // ====================================================================
        /// <summary>
        /// Get the list of users this user follows
        /// </summary>
        public List<InstagramBaseUser> ListFollows(int maxResults = 500)
        {
            // /users/self/follows
            return ExecuteGetWithPagination<InstagramBaseUser>("/users/self/follows", maxResults);
        }


        /// <summary>
        /// Get the list of users this user is followed by.
        /// </summary>
        public List<InstagramBaseUser> ListFollowedBy(int maxResults = 500)
        {
            // /users/self/followed-by
            return ExecuteGetWithPagination<InstagramBaseUser>("/users/self/followed-by", maxResults);
        }

        /// <summary>
        /// List the users who have requested this user's permission to follow.
        /// </summary>
        public List<InstagramBaseUser> ListRequestedBy(int maxResults = 100)
        {
            // /users/self/requested-by
            return ExecuteGetWithPagination<InstagramBaseUser>("/users/self/requested-by", maxResults);
        }

        /// <summary>
        /// Get information about a relationship to another user
        /// </summary>
        public InstagramRelationshipStatus GetRelationship(long userId)
        {
            // /users/user-id/relationship
            return ExecuteGet<InstagramRelationshipStatus>($"/users/{userId}/relationship");
        }

        /// <summary>
        /// Modify the relationship between the current user and the target user. 
        /// </summary>
        public InstagramRelationshipStatus SetRelationship(long userId, InstagramRelationshipAction action)
        {
            // /users/user-id/relationship
            return ExecutePost<InstagramRelationshipStatus>($"/users/{userId}/relationship",
                new Dictionary<string, object> { { "action", action.ToString().ToLower() } });
        }

        public InstagramRelationshipStatus Follow(long userId)
        {
            return SetRelationship(userId, InstagramRelationshipAction.Follow);
        }

        public InstagramRelationshipStatus Unfollow(long userId)
        {
            return SetRelationship(userId, InstagramRelationshipAction.Unfollow);
        }


        // ====================================================================
        //
        //  Media
        //
        // ====================================================================
        /// <summary>
        /// Get information about a media object
        /// </summary>
        public InstagramMedia GetMedia(string mediaId)
        {
            // /media/media-id
            return ExecuteGet<InstagramMedia>($"/media/{mediaId}");
        }

        public InstagramMedia GetMediaByShortCode(string shortCode)
        {
            // /media/shortcode/shortcode
            return ExecuteGet<InstagramMedia>($"/media/shortcode/{shortCode}");
        }

        /// <summary>
        /// Search for recent media in a given area.
        /// </summary>
        public List<InstagramMedia> SearchMedia(double lat, double lng, int? distanceInMeters = null)
        {
            // /media/search
            var qs = new Dictionary<string, object>();
            qs["lat"] = lat;
            qs["lng"] = lng;
            if (distanceInMeters != null)
                qs["distance"] = distanceInMeters.Value;

            return ExecuteGet<List<InstagramMedia>>("/media/search", qs);
        }


        // ====================================================================
        //
        //  Comments
        //
        // ====================================================================
        /// <summary>
        /// Get a list of recent comments on a media object. 
        /// </summary>
        public List<InstagramComment> ListComments(string mediaId, int maxResults = 100)
        {
            // /media/media-id/comments
            return ExecuteGetWithPagination<InstagramComment>($"/media/{mediaId}/comments", maxResults);
        }

        /// <summary>
        /// Create a comment on a media object
        /// </summary>
        public bool PostComment(string mediaId, string text)
        {
            // /media/media-id/comments
            var post = new Dictionary<string, object>();
            post["text"] = text;
            ExecutePost<string>($"/media/{mediaId}/comments", post);

            return LastResponseMeta != null && LastResponseMeta.Code == 200;
        }

        /// <summary>
        /// Remove a comment either on the authenticated user's media object or authored by the authenticated user.
        /// </summary>
        public bool DeleteComment(string mediaId, string commentId)
        {
            // /media/media-id/comments/comment-id
            ExecuteDelete<string>($"/media/{mediaId}/comments/{commentId}");
            return LastResponseMeta != null && LastResponseMeta.Code == 200;
        }


        // ====================================================================
        //
        //  Likes
        //
        // ====================================================================
        /// <summary>
        /// Get a list of users who have liked this media.
        /// </summary>
        public List<InstagramBaseUser> ListLikes(string mediaId, int maxResults = 500)
        {
            // /media/media-id/likes
            return ExecuteGetWithPagination<InstagramBaseUser>($"/media/{mediaId}/likes", maxResults);
        }


        /// <summary>
        /// Like a media
        /// </summary>
        public bool LikeMedia(string mediaId)
        {
            // /media/media-id/likes
            ExecutePost<string>($"/media/{mediaId}/likes", null);
            return LastResponseMeta != null && LastResponseMeta.Code == 200;
        }

        /// <summary>
        /// Unlike a media
        /// </summary>
        public bool UnlikeMedia(string mediaId)
        {
            // /media/media-id/likes
            ExecuteDelete<string>($"/media/{mediaId}/likes");
            return LastResponseMeta != null && LastResponseMeta.Code == 200;
        }




        // ====================================================================
        //
        //  Tags
        //
        // ====================================================================
        /// <summary>
        /// Get information about a tag object.
        /// </summary>
        public InstagramTagInformation GetTagInformation(string tag)
        {
            // /tags/tag-name
            return ExecuteGet<InstagramTagInformation>($"/tags/{tag}");
        }

        /// <summary>
        /// Get a list of recently tagged media.
        /// </summary>
        public List<InstagramMedia> ListRecentTagged(string tag, int count = 10, string minTagId = null,
            string maxTagId = null)
        {
            // /tags/tag-name/media/recent
            var qs = new Dictionary<string, object>();
            qs["count"] = count;
            if (minTagId != null)
                qs["min_tag_id"] = minTagId;
            if (maxTagId != null)
                qs["max_tag_id"] = maxTagId;
            return ExecuteGet<List<InstagramMedia>>($"/tags/{tag}/media/recent", qs);
        }

        /// <summary>
        /// Search for tags by name.
        /// </summary>
        public List<InstagramTagInformation> SearchTags(string q)
        {
            // /tags/search
            var qs = new Dictionary<string, object>();
            qs["q"] = q;
            return ExecuteGet<List<InstagramTagInformation>>("/tags/search", qs);
        }


        // ====================================================================
        //
        //  Locations
        //
        // ====================================================================
        /// <summary>
        /// Get information about a location.
        /// </summary>
        public InstagramLocation GetLocation(string locationId)
        {
            // /locations/location-id
            return ExecuteGet<InstagramLocation>($"/locations/{locationId}");
        }

        /// <summary>
        /// Get a list of recent media objects from a given location.
        /// </summary>
        public List<InstagramMedia> ListMediaByLocation(string locationId, string minId = null, string maxId = null)
        {
            // /locations/location-id/media/recent
            var qs = BuildCountMinMax(null, minId, maxId);
            return ExecuteGet<List<InstagramMedia>>($"/locations/{locationId}/media/recent", qs);
        }

        /// <summary>
        /// Search for a location by geographic coordinate.
        /// </summary>
        public List<InstagramLocation> SearchLocation(double? lat, double? lng, string facebookPlacesId = null, int? distanceInMeters = null)
        {
            var qs = new Dictionary<string, object>();
            if (lat.HasValue)
                qs["lat"] = lat.Value;
            if (lng.HasValue)
                qs["lng"] = lng.Value;
            if (facebookPlacesId != null)
                qs["facebook_places_id"] = facebookPlacesId;
            if (distanceInMeters != null)
                qs["distance"] = distanceInMeters.Value;

            return ExecuteGet<List<InstagramLocation>>("/locations/search", qs);
        }

        /// <summary>
        /// Search for a location by geographic coordinate.
        /// </summary>
        public List<InstagramLocation> SearchLocation(double lat, double lng, int? distance = null)
        {
            return SearchLocation(lat, lng, null, distance);
        }
        /// <summary>
        /// Search for a location by geographic coordinate.
        /// </summary>
        public List<InstagramLocation> SearchLocation(string facebookPlacesId, int? distance = null)
        {
            return SearchLocation(null, null, facebookPlacesId, distance);
        }


        // ====================================================================
        //
        //  API Calls
        //
        // ====================================================================
        protected string BuildUrl(string endpoint, Dictionary<string, object> qs)
        {
            StringBuilder sb = new StringBuilder(120);
            sb.Append("https://api.instagram.com/v1");
            sb.Append(endpoint);
            sb.Append("?access_token=");
            sb.Append(HttpUtility.UrlEncode(AuthToken));
            if (qs != null && qs.Count > 0)
            {
                sb.Append('&');
                sb.Append(BuildFormEncoded(qs));
            }
            return sb.ToString();
        }

        protected string BuildFormEncoded(Dictionary<string, object> qs)
        {
            if (qs == null || qs.Count == 0)
                return "";

            StringBuilder sb = new StringBuilder(50 * qs.Count);

            foreach (var kp in qs)
            {
                if (sb.Length > 0)
                    sb.Append('&');
                sb.AppendFormat("{0}={1}", kp.Key, HttpUtility.UrlEncode(kp.Value?.ToString() ?? ""));
            }
            return sb.ToString();
        }

        protected static Dictionary<string, object> BuildCountMinMax(int? count, string minId, string maxId)
        {
            var qs = new Dictionary<string, object>();
            if(count.HasValue)
                qs["count"] = count.Value;
            if (minId != null)
                qs["min_id"] = minId;
            if (maxId != null)
                qs["max_id"] = maxId;
            return qs;
        }


        protected T ParseResponse<T>(string json)
        {
            LastDataJson = json;
            var ir = JsonConvert.DeserializeObject<InstagramResponse<T>>(json);
            LastResponseMeta = ir.Meta;
            if (ir.Meta == null)
                LastStatusCode = HttpStatusCode.InternalServerError;
            else
                LastStatusCode = (HttpStatusCode)ir.Meta.Code;
            LastPagination = ir.Pagination;
            LastData = ir.Data;
            return ir.Data;
        }

        protected void ClearLasts()
        {
            LastData = null;
            LastDataJson = null;
            LastPagination = null;
            LastResponseMeta = null;
            LastStatusCode = HttpStatusCode.InternalServerError;
        }

        protected T ExecuteGetByUrl<T>(string url) where T : class
        {
            ClearLasts();
            try
            {
                using (var wc = new WebClient())
                {
                    var resp = wc.DownloadString(url);
                    return ParseResponse<T>(resp);
                }
            }
            catch (WebException wex)
            {
                LastStatusCode = ((HttpWebResponse)wex.Response).StatusCode;
                return null;
            }
        }

        protected List<T> ExecuteGetWithPagination<T>(string endpoint, int maxResults, Dictionary<string, object> qs = null)
            where T : class
        {
            var results = ExecuteGet<List<T>>(endpoint, qs);
            if (results.Count >= maxResults)
                return results.GetRange(0, maxResults);

            do
            {
                if (LastPagination == null || LastPagination.NextUrl == null)
                    break;

                var batch = ExecuteGetByUrl<List<T>>(LastPagination.NextUrl);
                if (batch == null || batch.Count == 0)
                    break;

                results.AddRange(batch);
            } while (results.Count < maxResults);

            if (results.Count >= maxResults)
                results = results.GetRange(0, maxResults);
            return results;

        }

        protected T ExecuteGet<T>(string endpoint, Dictionary<string, object> qs = null) where T : class
        {
            var url = BuildUrl(endpoint, qs);
            return ExecuteGetByUrl<T>(url);
        }

        protected T ExecutePost<T>(string endpoint, Dictionary<string, object> form) where T : class
        {
            ClearLasts();
            var url = BuildUrl(endpoint, null);
            try
            {
                using (var wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    string postData = BuildFormEncoded(form);

                    string resp = wc.UploadString(url, postData);
                    return ParseResponse<T>(resp);
                }
            }
            catch (WebException wex)
            {
                LastStatusCode = ((HttpWebResponse)wex.Response).StatusCode;
                return null;
            }
        }


        protected T ExecuteDelete<T>(string endpoint, Dictionary<string, object> qs = null) where T : class
        {
            ClearLasts();
            var url = BuildUrl(endpoint, qs);
            try
            {
                using (var wc = new WebClient())
                {
                    string resp = wc.UploadString(url, "DELETE", "");
                    return ParseResponse<T>(resp);
                }
            }
            catch (WebException wex)
            {
                LastStatusCode = ((HttpWebResponse)wex.Response).StatusCode;
                return null;
            }
        }


        // ====================================================================
        //
        //  Properties
        //
        // ====================================================================
        public string AuthToken { get; set; }

        public bool DidReachRateLimit => (int) LastStatusCode == 429 || LastResponseMeta?.ErrorType == "OAuthRateLimitException";
        
        
    }
}
