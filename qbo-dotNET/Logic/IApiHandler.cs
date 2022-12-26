﻿using System;
using Intuit.Ipp.OAuth2PlatformClient;

namespace qbo_dotNET.Logic
{
	public interface IApiHandler
	{
        public string? authorizeUrl { get; set; }

        public string? code { get; set; }

        public string? realmId { get; set; }

        public OAuth2Client auth2Client { get; set; }

        public string? InitiateOAuth2() { return authorizeUrl; }

        public async Task getServiceContext() {  }

    }
}

