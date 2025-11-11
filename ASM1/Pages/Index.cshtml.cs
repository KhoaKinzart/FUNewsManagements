using BusinessObjects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using System;
using System.Security.Claims;

namespace ASM1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ISystemAccountService _userService;
        private readonly IConfiguration _configuration;

        public IndexModel(ILogger<IndexModel> logger, ISystemAccountService userService, IConfiguration configuration)
        {
            _logger = logger;
            _userService = userService;
            _configuration = configuration;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            // Your existing index page logic here
        }

        public async Task<IActionResult> OnPostLoginAsync()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "Please enter both email and password.";
                return Page();
            }

            var adminEmail = _configuration["AdminAccount:Email"];
            var adminPassword = _configuration["AdminAccount:Password"];

            if (!string.IsNullOrEmpty(adminEmail) && !string.IsNullOrEmpty(adminPassword) &&
                Email == adminEmail && Password == adminPassword)
            {
   
                var adminClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, "0"), 
                    new Claim(ClaimTypes.Email, adminEmail),
                    new Claim(ClaimTypes.Name, "System Administrator"),
                    new Claim(ClaimTypes.Role, "ADMIN"),
                };

                var adminClaimsIdentity = new ClaimsIdentity(adminClaims, CookieAuthenticationDefaults.AuthenticationScheme);

                var adminAuthProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(2)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(adminClaimsIdentity),
                    adminAuthProperties);

                return RedirectToPage("/SystemAccounts/Index");
            }


            var user = _userService.GetSystemAccountByEmail(Email);
            if (user == null || user.AccountPassword != Password)
            {
                ErrorMessage = "Invalid email or password.";
                return Page();
            }

            string roleString = user.AccountRole switch
            {
                1 => "ADMIN",
                _ => "USER"
            };

            // Create claims for authenticated user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.AccountID.ToString()),
                new Claim(ClaimTypes.Email, user.AccountEmail),
                new Claim(ClaimTypes.Name, user.AccountName),
                new Claim(ClaimTypes.Role, roleString),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(2)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return roleString == "ADMIN"
                ? RedirectToPage("/SystemAccounts/Index")
                : RedirectToPage("/NewsArticles/Index");
        }

        public IActionResult OnGetGoogle(string returnUrl = "/")
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Page("/Index", "ClaimRole", new { returnUrl = Uri.EscapeDataString(returnUrl) })
            };

            return Challenge(properties, "Google");
        }

        [Authorize]
        public async Task<IActionResult> OnGetClaimRoleAsync(string returnUrl = "/")
        {
            var user = User;
            var accountEmail = user.FindFirst(ClaimTypes.Email)?.Value;
            var accountName = user.FindFirst(ClaimTypes.GivenName)?.Value + " " + user.FindFirst(ClaimTypes.Surname)?.Value;

            if (string.IsNullOrEmpty(accountEmail))
            {
                return RedirectToPage("/Index");
            }

            // Check if user exists or create new
            var existingUser = _userService.GetSystemAccountByEmail(accountEmail);
            if (existingUser == null)
            {
                Random random = new Random();
                short randomId = (short)random.Next(1, short.MaxValue);
                var newUser = new SystemAccount
                {
                    AccountID = randomId,
                    AccountEmail = accountEmail,
                    AccountName = accountName,
                    AccountRole = 2,
                    AccountPassword = "@1"
                };

                _userService.AddSystemAccount(newUser);
                existingUser = _userService.GetSystemAccountByEmail(accountEmail);
            }

            string roleString = existingUser.AccountRole switch
            {
                1 => "ADMIN",
                _ => "USER"
            };

            // Create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, existingUser.AccountID.ToString()),
                new Claim(ClaimTypes.Email, existingUser.AccountEmail),
                new Claim(ClaimTypes.Name, existingUser.AccountName),
                new Claim(ClaimTypes.Role, roleString),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(2)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return roleString == "ADMIN"
                ? RedirectToPage("/SystemAccounts/Index")
                : RedirectToPage("/NewsArticles/Index");
        }

        public async Task<IActionResult> OnGetLogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Index");
        }
    }
}