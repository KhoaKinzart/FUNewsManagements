using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ASM1.Pages.Reports
{
    [Authorize(Roles = "ADMIN")]
    public class NewsStatisticsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public NewsStatisticsModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration.GetValue<string>("ApiSettings:BaseUrl");
        }

        [BindProperty]
        public DateTime StartDate { get; set; } = DateTime.Today.AddDays(-30);

        [BindProperty]
        public DateTime EndDate { get; set; } = DateTime.Today;

        public List<StatisticItem> Statistics { get; set; } = new List<StatisticItem>();

        public string ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            await LoadStatistics();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (StartDate > EndDate)
            {
                ErrorMessage = "Start date cannot be later than end date.";
                return Page();
            }

            await LoadStatistics();
            return Page();
        }

        private async Task LoadStatistics()
        {
            try
            {
                var startDateStr = StartDate.ToString("yyyy-MM-dd");
                var endDateStr = EndDate.ToString("yyyy-MM-dd");
                
                var response = await _httpClient.GetStringAsync(
                    $"{_apiBaseUrl}/NewsArticles/Statistics?startDate={startDateStr}&endDate={endDateStr}");
                
                var jsonDocument = JsonDocument.Parse(response);
                Statistics = new List<StatisticItem>();
                
                foreach (var element in jsonDocument.RootElement.EnumerateArray())
                {
                    var statItem = new StatisticItem
                    {
                        Date = element.GetProperty("date").GetDateTime(),
                        DateString = element.GetProperty("dateString").GetString(),
                        TotalArticles = element.GetProperty("totalArticles").GetInt32(),
                        ActiveArticles = element.GetProperty("activeArticles").GetInt32(),
                        InactiveArticles = element.GetProperty("inactiveArticles").GetInt32()
                    };
                    Statistics.Add(statItem);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error loading statistics. Please try again later.";
                Statistics = new List<StatisticItem>();
            }
        }

        public class StatisticItem
        {
            public DateTime Date { get; set; }
            public string DateString { get; set; }
            public int TotalArticles { get; set; }
            public int ActiveArticles { get; set; }
            public int InactiveArticles { get; set; }
        }
    }
}