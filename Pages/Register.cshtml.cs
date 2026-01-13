using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class RegisterModel : PageModel
{
    // Hier kun je later bijvoorbeeld IHttpClientFactory injecten
    // private readonly IHttpClientFactory _httpClientFactory;
    // public RegisterModel(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

    [BindProperty]
    public RegisterInput Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
        // Alleen pagina tonen
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // 1) Server-side validatie (DataAnnotations)
        if (!ModelState.IsValid)
            return Page();

        // 2) Hier komt jouw Python API call
        // TODO: stuur JSON naar Python API: POST /register
        // payload: { email, password }
        //
        // var client = _httpClientFactory.CreateClient("PythonApi");
        // var resp = await client.PostAsJsonAsync("register", new { email = Input.Email, password = Input.Password });
        // if (!resp.IsSuccessStatusCode) { ErrorMessage = "Registratie mislukt"; return Page(); }

        // Voor nu doen we alsof het gelukt is:
        await Task.CompletedTask;

        // 3) Redirect naar login na succesvolle registratie
        return RedirectToPage("/Login");
    }

    public class RegisterInput
    {
        [Required(ErrorMessage = "Email is verplicht.")]
        [EmailAddress(ErrorMessage = "Vul een geldig emailadres in.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Wachtwoord is verplicht.")]
        [MinLength(6, ErrorMessage = "Wachtwoord moet minimaal 6 tekens zijn.")]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Bevestig je wachtwoord.")]
        [Compare(nameof(Password), ErrorMessage = "Wachtwoorden komen niet overeen.")]
        public string ConfirmPassword { get; set; } = "";
    }
}

