namespace _15Puzzle.Models
{
    public class PuzzleInfo
    {
        public string Name { get; }
        public string Picture { get; }
        public int  Tiles { get; }
        public bool Landscape { get; }

        public PuzzleInfo(string name, string picture, int tiles, bool landscape)
        {
            Name = name;
            Picture = picture;
            Tiles = tiles;
            Landscape = landscape;
        }
    }
}
