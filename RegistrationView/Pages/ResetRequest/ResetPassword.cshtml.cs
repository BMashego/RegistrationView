using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using RegistrationView.Models;
using RegistrationView.Services;
using System.ComponentModel.DataAnnotations;

namespace RegistrationView.Pages.ResetRequest
{
    public class ResetPasswordModel : PageModel
    {
        private readonly StoredProcedureService _storedProcedureService;
        private readonly IPasswordHasher<UserModel> _passwordHasher;

        public ResetPasswordModel(StoredProcedureService storedProcedureService, IPasswordHasher<UserModel> passwordHasher)
        {
            _storedProcedureService = storedProcedureService;
            _passwordHasher = passwordHasher;
        }

        [BindProperty]
        public ResetInputModel Input { get; set; }

        public class ResetInputModel
        {
            [Required]
            [EmailAddress]
            public string EmailAddress { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Passwords { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("Passwords", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Check if user exists by Email Address
            var parameters = new[]
            {
                new SqlParameter("@EmailAddress", Input.EmailAddress),
                new SqlParameter("@Action", "CHECK_EMAIL")
            };

            var user = await _storedProcedureService.ExecuteStoredProcedureAsync("sp_UserTable", parameters);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found with this email address.";
                return Page();
            }

            // Hash the new password using PasswordHasher
            user.Passwords = _passwordHasher.HashPassword(user, Input.Passwords);

            // Update the user's password in the database
            var updateParams = new[]
            {
                new SqlParameter("@EmailAddress", Input.EmailAddress),
                new SqlParameter("@Passwords", user.Passwords),
                new SqlParameter("@Action", "RESET")
            };

            await _storedProcedureService.ExecuteStoredProcedureAsync("sp_UserTable", updateParams);

            TempData["SuccessMessage"] = "Password has been reset successfully. You can now log in.";
            return RedirectToPage("/Login/Login");
        }
    }
}
