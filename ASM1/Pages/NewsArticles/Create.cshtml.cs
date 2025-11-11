using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BusinessObjects;
using System.Security.Claims;

namespace ASM1.Pages.NewsArticles
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public CreateModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration.GetValue<string>("ApiSettings:BaseUrl");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var categories = await _httpClient.GetFromJsonAsync<List<Category>>($"{_apiBaseUrl}/Categories");
                ViewData["CategoryID"] = new SelectList(categories, "CategoryID", "CategoryDesciption");

                var accounts = await _httpClient.GetFromJsonAsync<List<SystemAccount>>($"{_apiBaseUrl}/SystemAccounts");
                ViewData["CreatedByID"] = new SelectList(accounts, "AccountID", "AccountID");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error loading form data. Please try again later.");
            }

            return Page();
        }

        [BindProperty]
        public NewsArticle NewsArticle { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            NewsArticle.CreatedByID = short.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            NewsArticle.UpdatedByID = short.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            NewsArticle.CreatedDate = DateTime.Now;
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

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/NewsArticles", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("./Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to create the news article. Please try again.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while creating the news article.");
            }

            return Page();
        }
    }
}