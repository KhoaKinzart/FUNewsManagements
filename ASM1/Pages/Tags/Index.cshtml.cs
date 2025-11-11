using BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ASM1.Pages.Tags
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

        public IList<Tag> Tag { get; set; } = default!;

        public async Task OnGetAsync()
        {
            try
            {
                Tag = await _httpClient.GetFromJsonAsync<List<Tag>>($"{_apiBaseUrl}/Tags");
            }
            catch (Exception ex)
            {
                // Log the exception
                Tag = new List<Tag>();
                ModelState.AddModelError(string.Empty, "Error retrieving tags. Please try again later.");
            }
        }
    }
}