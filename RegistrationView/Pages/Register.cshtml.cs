using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using RegistrationView.Functions;
using RegistrationView.Models;
using RegistrationView.Services;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace RegistrationView.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly StoredProcedureService _storedProcedureService;

        public RegisterModel(StoredProcedureService storedProcedureService)
        {
            _storedProcedureService = storedProcedureService;
        }

        [BindProperty]
        public InputModel User { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Full Name")]
            public string Fullname { get; set; }

            [Required]
            [EmailAddress]
            public string EmailAddress { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [StringLength(100, MinimumLength = 4,
                ErrorMessage = "Password must be at least 4 characters long.")]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{4,}$",
                ErrorMessage = "Password must contain at least one uppercase, one lowercase, one number, and one special character.")]
            public string Passwords { get; set; }
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Hash password (you can use a better method in real apps)
            var hashedPassword = HashPassword(User.Passwords);

            var parameters = new[]
            {
                new SqlParameter("@Fullname", User.Fullname),
                new SqlParameter("@EmailAddress", User.EmailAddress),
                new SqlParameter("@Passwords", hashedPassword),
                new SqlParameter("@Action", "ADD")
            };

            try
            {
                await _storedProcedureService.ExecuteStoredProcedureAsync("sp_UserTable", parameters);
                TempData["SuccessMessage"] = "Registration successful!";
                return RedirectToPage("/Login/Login");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Error while registering user. Please try again.");
                return Page();
            }
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
    
}
