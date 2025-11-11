using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessObjects;

namespace ASM1.Pages.Categories
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public DetailsModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration.GetValue<string>("ApiSettings:BaseUrl");
        }

        public Category Category { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                Category = await _httpClient.GetFromJsonAsync<Category>($"{_apiBaseUrl}/Categories/{id}");

                if (Category == null)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                return NotFound("Error retrieving the category. Please try again later.");
            }

            return Page();
        }
    }
}