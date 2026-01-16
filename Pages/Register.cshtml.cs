using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class RegisterModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public RegisterModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

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

        // 2) Stuur JSON naar Python API: POST /register
        try
        {
            var client = _httpClientFactory.CreateClient();
            var payload = new { email = Input.Email, password = Input.Password };
            
            var response = await client.PostAsJsonAsync("http://localhost:8002/register", payload);
            
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse>(jsonResponse, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
                
                if (result?.Status == "ok")
                {
                    // 3) Redirect naar login na succesvolle registratie
                    return RedirectToPage("/Login");
                }
                else
                {
                    ErrorMessage = "Registratie mislukt. Probeer het opnieuw.";
                    return Page();
                }
            }
            else
            {
                ErrorMessage = "Registratie mislukt. Server gaf een fout terug.";
                return Page();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Er is een fout opgetreden: {ex.Message}";
            return Page();
        }
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

    public class ApiResponse
    {
        public string Status { get; set; } = "";
    }
}

