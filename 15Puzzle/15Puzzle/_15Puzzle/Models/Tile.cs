namespace _15Puzzle.Models
{
    public class Tile
    {
        public int Index { get; private set; }

        public int Index0X { get;  }
        public int Index0Y { get;  }
        public int IndexX { get; set; }
        public int IndexY { get; set; }

        public Tile(int index,int indexX,int indexY)
        {
            Index = index;
            Index0X =IndexX = indexX;
            Index0Y =IndexY = indexY;
        }

    }
}
