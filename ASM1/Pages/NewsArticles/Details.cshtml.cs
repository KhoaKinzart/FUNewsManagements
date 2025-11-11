using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessObjects;

namespace ASM1.Pages.NewsArticles
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

        public NewsArticle NewsArticle { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                NewsArticle = await _httpClient.GetFromJsonAsync<NewsArticle>($"{_apiBaseUrl}/NewsArticles/{id}");

                if (NewsArticle == null)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                return NotFound("Error retrieving the news article. Please try again later.");
            }

            return Page();
        }
    }
}