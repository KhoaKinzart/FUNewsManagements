using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ASM1.Pages.NewsArticles
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration.GetValue<string>("ApiSettings:BaseUrl");
        }

        public IList<NewsArticle> NewsArticle { get; set; } = default!;

        public async Task OnGetAsync()
        {
            try
            {
                // Lấy thông tin user từ claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                
                bool isAdmin = userRole == "ADMIN";
                
                if (isAdmin)
                {
                    // Admin thấy tất cả NewsArticle
                    NewsArticle = await _httpClient.GetFromJsonAsync<List<NewsArticle>>($"{_apiBaseUrl}/NewsArticles");
                }
                else
                {
                    // User chỉ thấy NewsArticle của mình
                    if (short.TryParse(userIdClaim, out short userId) && userId > 0)
                    {
                        NewsArticle = await _httpClient.GetFromJsonAsync<List<NewsArticle>>($"{_apiBaseUrl}/NewsArticles/ByUser/{userId}");
                    }
                    else
                    {
                        // Nếu không parse được userIdClaim (admin từ config có ID = 0)
                        NewsArticle = new List<NewsArticle>();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                NewsArticle = new List<NewsArticle>();
                ModelState.AddModelError(string.Empty, "Error retrieving news articles. Please try again later.");
            }
        }
    }
}