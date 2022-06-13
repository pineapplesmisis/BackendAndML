using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MCH.API.Models;
using MCH.Data.Entities;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger _logger;

        public MlApi(string apiUrl, ILogger logger)
        {
            _apiUrl = apiUrl;
            _client = new();
            _logger = logger;
        }

        public async Task<ProductIds> getProductIdsByQuery(string query, int count)
        {
            ProductIds ids = new();
            var url = $"{_apiUrl}searchProducts/{query}/{count}";
            try
            {
                var response = await _client.GetAsync(new Uri(url));
                ids = JsonConvert.DeserializeObject<ProductIds>(await response.Content.ReadAsStringAsync());
                return ids;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error while getting products by query: {query}.Mesage:{ex.Message}");
                return null;
            }
        }

        public async Task<ProductIds> getSimularProducts(int productId, int count)
        {
            ProductIds ids = new();
            try
            {
                var url = $"{_client}simiuralProducts/{productId}/{count}";
                var response = await _client.GetAsync(new Uri(url));
                ids = JsonConvert.DeserializeObject<ProductIds>(await response.Content.ReadAsStringAsync());
                return ids;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error while getting simular products with product: {productId}. Message:{ex.Message}");
                return ids;
            }

        }
    }
}