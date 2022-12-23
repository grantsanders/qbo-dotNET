using System;
using Intuit.Ipp.OAuth2PlatformClient;

namespace qbo_dotNET.Logic
{
	public interface IApiHandler
	{
        public string authorizeUrl { get; set; }

        public OAuth2Client auth2Client { get; set; }

        public void InitiateOAuth2() { }


    }
}

