using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MCH.API.Models;
using MCH.Data.Entities;
using Newtonsoft.Json;

namespace MCH.API.ExternalServices
{
    /// <summary>
    /// Класс для работы с API
    /// ML
    /// </summary>
    public class MlApi
    {
        private readonly string _apiUrl;
        private readonly HttpClient _client;

        public MlApi(string apiUrl)
        {
            _apiUrl = apiUrl;
            _client = new();
        }

        public async Task<ProductIds> getProductIdsByQuery(string query)
        {
            var url = $"{_apiUrl}searchProducts/{query}";
            var response = await _client.GetAsync(new Uri(url));
            ProductIds ids = JsonConvert.DeserializeObject<ProductIds>(await response.Content.ReadAsStringAsync());
            return ids;
        }

    }
}