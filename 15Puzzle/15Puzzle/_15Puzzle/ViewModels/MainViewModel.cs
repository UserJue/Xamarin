using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using _15Puzzle.Annotations;

namespace _15Puzzle.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private int holeX = 3;
        private int holeY = 3;
        private Random random;

        private int[][] places; 

        public TileViewModel[] Tiles { get; private set; }

        public Action OnTilesMoved;

        public MainViewModel()
        {
            random = new Random();
            Tiles = new TileViewModel[15];
            places = new[] {new[] { 0,1,2,3}, new[] { 4,5,6,7}, new[] { 8,9,10,11}, new[] { 12,13,14,-1}};
            for (int i = 0; i < 15; i++)
            {
                Tiles[i] = new TileViewModel(i);
                Tiles[i].PropertyChanged += MainViewModel_PropertyChanged;
                Tiles[i].canMoveX = CanMoveX;
                Tiles[i].canMoveY = CanMoveY;
            }
            Shuffle();
            FillPlaces();
        }

        public void Shuffle()
        {
            for (var i = 0; i < 20; i++)
            {
                var index1 = random.Next(15);
                var index2 = random.Next(15);
                if (index1 != index2)
                {
                    var x = Tiles[index1].X;
                    var y = Tiles[index1].Y;
                    Tiles[index1].X = Tiles[index2].X; 
                    Tiles[index1].Y = Tiles[index2].Y; 
                    Tiles[index2].X = x; 
                    Tiles[index2].Y = y;
                    OnTilesMoved?.Invoke();
                }
            }
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
            for (var i = 0; i < 4; i++)
                for (var j = 0; j < 4; j++)
                    places[i][j] = -1;
            for (int index = 0; index < Tiles.Length; index++)
            {
                var indexX = GetIndex(Tiles[index].X);
                var indexY = GetIndex(Tiles[index].Y);
                if (places[indexX][indexY] < 0)
                    places[indexX][indexY] = index;
                else
                {
                    ;
                }
            }
            for (var i = 0; i < 4; i++)
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
