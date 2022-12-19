using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Intuit.Ipp.OAuth2PlatformClient;
using Microsoft.Extensions.Hosting;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace qbo_dotNET.Controllers
{
    public class InvoicingController : Controller
    {

        public static OAuth2Client auth2Client = new OAuth2Client(clientid, clientsecret, redirectUrl, environment);

        public IActionResult Index()
        {
            return ViewComponent("Index");
        }

        public IActionResult InitiateAuth(string submitButton)
        {

            List<OidcScopes> scopes = new List<OidcScopes>();
            scopes.Add(OidcScopes.Accounting);
            string authorizeUrl = auth2Client.GetAuthorizationURL(scopes);
            return Redirect(authorizeUrl);
        }
    }
}

