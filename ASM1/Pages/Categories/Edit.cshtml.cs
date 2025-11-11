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

namespace ASM1.Pages.Categories
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

                // If you need parent categories for a dropdown
                var categories = await _httpClient.GetFromJsonAsync<List<Category>>($"{_apiBaseUrl}/Categories");
                ViewData["ParentCategoryID"] = new SelectList(categories, "CategoryID", "CategoryName");
            }
            catch (Exception ex)
            {
                // Log the exception
                return NotFound("Error retrieving the category. Please try again later.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            try
            {
                var response = await _httpClient.PutAsJsonAsync(
                    $"{_apiBaseUrl}/Categories/{Category.CategoryID}", Category);
                response.EnsureSuccessStatusCode();

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                // Log the exception
                ModelState.AddModelError(string.Empty, "Error updating category. Please try again later.");

                // Reload parent categories dropdown on error
                try
                {
                    var categories = await _httpClient.GetFromJsonAsync<List<Category>>($"{_apiBaseUrl}/Categories");
                    ViewData["ParentCategoryID"] = new SelectList(categories, "CategoryID", "CategoryName", Category.ParentCategoryID);
                }
                catch { /* Ignore dropdown loading errors on error page */ }

                return Page();
            }
        }
    }
}