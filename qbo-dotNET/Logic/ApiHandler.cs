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
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace qbo_dotNET.Logic
{
    public class ApiHandler : IApiHandler
    {

        public string authorizeUrl { get; set; }
        public string code { get; set; }
        public string realmId { get; set; }
        public string accessToken { get; set; }
        public string clientId { get; set; }
        public string clientSecret { get; set; }
        public OAuth2Client auth2Client { get; set; }
        public ServiceContext serviceContext { get; set; }
        public DataService service { get; set; }
        public Dictionary<string, Item> itemDictionary { get; set; }
        public Dictionary<string, Customer> customerDictionary { get; set; }
        public List<Invoice> postedInvoiceList { get; set; }
        private readonly ILogger<ApiHandler> _logger;

        public ApiHandler(ILogger<ApiHandler> logger)
        {
            var vaultUri = "https://granthum-vault.vault.azure.net/";
            var client = new SecretClient(new Uri(vaultUri), new DefaultAzureCredential());
            clientId = client.GetSecret("boldbean-dotNET-clientID").Value.Value.ToString();
            clientSecret = client.GetSecret("boldbean-dotNET-clientSecret").Value.Value.ToString();
            auth2Client = new OAuth2Client(clientId, clientSecret, "https://boldbean-dotnet.azurewebsites.net/oauth2redirect", "production");
            _logger = logger;
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
            OAuth2RequestValidator oAuth2RequestValidator = new OAuth2RequestValidator(accessToken);
            serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oAuth2RequestValidator);
            serviceContext.IppConfiguration.BaseUrl.Qbo = "https://quickbooks.api.intuit.com/";
            serviceContext.IppConfiguration.MinorVersion.Qbo = "55";
            service = new DataService(serviceContext);

            await getWorkingLists();

            _logger.LogWarning("Working lists updated");
        }

        public async System.Threading.Tasks.Task getWorkingLists()
        {
            Customer c = new();
            Item i = new();

            try
            {
                IEnumerable<Customer> customerList = service.FindAll(c).ToList();
                IEnumerable<Item> itemList = service.FindAll(i).ToList();

                foreach (Item item in itemList)
                {
                    if (item.Type == ItemTypeEnum.Service)
                    {
                        item.Active = false;
                        item.ActiveSpecified = true;
                        item.Name += " old";
                        updateItem(item);
                    }
                }

                customerDictionary = customerList.ToDictionary(c => c.DisplayName, c => c, StringComparer.OrdinalIgnoreCase);
                itemDictionary = itemList.ToDictionary(i => i.Name, i => i, StringComparer.OrdinalIgnoreCase);

            }
            catch (Exception ex) { _logger.LogError(ex.Data + " " + ex.Message); }
        }

        public async System.Threading.Tasks.Task postInvoices(List<Invoice> finalInvoiceList)
        {
            foreach (Invoice invoice in finalInvoiceList)
            {
                service.AddAsync<Invoice>(invoice);
                _logger.LogWarning("Invoice added: " + invoice.CustomerRef.name);
                postedInvoiceList.Add(invoice);

            }
        }

        public async System.Threading.Tasks.Task updateCustomerDictionary() => customerDictionary = customerDictionary = service.FindAll(new Customer()).ToList().ToDictionary(c => c.DisplayName, c => c) ?? new Dictionary<string, Customer>();

        public async System.Threading.Tasks.Task updateItemDictionary() => itemDictionary = service.FindAll(new Item()).ToList().ToDictionary(i => i.Name, i => i) ?? new Dictionary<string, Item>();

        public async System.Threading.Tasks.Task<Item> updateItem(Item item) { return await System.Threading.Tasks.Task.FromResult(service.Update<Item>(item)); }

        public async System.Threading.Tasks.Task<Customer> updateCustomer(Customer customer) { return await System.Threading.Tasks.Task.FromResult(service.Update<Customer>(customer)); }

    }
}

