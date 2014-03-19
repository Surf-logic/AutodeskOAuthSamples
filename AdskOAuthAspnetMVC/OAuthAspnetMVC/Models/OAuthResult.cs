using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OAuthAspnetMVC.Models
{
    public class OAuthResult
    {

        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
        public string oauth_user_name { get; set; }
        public string oauth_user_guid { get; set; }
        public string scope { get; set; }
        public string oauth_problem { get; set; }
        public string oauth_error_message { get; set; }
        public string sessionHandle { get; set; }

    }
}