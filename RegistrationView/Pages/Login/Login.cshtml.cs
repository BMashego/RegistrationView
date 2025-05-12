using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using RegistrationView.Models;
using RegistrationView.Services;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace RegistrationView.Pages.Login
{
    public class LoginModel : PageModel
    {
        private readonly StoredProcedureService _storedProcedureService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IPasswordHasher<UserModel> _passwordHasher;

        public LoginModel(StoredProcedureService storedProcedureService, IAuthenticationService authenticationService, IPasswordHasher<UserModel> passwordHasher)
        {
            _storedProcedureService = storedProcedureService;
            _authenticationService = authenticationService;
            _passwordHasher = passwordHasher;
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
                new SqlParameter("@Action", "CHECK_EMAIL")
            };

            // Fetch user data from SP
            var user = await _storedProcedureService.ExecuteStoredProcedureAsync("sp_UserTable", parameters);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return Page();
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Passwords, Input.Passwords);

            if (result != PasswordVerificationResult.Success)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return Page();
            }

            // Success - create auth cookie, etc.
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
            new Claim(ClaimTypes.Name, user.Fullname),
            new Claim(ClaimTypes.Email, user.EmailAddress)
        };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await _authenticationService.SignInAsync(HttpContext, CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = true });


            // Set success message and redirect to the Welcome page
            TempData["SuccessMessage"] = $"Welcome back, {user.Fullname}!";
            return RedirectToPage("/Welcome/Welcome");
        }
    }
}
