using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AmyDaveWedding.Models
{
    public class ApiCredential
    {
        public ApiCredential(string key, string secret)
        {
            this.Key = key;
            this.Secret = secret;
        }

        public string Key { get; private set; }
        public string Secret { get; private set; }
        public string AccessToken { get; set; }
        public string AccessTokenDate { get; set; }
    }

    public class SocialUserInfo
    {
        public string FullName { get; set; }
        public string ProfileImageUrl { get; set; }
    }
}