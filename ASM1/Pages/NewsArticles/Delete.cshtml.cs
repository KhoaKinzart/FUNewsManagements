using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASM1.Pages.NewsArticles
{
    public class DeleteModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public DeleteModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration.GetValue<string>("ApiSettings:BaseUrl");
        }

        [BindProperty]
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

                // Check if the user is authorized to delete this article
                var userId = short.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var isAdmin = User.IsInRole("ADMIN");

                if (!isAdmin && NewsArticle.CreatedByID != userId)
                {
                    // User is neither an admin nor the creator of this article
                    return Forbid();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                return NotFound("Error retrieving the news article. Please try again later.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                // Verify authorization again
                var article = await _httpClient.GetFromJsonAsync<NewsArticle>($"{_apiBaseUrl}/NewsArticles/{id}");

                if (article == null)
                {
                    return NotFound();
                }

                var userId = short.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var isAdmin = User.IsInRole("ADMIN");

                if (!isAdmin && article.CreatedByID != userId)
                {
                    // User is neither an admin nor the creator of this article
                    return Forbid();
                }

                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/NewsArticles/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Failed to delete the news article.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                ModelState.AddModelError(string.Empty, "An error occurred while deleting the news article.");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}