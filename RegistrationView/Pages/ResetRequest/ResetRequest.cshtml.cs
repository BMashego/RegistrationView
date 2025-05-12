using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using RegistrationView.Services;

namespace RegistrationView.Pages.ResetRequest
{
    public class ResetRequestModel : PageModel
    {
        private readonly StoredProcedureService _storedProcedureService;

        public ResetRequestModel(StoredProcedureService storedProcedureService)
        {
            _storedProcedureService = storedProcedureService;
        }

        [BindProperty]
        public string EmailAddress { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(EmailAddress))
            {
                ModelState.AddModelError(string.Empty, "Please enter an email address.");
                return Page();
            }

            // Check if user exists
            var parameters = new[]
            {
                new SqlParameter("@EmailAddress", EmailAddress),
                new SqlParameter("@Action", "CHECK_EMAIL")
            };

            var user = await _storedProcedureService.ExecuteStoredProcedureAsync("sp_UserTable", parameters);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "No user found with this email address.");
                return Page();
            }

            // Simulate sending an email (replace with actual email sending logic)
            TempData["Message"] = "Password reset link has been sent to your email address.";
            return RedirectToPage("/ResetRequest/ResetPassword");
        }
    }
}
