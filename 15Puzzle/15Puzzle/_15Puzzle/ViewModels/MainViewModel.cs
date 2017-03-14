using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;
using _15Puzzle.Annotations;

namespace _15Puzzle.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private int holeX = 3;
        private int holeY = 3;

        private int[][] places;
        private Models._15Puzzle model;

        public TileViewModel[] Tiles { get; }

        public Action OnTilesMoved;

        public ICommand ShuffleCommand { get;  private set; }

        public MainViewModel(Models._15Puzzle model)
        {
            this.model = model;
            ShuffleCommand = new Command(() =>
            {
                model.Shuffle();
                foreach (var tile in model.Tiles)
                {
                    Tiles[tile.Index].X = tile.IndexX;
                    Tiles[tile.Index].Y = tile.IndexY;
                }
                OnTilesMoved?.Invoke();
            });
            Tiles = new TileViewModel[model.Tiles.Count];
            places = new int[model.Dimension][];
            for (int i = 0;i < model.Dimension;i++)
                places[i] = new int[model.Dimension];
            foreach (var tile in model.Tiles)
            {
                Tiles[tile.Index] = new TileViewModel(tile);
                Tiles[tile.Index].Picture = model.Picture;
                Tiles[tile.Index].PropertyChanged += MainViewModel_PropertyChanged;
                Tiles[tile.Index].canMoveX = CanMoveX;
                Tiles[tile.Index].canMoveY = CanMoveY;
            }
            FillPlaces();
            Device.StartTimer(TimeSpan.FromSeconds(1),TimerOnTick );
        }

        internal bool TimerOnTick()
        {
            FillPlaces();
            OnTilesMoved?.Invoke();
            return true;
        }

        private void MainViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == "X") || (e.PropertyName == "Y"))
            {
                var tile = sender as TileViewModel;
                if ((tile != null) && tile.Moving)
                {
                    var indexX = tile.IndexX;
                    var indexY = tile.IndexY;
                    switch (tile.MoveDirection)
                    {
                        case TileViewModel.Direction.None:
                            break;
                        case TileViewModel.Direction.Up:
                            indexY--;
                            if ((indexY >= 0) && (places[indexX][indexY] >= 0))
                            {
                                Tiles[places[indexX][indexY]].Move(tile.Delta, false);
                                indexY--;
                                if ((indexY >= 0) && (places[indexX][indexY] >= 0))
                                    Tiles[places[indexX][indexY]].Move(tile.Delta, false);
                            }
                            break;
                        case TileViewModel.Direction.Right:
                            indexX++;
                            if ((indexX < 3) && (places[indexX][indexY] >= 0))
                            {
                                Tiles[places[indexX][indexY]].Move(tile.Delta, true);
                                indexX++;
                                if ((indexX < 3) && (places[indexX][indexY] >= 0))
                                    Tiles[places[indexX][indexY]].Move(tile.Delta, true);
                            }
                            break;
                        case TileViewModel.Direction.Down:
                            indexY++;
                            if ((indexY < 3) && (places[indexX][indexY] >= 0))
                            {
                                Tiles[places[indexX][indexY]].Move(tile.Delta,false);
                                indexY++;
                                if ((indexY < 3) && (places[indexX][indexY] >= 0))
                                    Tiles[places[indexX][indexY]].Move(tile.Delta, false);
                            }
                            break;
                        case TileViewModel.Direction.Left:
                            indexX--;
                            if ((indexX < 3) && (places[indexX][indexY] >= 0))
                            {
                                Tiles[places[indexX][indexY]].Move(tile.Delta, true);
                                indexX--;
                                if ((indexX < 3) && (places[indexX][indexY] >= 0))
                                    Tiles[places[indexX][indexY]].Move(tile.Delta, true);
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    OnTilesMoved?.Invoke();
                }
            }
            else if (e.PropertyName == "Moving")
            {
                var tile = sender as TileViewModel;
                if ((tile != null) && !tile.Moving)
                {
                    FillPlaces();
                    OnTilesMoved?.Invoke();
                }
            }
        }

        private bool CanMoveX(double x, double y, double dx)
        {
            if (((x + dx) < 0) || ((x + dx) > 3)) return false;
            if (holeY != (int) y) return false;
            var hole = holeX;
            if (dx < 0)
            {
                if ((x > 0) && (places[(int)x - 1][(int)y] >= 0))
                {
                    hole++;
                    if ((x > 1) && (places[(int)x - 2][(int)y] >= 0))
                        hole++;
                }
            }
            else
            {
                if ((x < 2) && (places[(int)x + 1][(int)y] >= 0))
                {
                    hole--;
                    if ((x < 3) && (places[(int)x + 2][(int)y] >= 0))
                        hole--;
                }
            }
            return (dx < 0) ? (x + dx) > hole : (x + dx) < hole;
        }

        private bool CanMoveY(double x, double y, double dy)
        {
            if (((y + dy) < 0) || ((y + dy) > 3)) return false;
            if (holeX != (int) x) return false;
            var hole = holeY;
            if (dy < 0)
            {
                if ((y > 0) && (places[(int) x][(int) y - 1] >= 0))
                {
                    hole++;
                    if ((y > 1) && (places[(int)x][(int)y - 2] >= 0))
                        hole++;
                }
            }
            else
            {
                if ((y < 2) && (places[(int) x][(int) y + 1] >= 0))
                {
                    hole--;
                    if ((y < 3) && (places[(int)x][(int)y + 2] >= 0))
                        hole--;
                }
            }
            return (dy < 0) ? (y + dy) > hole : (y + dy) < hole;
        }

        private void FillPlaces()
        {
            bool ok = true;
            for (var i = 0; i < 4; i++)
                for (var j = 0; j < 4; j++)
                    places[i][j] = -1;
            for (var index = 0; index < Tiles.Length; index++)
            {
                var indexX = GetIndex(Tiles[index].X);
                var indexY = GetIndex(Tiles[index].Y);
                if (places[indexX][indexY] < 0)
                    places[indexX][indexY] = index;
                else
                {
                    ok = false;
                }
            }
            for (var i = 0;ok && (i < 4); i++)
                for (var j = 0; j < 4; j++)
                    if (places[i][j] == -1)
                    {
                        holeX = i;
                        holeY = j;
                    }
                    else
                    {
                        Tiles[places[i][j]].Set(i,j);
                    }
            bool hole = false;
            for (var i = 0; ok && (i < 4); i++)
                for (var j = 0; ok && (j < 4); j++)
                    if (places[i][j] == -1)
                        if (hole)
                            ok = false;
                        else
                            hole = true;
            if (ok && hole)
                model.CheckFinished(places);
            else
            {
                foreach (var tile in model.Tiles)
                {
                    Tiles[tile.Index].X = tile.IndexX;
                    Tiles[tile.Index].Y = tile.IndexY;
                }
            }
        }

        private int GetIndex(double value)
        {
            var index = (int)value;
            if ((value - index) > 0.5) index++;
            if (index < 0) index = 0;
            if (index > 3) index = 3;
            return index;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
