using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Palette_recognition.Models;
using System.Text.Json;

public class Schilderijen_Model : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public Schilderijen_Model(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty]
    public SchilderijViewModel Input { get; set; } = new();

    public async Task OnGetAsync()
    {
        // Initialiseer de pogingen teller als deze nog niet bestaat
        if (!TempData.ContainsKey("Pogingen"))
        {
            TempData["Pogingen"] = 3;
        }

        // Haal data op van de API
        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("http://localhost:8001/data");
            
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var artworks = JsonSerializer.Deserialize<List<ArtworkApiResponse>>(jsonString, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
                
                if (artworks != null && artworks.Count > 0)
                {
                    // Selecteer random artwork uit de lijst
                    var random = new Random();
                    var selectedArtwork = artworks[random.Next(artworks.Count)];
                    
                    // Converteer API data naar SchilderijViewModel
                    Input = new SchilderijViewModel
                    {
                        Afbeelding = Path.GetFileName(selectedArtwork.artworkFilePath),
                        JuisteAntwoord = selectedArtwork.artworkName,
                        CurrentArtworkName = selectedArtwork.artworkName,
                        Hints = new List<string>
                        {
                            $"Hint 1: Het kunstwerk bevindt zich in {selectedArtwork.artworkLocation}",
                            $"Hint 2: Geschilderd door {selectedArtwork.artistName}",
                            $"Hint 3: Genre: {selectedArtwork.artworkGenre}, Jaar: {selectedArtwork.artworkYear}"
                        }
                    };
                }
                else
                {
                    // Fallback naar hardcoded data
                    Input = GetDefaultSchilderij();
                }
            }
            else
            {
                // Fallback naar hardcoded data als API niet beschikbaar is
                Input = GetDefaultSchilderij();
            }
        }
        catch (Exception)
        {
            // Fallback naar hardcoded data bij error
            Input = GetDefaultSchilderij();
        }

        TempData.Keep("Pogingen");
    }

    private SchilderijViewModel GetDefaultSchilderij()
    {
        return new SchilderijViewModel
        {
            Afbeelding = "Mona-Lisa.jpg",
            JuisteAntwoord = "Mona Lisa",
            CurrentArtworkName = "Mona Lisa",
            Hints = new List<string>
            {
                "Hint 1: Schilderij hang in de Louvre",
                "Hint 2: Geschilderd door Leonardo da Vinci",
                "Hint 3: Wereldberoemd om haar glimlach"
            }
        };
    }

    public IActionResult OnPost()
    {
        int pogingen = TempData.ContainsKey("Pogingen") ? (int)TempData["Pogingen"]! : 3;

        // Check of het antwoord correct is
        if (!string.IsNullOrWhiteSpace(Input.Antwoord) &&
            string.Equals(Input.Antwoord.Trim(), Input.JuisteAntwoord, StringComparison.OrdinalIgnoreCase))
        {
            TempData["Pogingen"] = 3;
            TempData.Remove("Hint");
            return RedirectToPage(GetEndscreenPage());
        }

        // Verminder pogingen
        pogingen = Math.Max(0, pogingen - 1);
        TempData["Pogingen"] = pogingen;

        // Toon hint - check if hints exist
        if (Input.Hints != null && Input.Hints.Count > 0)
        {
            int hintIndex = Input.Hints.Count - pogingen - 1;
            if (hintIndex >= 0 && hintIndex < Input.Hints.Count)
            {
                TempData["Hint"] = Input.Hints[hintIndex];
            }
        }

        // Als geen pogingen meer over zijn, ga naar endscreen
        if (pogingen == 0)
        {
            TempData["Pogingen"] = 3;
            TempData.Remove("Hint");
            return RedirectToPage(GetEndscreenPage());
        }

        TempData.Keep("Pogingen");
        TempData.Keep("Hint");
        return Page();
    }

    public IActionResult OnPostSkip()
    {
        int pogingen = TempData.ContainsKey("Pogingen") ? (int)TempData["Pogingen"]! : 3;

        pogingen = Math.Max(0, pogingen - 1);
        TempData["Pogingen"] = pogingen;

        // Toon hint - check if hints exist
        if (Input.Hints != null && Input.Hints.Count > 0)
        {
            int hintIndex = 2 - pogingen;
            if (hintIndex >= 0 && hintIndex < Input.Hints.Count)
            {
                TempData["Hint"] = Input.Hints[hintIndex];
            }
        }

        if (pogingen == 0)
        {
            TempData["Pogingen"] = 3;
            return RedirectToPage(GetEndscreenPage());
        }

        TempData.Keep("Pogingen");
        return Page();
    }

    private string GetEndscreenPage()
    {
        var artworkName = (Input.CurrentArtworkName ?? "").Trim();
        Console.WriteLine($"DEBUG: CurrentArtworkName = '{artworkName}' (Length: {artworkName.Length})");
        Console.WriteLine($"DEBUG: JuisteAntwoord = '{Input.JuisteAntwoord}'");
        
        // Case-insensitive vergelijking
        return artworkName.ToLower() switch
        {
            "mona lisa" => "/EndscreenMonaLisa",
            "starry night" or "de sterrennacht" => "/EndscreenStarryNight",
            "the night watch" or "de nachtwacht" => "/EndscreenTheNightWatch",
            "sunflowers" or "zonnebloemen" => "/EndscreenSunflowers",
            _ => "/EndscreenMonaLisa" // Default fallback
        };
    }
}
