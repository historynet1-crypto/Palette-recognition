namespace Palette_recognition.Models
{
    public class ArtworkApiResponse
    {
        public int artwork_id { get; set; }
        public string artistName { get; set; } = "";
        public int artworkYear { get; set; }
        public string artworkName { get; set; } = "";
        public string artworkGenre { get; set; } = "";
        public string artworkLocation { get; set; } = "";
        public string artworkDescription { get; set; } = "";
        public string artworkFilePath { get; set; } = "";
    }
}
