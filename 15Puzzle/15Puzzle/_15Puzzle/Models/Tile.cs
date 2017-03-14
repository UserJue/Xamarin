namespace _15Puzzle.Models
{
    public class Tile
    {
        public int Index { get; private set; }

        public int IndexX { get; set; }
        public int IndexY { get; set; }

        public Tile(int index,int indexX,int indexY)
        {
            Index = index;
            IndexX = indexX;
            IndexY = indexY;
        }

    }
}
