using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessObjects;

namespace ASM1.Pages.Categories
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

        public async Task<IActionResult> OnPostAsync(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/Categories/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                // Log the exception
                ModelState.AddModelError(string.Empty, "Error deleting category. Please try again later.");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}