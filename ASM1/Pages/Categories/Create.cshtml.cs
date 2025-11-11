using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BusinessObjects;

namespace ASM1.Pages.Categories
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
                // If you need parent categories for a dropdown
                var categories = await _httpClient.GetFromJsonAsync<List<Category>>($"{_apiBaseUrl}/Categories");
                ViewData["ParentCategoryID"] = new SelectList(categories, "CategoryID", "CategoryName");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error loading form data. Please try again later.");
            }

            return Page();
        }

        [BindProperty]
        public Category Category { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {

            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/Categories", Category);
                response.EnsureSuccessStatusCode();

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                // Log the exception
                ModelState.AddModelError(string.Empty, "Error creating category. Please try again later.");
                return Page();
            }
        }
    }
}