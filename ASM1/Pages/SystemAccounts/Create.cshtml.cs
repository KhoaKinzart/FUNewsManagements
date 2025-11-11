using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessObjects;

namespace ASM1.Pages.SystemAccounts
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

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public SystemAccount SystemAccount { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/SystemAccounts", SystemAccount);
                response.EnsureSuccessStatusCode();

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                // Log the exception
                ModelState.AddModelError(string.Empty, "Error creating system account. Please try again later.");
                return Page();
            }
        }
    }
}