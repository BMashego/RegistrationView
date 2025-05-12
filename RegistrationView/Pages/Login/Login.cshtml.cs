using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using RegistrationView.Models;
using RegistrationView.Services;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using static RegistrationView.Pages.RegisterModel;

namespace RegistrationView.Pages.Login
{
    public class LoginModel : PageModel
    {
        private readonly StoredProcedureService _storedProcedureService;
        private readonly IAuthenticationService _authenticationService;

        public LoginModel(StoredProcedureService storedProcedureService, IAuthenticationService authenticationService)
        {
            _storedProcedureService = storedProcedureService;
            _authenticationService = authenticationService;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string EmailAddress { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Passwords { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var parameters = new[]
            {
                new SqlParameter("@EmailAddress", Input.EmailAddress),
                new SqlParameter("@Passwords", Input.Passwords),
                new SqlParameter("@Action", "LOGIN")
            };

            // Fetch user data from SP
            var user = await _storedProcedureService.ExecuteStoredProcedureAsync("sp_UserTable", parameters);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return Page();
            }

            // Create claims for the logged-in user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim(ClaimTypes.Name, user.Fullname),
                new Claim(ClaimTypes.Email, user.EmailAddress)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await _authenticationService.SignInAsync(
                HttpContext,
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                });

            //Redirect To Welcome Page
            TempData["SuccessMessage"] = "Successful Login!";
            TempData["SuccessMessage"] = $"Welcome back, {user.Fullname}!";
            return RedirectToPage("/Welcome/Welcome");
        }
    }
}
