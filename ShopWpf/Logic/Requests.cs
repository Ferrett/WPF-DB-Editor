using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace ShopWpf.Logic
{
    public static class Requests
    {
        static string APIurl = @"https://xhvlop3q7v55snb2tvjh7dt57a0jswko.lambda-url.eu-north-1.on.aws";

        public static async Task<HttpResponseMessage> GetRequest(string tableName)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync($"{APIurl}/{tableName}/{Routes.GetRequest}");

            return response;
        }

        public static async Task<HttpResponseMessage> DeleteRequest(string tableName, int ID)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.DeleteAsync($"{APIurl}/{tableName}/{Routes.DeleteRequest}/{ID}");
            
            return response;
        }

        public static async Task<HttpResponseMessage> PostRequest(string tableName, string content, HttpContent? multipartContent = null)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync($"{APIurl}/{tableName}/{Routes.PostRequest}/{content}", multipartContent);
            
            return response;
        }

        public static async Task<HttpResponseMessage> PutRequest(string tableName, string putRequestName, int ID, string? content = null,  HttpContent? multipartContent = null)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PutAsync($"{APIurl}/{tableName}/{putRequestName}/{ID}/{content}", multipartContent);

            return response;
        }
    }
}
