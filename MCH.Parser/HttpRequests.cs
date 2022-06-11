using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MCH
{
    public class HttpRequests
    {
        private HttpClient _client;

        public HttpRequests()
        {
            _client = new();
        }
        public async  Task<string> CreateRequest(Uri  url)
        {
            HttpResponseMessage response = await _client.GetAsync(url);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }

            return string.Empty;
        }

        
    }
}