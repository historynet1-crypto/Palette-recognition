using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class LoginModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LoginModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

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

        try
        {
            var client = _httpClientFactory.CreateClient();
            var payload = new { email = Input.Email, password = Input.Password };
            
            var response = await client.PostAsJsonAsync("http://localhost:8002/login", payload);
            
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<LoginApiResponse>(jsonResponse, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
                
                if (!string.IsNullOrEmpty(result?.AccessToken))
                {
                    // Sla access token op in session
                    HttpContext.Session.SetString("AccessToken", result.AccessToken);
                    HttpContext.Session.SetString("UserEmail", Input.Email);
                    
                    // Redirect naar Index/Home na succesvolle login
                    return RedirectToPage("/Index");
                }
                else
                {
                    ErrorMessage = "Login mislukt. Geen token ontvangen.";
                    return Page();
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var errorResult = JsonSerializer.Deserialize<LoginErrorResponse>(errorContent, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
                
                ErrorMessage = errorResult?.Detail ?? "Ongeldige inloggegevens.";
                return Page();
            }
            else
            {
                ErrorMessage = "Login mislukt. Server gaf een fout terug.";
                return Page();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Er is een fout opgetreden: {ex.Message}";
            return Page();
        }
    }

    public class LoginInput
    {
        [Required(ErrorMessage = "Email is verplicht.")]
        [EmailAddress(ErrorMessage = "Vul een geldig emailadres in.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Wachtwoord is verplicht.")]
        public string Password { get; set; } = "";
    }

    public class LoginApiResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = "";
        
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = "";
    }

    public class LoginErrorResponse
    {
        public string Detail { get; set; } = "";
    }
}
