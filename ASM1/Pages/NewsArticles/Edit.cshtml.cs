using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ASM1.Pages.NewsArticles
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public EditModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
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

                // Check if the user is authorized to edit this article
                var userId = short.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var isAdmin = User.IsInRole("ADMIN");

                if (!isAdmin && NewsArticle.CreatedByID != userId)
                {
                    // User is neither an admin nor the creator of this article
                    return Forbid();
                }

                var categories = await _httpClient.GetFromJsonAsync<List<Category>>($"{_apiBaseUrl}/Categories");
                ViewData["CategoryID"] = new SelectList(categories, "CategoryID", "CategoryDesciption");

                var accounts = await _httpClient.GetFromJsonAsync<List<SystemAccount>>($"{_apiBaseUrl}/SystemAccounts");
                ViewData["CreatedByID"] = new SelectList(accounts, "AccountID", "AccountID");
            }
            catch (Exception ex)
            {
                // Log the exception
                return NotFound("Error retrieving the news article. Please try again later.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Verify the user is authorized to edit this article
            var userId = short.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var isAdmin = User.IsInRole("ADMIN");

            // Fetch the original article to check ownership
            try
            {
                var originalArticle = await _httpClient.GetFromJsonAsync<NewsArticle>($"{_apiBaseUrl}/NewsArticles/{NewsArticle.NewsArticleID}");
                if (originalArticle != null && !isAdmin && originalArticle.CreatedByID != userId)
                {
                    return Forbid();
                }
            }
            catch
            {
                // If we can't verify, err on the side of caution
                if (!isAdmin)
                {
                    return Forbid();
                }
            }

            // Set the updater ID
            NewsArticle.UpdatedByID = userId;
            NewsArticle.ModifiedDate = DateTime.Now;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(NewsArticle),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PutAsync(
                    $"{_apiBaseUrl}/NewsArticles/{NewsArticle.NewsArticleID}", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("./Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to update the news article. Please try again.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                ModelState.AddModelError(string.Empty, "An error occurred while updating the news article.");
            }

            try
            {
                var categories = await _httpClient.GetFromJsonAsync<List<Category>>($"{_apiBaseUrl}/Categories");
                ViewData["CategoryID"] = new SelectList(categories, "CategoryID", "CategoryDesciption", NewsArticle.CategoryID);

                var accounts = await _httpClient.GetFromJsonAsync<List<SystemAccount>>($"{_apiBaseUrl}/SystemAccounts");
                ViewData["CreatedByID"] = new SelectList(accounts, "AccountID", "AccountID", NewsArticle.CreatedByID);
            }
            catch { /* Ignore errors loading dropdowns on error page */ }

            return Page();
        }
    }
}