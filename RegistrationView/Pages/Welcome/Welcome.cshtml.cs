using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace RegistrationView.Pages.Welcome
{
    [Authorize]
    public class WelcomeModelModel : PageModel
    {
        public string Fullname { get; set; }

        public void OnGet()
        {
            // Retrieve user's full name from claims
            Fullname = User.FindFirstValue(ClaimTypes.Name);
        }
    }
}
