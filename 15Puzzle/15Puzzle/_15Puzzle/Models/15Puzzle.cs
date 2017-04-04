using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Json;
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

        public int[][] Places { get; private set; }
        public int DimensionX { get; private set; }
        public int DimensionY { get; private set; }
        public int HoleX { get; set; }
        public int HoleY { get; set; }

        public GameStatus Status
        {
            get { return status; }
            set
            {
                if (value == status) return;
                if (status == GameStatus.Activ)
                {
                    usedTime += DateTime.Now - startTime;
                    status = value;
                    SaveToProperties();
                }
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

        public bool LoadFromProperties()
        {
            int? intValue1;
            int? intValue2;
            IDictionary<string, object> settingsDictionary = Application.Current.Properties;
            if (!settingsDictionary.ContainsKey("UsedMoves")) return false;
            intValue1 = settingsDictionary["UsedMoves"] as int?;
            if (intValue1 == null) return false;
            usedMoves = intValue1.Value;
            if (!settingsDictionary.ContainsKey("UsedTime")) return false;
            intValue1 = settingsDictionary["UsedTime"] as int?;
            if (intValue1 == null) return false;
            usedTime = TimeSpan.FromSeconds(intValue1.Value);
            if (!settingsDictionary.ContainsKey("Picture")) return false;
            var picture = settingsDictionary["Picture"] as string;
            if (picture == null) return false;
            Picture = picture;
            if (!settingsDictionary.ContainsKey("DimensionX")) return false;
            intValue1 = settingsDictionary["DimensionX"] as int?;
            if (intValue1 == null) return false;
            DimensionX = intValue1.Value;
            if (!settingsDictionary.ContainsKey("DimensionY")) return false;
            intValue1 = settingsDictionary["DimensionY"] as int?;
            if (intValue1 == null) return false;
            DimensionY = intValue1.Value;
            Tiles.Clear();
            Places = new int[DimensionX][];
            for (int i = 0; i < DimensionX; i++)
            {
                Places[i] = new int[DimensionY];
                for (var j = 0; j < DimensionY; j++)
                    Places[i][j] = -1;
            }
            for (var i = 0; i < DimensionX*DimensionY-1; i++)
            {
                var key = $"Tile_{i}_0X";
                if (!settingsDictionary.ContainsKey(key)) return false;
                intValue1 = settingsDictionary[key] as int?;
                if (intValue1 == null) return false;
                key = $"Tile_{i}_0Y";
                if (!settingsDictionary.ContainsKey(key)) return false;
                intValue2 = settingsDictionary[key] as int?;
                if (intValue2 == null) return false;
                var tile = new Tile(i, intValue1.Value, intValue2.Value);
                if (usedMoves > 0)
                {
                    key = $"Tile_{i}_X";
                    if (!settingsDictionary.ContainsKey(key)) return false;
                    intValue1 = settingsDictionary[key] as int?;
                    if (intValue1 == null) return false;
                    tile.IndexX = intValue1.Value;
                    key = $"Tile_{i}_Y";
                    if (!settingsDictionary.ContainsKey(key)) return false;
                    intValue2 = settingsDictionary[key] as int?;
                    if (intValue2 == null) return false;
                    tile.IndexY = intValue2.Value;
                }
                Tiles.Add(tile);
                Places[tile.IndexX][tile.IndexY] = tile.Index;
            }
            return true;
        }

        public void SaveToProperties()
        {
            //            Application.Current.Properties["Status"] = Status;
            Application.Current.Properties["UsedMoves"] = usedMoves;
            Application.Current.Properties["UsedTime"] = (int)usedTime.TotalSeconds;
            foreach (var tile in Tiles)
            {
                var key = $"Tile_{tile.Index}_0X";
                Application.Current.Properties[key] = tile.Index0X;
                key = $"Tile_{tile.Index}_0Y";
                Application.Current.Properties[key] = tile.Index0Y;
                key = $"Tile_{tile.Index}_X";
                Application.Current.Properties[key] = tile.IndexX;
                key = $"Tile_{tile.Index}_Y";
                Application.Current.Properties[key] = tile.IndexY;
            }
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
            Places = new int[DimensionX][];
            for (int i = 0; i < DimensionX; i++)
            {
                Places[i] = new int[DimensionY];
            }
            var index = 0;
            for (var j = 0; j < DimensionY; j++)
                for (var i = 0;i < DimensionX;i++)
                {
                    if (index >= tilesNumber) break;
                    Tiles.Add(new Tile(index,i,j));
                    Places[i][j] = index;
                    index++;
                }
            HoleX = DimensionX-1;
            HoleY = DimensionY-1;
            Places[HoleX][HoleY] = -1;
            Shuffle();
            Application.Current.Properties["Picture"] = Picture;
            Application.Current.Properties["DimensionX"] = DimensionX;
            Application.Current.Properties["DimensionY"] = DimensionY;
            SaveToProperties();
            return true;
        }

        public bool Create(int tilesNumber, int holeIndex, bool landscape = false)
        {
            Status = GameStatus.Activ;
            UsedMoves = 0;
            usedTime = TimeSpan.Zero;
            Picture = null;
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
            var hole = false;
            Places = new int[DimensionX][];
            for (int i = 0; i < DimensionX; i++)
            {
                Places[i] = new int[DimensionY];
            }
            for (var j = 0; j < DimensionY; j++)
                for (var i = 0; i < DimensionX; i++)
                {
                    if ((index == holeIndex) && !hole)
                    {
                        HoleX = i;
                        HoleY = j;
                        Places[HoleX][HoleY] = -1;
                        hole = true;
                        continue;
                    }
                    Tiles.Add(new Tile(index, i, j));
                    Places[i][j] = index;
                    index++;
                }
            return true;
        }

        public void Shuffle()
        {
            for (var i = 0; i < 200; i++)
            {
                int dir;
                try
                {
                    dir = random.Next(4);
                    if(!Move(dir))
                        i--;
                }
                catch (Exception)
                {
                    dir = 0;
                }
            }
            //var index1 = random.Next(DimensionX* DimensionY-1);
            //var index2 = random.Next(DimensionX * DimensionY-1);
            //if (index1 != index2)
            //{
            //    var x = Tiles[index1].IndexX;
            //    var y = Tiles[index1].IndexY;
            //    Tiles[index1].IndexX = Tiles[index2].IndexX;
            //    Tiles[index1].IndexY = Tiles[index2].IndexY;
            //    Tiles[index2].IndexX = x;
            //    Tiles[index2].IndexY = y;
            //}
            Status = GameStatus.None;
        }

        public bool Move(int dir)
        {
            switch (dir)
            {
                case 0: // Up
                    if (HoleY == DimensionY - 1) return false;
                    Tiles[Places[HoleX][HoleY + 1]].IndexY--;
                    Places[HoleX][HoleY] = Places[HoleX][HoleY + 1];
                    HoleY++;
                    Places[HoleX][HoleY] = -1;
                    break;
                case 1: // Right
                    if (HoleX == 0) return false;
                    Tiles[Places[HoleX - 1][HoleY]].IndexX++;
                    Places[HoleX][HoleY] = Places[HoleX - 1][HoleY];
                    HoleX--;
                    Places[HoleX][HoleY] = -1;
                    break;
                case 2: // Down
                    if (HoleY == 0) return false;
                    Tiles[Places[HoleX][HoleY - 1]].IndexY++;
                    Places[HoleX][HoleY] = Places[HoleX][HoleY - 1];
                    HoleY--;
                    Places[HoleX][HoleY] = -1;
                    break;
                default: // Left
                    if (HoleX == DimensionX - 1) return false;
                    Tiles[Places[HoleX + 1][HoleY]].IndexX--;
                    Places[HoleX][HoleY] = Places[HoleX + 1][HoleY];
                    HoleX++;
                    Places[HoleX][HoleY] = -1;
                    break;
            }
            return true;
        }

        public bool CheckFinished(int[][] places)
        {
            var result = true;
            var index = 0;
            for (var j = 0; j < DimensionY; j++)
                for (var i = 0; i < DimensionX; i++)
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
                usedTime += (DateTime.Now - startTime);
                startTime = DateTime.Now;
                var factor = ((usedTime.Minutes >= 60)
                    ? 1
                    : (usedTime.Minutes > 10) ? (600 - usedTime.Minutes + 10) : 6000 - usedTime.TotalSeconds);
                var score = (usedMoves < 100) ? 100000 + (100 - usedMoves)* factor : 
                    (1000 - usedMoves) * (factor/10+1);
                if (Application.Current.MainPage != null)
                    Application.Current.MainPage.DisplayAlert(AppResource.ApplicationTitle, AppResource.WinMessage + (int)score, "OK");
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
