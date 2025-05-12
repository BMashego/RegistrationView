using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace RegistrationView.Pages.Welcome
{
    public class WelcomeModelModel : PageModel
    {
        public string Fullname { get; set; }

        public void OnGet()
        {
            var nameClaim = User.FindFirst(ClaimTypes.Name);
            Fullname = nameClaim != null ? nameClaim.Value : "User";
        }
    }
}
