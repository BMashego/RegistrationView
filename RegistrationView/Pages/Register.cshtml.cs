using Microsoft.AspNetCore.Identity;
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
        private readonly IPasswordHasher<UserModel> _passwordHasher;

        public RegisterModel(StoredProcedureService storedProcedureService, IPasswordHasher<UserModel> passwordHasher)
        {
            _storedProcedureService = storedProcedureService;
            _passwordHasher = passwordHasher;
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
            [StringLength(100, MinimumLength = 12,
                ErrorMessage = "Password must be at least 12 characters long.")]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{12,}$",
                ErrorMessage = "Password must contain at least one uppercase, one lowercase, one number, and one special character.")]
            public string Passwords { get; set; }
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = new UserModel
            {
                Fullname = User.Fullname,
                EmailAddress = User.EmailAddress
            };

            // Hash password
            user.Passwords = _passwordHasher.HashPassword(user, User.Passwords);

            var parameters = new[]
            {
            new SqlParameter("@Fullname", user.Fullname),
            new SqlParameter("@EmailAddress", user.EmailAddress),
            new SqlParameter("@Passwords", user.Passwords),
            new SqlParameter("@Action", "ADD")
        };

            await _storedProcedureService.ExecuteStoredProcedureAsync("sp_UserTable", parameters);

            TempData["SuccessMessage"] = "Registration successful!";
            return RedirectToPage("/Login/Login");
        }
    }
    
}
