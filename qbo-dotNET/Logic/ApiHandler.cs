using System;
using System.Security.Claims;
using Intuit.Ipp.Data;
using Intuit.Ipp.Core;
using Intuit.Ipp.OAuth2PlatformClient;
using Intuit.Ipp.Security;
using Microsoft.AspNetCore.Components;
using static System.Formats.Asn1.AsnWriter;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Intuit.Ipp.DataService;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace qbo_dotNET.Logic
{
    public class ApiHandler : IApiHandler
    {

        public string? authorizeUrl { get; set; }
        public string? code { get; set; }
        public string? realmId { get; set; }
        public string? accessToken { get; set; }
        public OAuth2Client auth2Client { get; set; }
        public ServiceContext? serviceContext { get; set; }
        public DataService? service { get; set; }
        public List<Item>? workingItemList { get; set; }
        public List<Customer>? workingCustomerList { get; set; }

        public ApiHandler()
        {
            auth2Client = new OAuth2Client("ABgmNW1YLLBP9g4fl46KoKYCF5lumKaKR6vxkSdb7eycxwwvPy", "WzqNy1Mf8rb6hpsqQCVxmU2JijqAEnVWv1jgFhy3", "https://localhost:7247/oauth2redirect", "sandbox");
        }

        public string? InitiateOAuth2()
        {
            List<OidcScopes> scopes = new List<OidcScopes>();
            scopes.Add(OidcScopes.Accounting);
            authorizeUrl = auth2Client.GetAuthorizationURL(scopes);
            return authorizeUrl;
        }

        
        public async System.Threading.Tasks.Task getServiceContext()
        {
            var tokenResponse = await auth2Client.GetBearerTokenAsync(code);
            accessToken = tokenResponse.AccessToken;
            serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO);
            serviceContext.IppConfiguration.BaseUrl.Qbo = "https://sandbox-quickbooks.api.intuit.com/";
            service = new DataService(serviceContext);
            Customer customer = new();
            Item item = new();

            await System.Threading.Tasks.Task.Run(() =>
            {
                workingCustomerList = service.FindAll<Customer>(customer).ToList();
                workingItemList = service.FindAll<Item>(item).ToList();
            });
        }

        

        public async System.Threading.Tasks.Task postInvoices (IEnumerable<Invoice> finalInvoiceList)
        {

        }
    }
}

