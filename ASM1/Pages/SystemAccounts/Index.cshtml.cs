using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessObjects;
using Microsoft.AspNetCore.Authorization;

namespace ASM1.Pages.SystemAccounts
{
    [Authorize(Roles = "ADMIN")]
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration.GetValue<string>("ApiSettings:BaseUrl");
        }

        public IList<SystemAccount> SystemAccounts { get; set; } = default!;

        public async Task OnGetAsync()
        {
            try
            {
                SystemAccounts = await _httpClient.GetFromJsonAsync<List<SystemAccount>>($"{_apiBaseUrl}/SystemAccounts");
            }
            catch (Exception ex)
            {
                // Log the exception
                SystemAccounts = new List<SystemAccount>();
                ModelState.AddModelError(string.Empty, "Error retrieving system accounts. Please try again later.");
            }
        }
    }
}