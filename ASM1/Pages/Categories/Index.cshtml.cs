using BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ASM1.Pages.Categories
{
    [Authorize(Roles = "ADMIN")]
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration.GetValue<string>("ApiSettings:BaseUrl");
        }

        public IList<Category> Category { get; set; } = default!;

        public async Task OnGetAsync()
        {
            try
            {
                Category = await _httpClient.GetFromJsonAsync<List<Category>>($"{_apiBaseUrl}/Categories");
            }
            catch (Exception ex)
            {
                // Log the exception
                Category = new List<Category>();
                ModelState.AddModelError(string.Empty, "Error retrieving categories. Please try again later.");
            }
        }
    }
}