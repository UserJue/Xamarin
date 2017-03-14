using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Runtime.CompilerServices;
using _15Puzzle.Annotations;

namespace _15Puzzle.Models
{
    public class _15Puzzle : INotifyPropertyChanged
    {
        public enum GameStatus { None, WatingForPlayer, Activ, Inactiv, Stoped, Finished, Deleted }

        private Random random;

        public IList<Tile> Tiles { get; set; }

        public int Dimension { get; private set; }

        public GameStatus Status { get; set; }

        public string Picture { get; set; }

        public _15Puzzle()
        {
            Status = GameStatus.None;
            random = new Random();
            Tiles = new List<Tile>();
        }

        public bool Create(int tilesNumber,string picture=null)
        {
            Status = GameStatus.None;
            Picture = picture;
            var dimension = (int)Math.Sqrt(tilesNumber + 1);
            if (dimension* dimension != tilesNumber + 1) return false;
            Tiles.Clear();
            Dimension = dimension;
            var index = 0;
            for (var j = 0; j < dimension; j++)
                for (var i = 0;i < dimension;i++)
                {
                    if (index >= tilesNumber) break;
                    Tiles.Add(new Tile(index,i,j));
                    index++;
                }
            Shuffle();
            return true;
        }

        public void Shuffle()
        {
            for (var i = 0; i < 2; i++)
            {
                var index1 = random.Next(15);
                var index2 = random.Next(15);
                if (index1 != index2)
                {
                    var x = Tiles[index1].IndexX;
                    var y = Tiles[index1].IndexY;
                    Tiles[index1].IndexX = Tiles[index2].IndexX;
                    Tiles[index1].IndexY = Tiles[index2].IndexY;
                    Tiles[index2].IndexX = x;
                    Tiles[index2].IndexY = y;
                }
            }
        }

        public bool CheckFinished(int[][] places)
        {
            var result = true;
            var index = 0;
            for (var j = 0; j < Dimension; j++)
                for (var i = 0; i < Dimension; i++)
                {
                    result &= places[i][j] == index;
                    if ((places[i][j] >= 0) && (places[i][j] < Tiles.Count))
                    {
                        Tiles[places[i][j]].IndexX = i;
                        Tiles[places[i][j]].IndexY = j;
                    }
                    index++;
                }
            return result;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
