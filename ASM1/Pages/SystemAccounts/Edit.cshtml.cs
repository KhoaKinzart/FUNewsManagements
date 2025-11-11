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
        public SystemAccount SystemAccount { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                SystemAccount = await _httpClient.GetFromJsonAsync<SystemAccount>($"{_apiBaseUrl}/SystemAccounts/{id}");

                if (SystemAccount == null)
                {
                    return NotFound();
                }

                return Page();
            }
            catch (Exception ex)
            {
                // Log the exception
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var response = await _httpClient.PutAsJsonAsync(
                    $"{_apiBaseUrl}/SystemAccounts/{SystemAccount.AccountID}", SystemAccount);
                response.EnsureSuccessStatusCode();

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error updating system account. Please try again later.");
                return Page();
            }
        }
    }
}