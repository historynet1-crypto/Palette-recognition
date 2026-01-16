namespace Palette_recognition.Models
{
    public class SchilderijViewModel
    {
        public string Antwoord { get; set; } = "";
        public string Afbeelding { get; set; } = "";
        public string JuisteAntwoord { get; set; } = "";
        public string CurrentArtworkName { get; set; } = "";
        public List<string> Hints { get; set; } = new();
    }
}
