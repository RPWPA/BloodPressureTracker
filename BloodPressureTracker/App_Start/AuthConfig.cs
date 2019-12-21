using DotNetOpenAuth.GoogleOAuth2;
using Microsoft.AspNet.Membership.OpenAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BloodPressureTracker.App_Start
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            GoogleOAuth2Client clientGoog = new GoogleOAuth2Client("480180707060-9reoqur9u16sjekgefopqkefcf4elm5a.apps.googleusercontent.com", "21zHYv6WtPWaWhOkP47vobxV");
            IDictionary<string, string> extraData = new Dictionary<string, string>();
            OpenAuth.AuthenticationClients.Add("google", () => clientGoog, extraData);
        }
    }
}