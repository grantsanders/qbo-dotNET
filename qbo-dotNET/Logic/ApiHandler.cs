using System;
using Intuit.Ipp.Core;
using Intuit.Ipp.OAuth2PlatformClient;
using Microsoft.AspNetCore.Components;
using static System.Formats.Asn1.AsnWriter;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace qbo_dotNET.Logic
{
    public class ApiHandler : IApiHandler
    {

        public string authorizeUrl { get; set; }

        public OAuth2Client auth2Client { get; set; }

        public ApiHandler()
        {
            auth2Client = new OAuth2Client("ABgmNW1YLLBP9g4fl46KoKYCF5lumKaKR6vxkSdb7eycxwwvPy", "WzqNy1Mf8rb6hpsqQCVxmU2JijqAEnVWv1jgFhy3", "https://localhost:7247/oauth2redirect", "sandbox");

        }

        public string InitiateOAuth2()
        {
            List<OidcScopes> scopes = new List<OidcScopes>();
            scopes.Add(OidcScopes.Accounting);
            authorizeUrl = auth2Client.GetAuthorizationURL(scopes);
            return authorizeUrl;
        }


    }
}

