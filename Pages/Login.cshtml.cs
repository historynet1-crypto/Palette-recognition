using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


public class LoginModel : PageModel
{
    [BindProperty]
    public LoginInput Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
        // Alleen pagina tonen
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        // TODO: hier komt jouw Python API call: POST /login
        // payload: { email, password }
        //
        // Verwachte response kan bijv zijn:
        // - success true/false
        // - of token
        // - of userId
        //
        // Als success:
        // - redirect naar Home
        // Als fail:
        // - ErrorMessage tonen

        await Task.CompletedTask;

        // Voor nu doen we alsof login altijd faalt:
        ErrorMessage = "Login nog niet gekoppeld aan API.";
        return Page();
    }

    public class LoginInput
    {
        [Required(ErrorMessage = "Email is verplicht.")]
        [EmailAddress(ErrorMessage = "Vul een geldig emailadres in.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Wachtwoord is verplicht.")]
        public string Password { get; set; } = "";
    }
}
