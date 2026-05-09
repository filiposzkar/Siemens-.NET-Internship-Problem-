using System;
using System.Collections.Generic;
using System.Text;
using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Models;


namespace Siemens.Internship2026.GradeBook.Repositories
{
    public class ExternalItemRepository : IItemReader
    {
        private readonly HttpClient _httpClient;
        private const string Url = "https://gist.githubusercontent.com/ArdeleanTudor/8ea407832cd9794960e0e6bbd1319f6e/raw/145b121103dd1%E2%80%A6";

        public ExternalItemRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Item>> GetAllAsync()
        {
            try
            {
                var items = await _httpClient.GetFromJsonAsync<List<Item>>(Url);
                return items ?? new List<Item>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");
                return new List<Item>();
            }
        }

        public async Task<Item?> GetByIdAsync(int id)
        {
            var items = await GetAllAsync();
            return items.FirstOrDefault(i => i.Id == id);
        }
    }
}
