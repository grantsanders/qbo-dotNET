﻿using System;
using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.OAuth2PlatformClient;

namespace qbo_dotNET.Logic
{
    public interface IApiHandler
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
        public List<Invoice> postedInvoiceList { get; set; }
        public Dictionary<string, Item> itemDictionary { get; set; }
        public Dictionary<string, Customer> customerDictionary { get; set; }

        public string? InitiateOAuth2() { return authorizeUrl; }

        public async System.Threading.Tasks.Task postInvoices(List<Invoice> finalInvoiceList) { }

        public async System.Threading.Tasks.Task getServiceContext() { }

        public async System.Threading.Tasks.Task<Item> updateItem(Item item) { return item; }

        public async System.Threading.Tasks.Task<Customer> updateCustomer(Customer customer) { return customer; }

        public async System.Threading.Tasks.Task getWorkingLists() { }

        public async System.Threading.Tasks.Task updateItemDictionary() { }

        public async System.Threading.Tasks.Task updateCustomerDictionary() { }

    }
}

