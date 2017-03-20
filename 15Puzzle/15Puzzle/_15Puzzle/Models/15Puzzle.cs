using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using _15Puzzle.Annotations;

namespace _15Puzzle.Models
{
    public class _15Puzzle : INotifyPropertyChanged
    {
        public enum GameStatus { None, WatingForPlayer, Activ, Inactiv, Stoped, Finished, Deleted }

        private Random random;
        private GameStatus status;
        private TimeSpan usedTime;
        private DateTime startTime;
        private int usedMoves;

        public IList<Tile> Tiles { get; set; }

        public int DimensionX { get; private set; }
        public int DimensionY { get; private set; }

        public GameStatus Status
        {
            get { return status; }
            set
            {
                if (value == status) return;
                if (status == GameStatus.Activ)
                    usedTime += DateTime.Now - startTime;
                else if (value == GameStatus.None)
                {
                    usedTime = TimeSpan.Zero;
                    UsedMoves = 0;
                }
                startTime = DateTime.Now;
                status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public TimeSpan UsedTime => (status == GameStatus.Activ) ? usedTime + (DateTime.Now - startTime): usedTime;

        public int UsedMoves
        {
            get { return usedMoves; }
            set
            {
                if (value == usedMoves) return;
                usedMoves = value;
                OnPropertyChanged(nameof(UsedMoves));
            }
        }

        public string Picture { get; set; }

        public _15Puzzle()
        {
            Status = GameStatus.None;
            random = new Random();
            Tiles = new List<Tile>();
        }

        public bool Create(int tilesNumber,string picture=null,bool landscape=false)
        {
            Status = GameStatus.None;
            UsedMoves = 0;
            usedTime = TimeSpan.Zero;
            Picture = picture;
            switch (tilesNumber)
            {
                case 15:
                    DimensionX = DimensionY = 4;
                    break;
                case 23:
                    if (landscape)
                    {
                        DimensionX = 6;
                        DimensionY = 4;
                    }
                    else
                    {
                        DimensionX = 4;
                        DimensionY = 6;
                    }
                    break;
                default:
                    return false;
            }
            Tiles.Clear();
            var index = 0;
            for (var j = 0; j < DimensionY; j++)
                for (var i = 0;i < DimensionX;i++)
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
            for (var i = 0; i < 20; i++)
            {
                var index1 = random.Next(DimensionX* DimensionY-1);
                var index2 = random.Next(DimensionX * DimensionY-1);
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
            Status = GameStatus.None;
        }

        public bool CheckFinished(int[][] places)
        {
            var result = true;
            var index = 0;
            for (var i = 0; i < DimensionX; i++)
                for (var j = 0; j < DimensionY; j++)
                {
                    if ((places[i][j] >= 0) && (places[i][j] < Tiles.Count))
                    {
                        result &= places[i][j] == index;
                        Tiles[places[i][j]].IndexX = i;
                        Tiles[places[i][j]].IndexY = j;
                    }
                    index++;
                }
            UsedMoves++;
            if (result)
            {
                if (Application.Current.MainPage != null)
                    Application.Current.MainPage.DisplayAlert(AppResource.ApplicationTitle, AppResource.WinMessage, "OK");
                Status = GameStatus.Finished;
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
