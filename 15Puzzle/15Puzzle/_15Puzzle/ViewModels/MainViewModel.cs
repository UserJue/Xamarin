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
        private int[][] lastPlaces;
        private Models._15Puzzle model;
        private int showPictureCount;
        private bool showPicture;
        private TileViewModel.Direction moveDirection;
        private bool timerWorking;
        private string hidePictureText;
        private string showPictureText;

        public TileViewModel[] Tiles { get; }

        public string StartBreakText
        {
            get
            {
                var text = (model.Status == Models._15Puzzle.GameStatus.None) ||
                       (model.Status == Models._15Puzzle.GameStatus.Inactiv)
                    ? "Start"
                    : (model.Status == Models._15Puzzle.GameStatus.Activ) ? "Break" : null;
                return text;
            }
        }

        public string StartBreakPicture => (model.Status == Models._15Puzzle.GameStatus.None) ||
                       (model.Status == Models._15Puzzle.GameStatus.Inactiv)
                    ? "Start"
                    : (model.Status == Models._15Puzzle.GameStatus.Activ) ? "Break" : null;

        public string SettingPicture => "Setting";

        public Action OnTilesMoved;
        private bool isPortrait;
        private bool init;

        public ICommand ShuffleCommand { get;  private set; }

        public ICommand StartBreakCommand { get; set; }

        public string Picture => model.Picture;

        public bool Init
        {
            get { return init; }
            set
            {
                init = value;
                ShowPicture = !init;
            }
        }

        public bool ShowPicture
        {
            get { return showPicture; }
            set
            {
                if (value == showPicture) return;
                showPicture = value;
                ShowPictureText = showPicture ? hidePictureText : showPictureText;
                OnPropertyChanged(nameof(ShowPicture));
                OnPropertyChanged(nameof(ShowTiles));
                OnPropertyChanged(nameof(ShowPictureText));
            }
        }

        public bool ShowShuffle => model.Status != Models._15Puzzle.GameStatus.Activ;

        public bool IsPortrait
        {
            get { return isPortrait; }
            set
            {
                if (value == isPortrait) return;
                isPortrait = value;
                OnPropertyChanged(nameof(IsPortrait));
                OnPropertyChanged(nameof(IsLandscape));
            }
        }

        public bool IsLandscape
        {
            get { return !isPortrait; }
            set
            {
                if (value == !isPortrait) return;
                isPortrait = !value;
                OnPropertyChanged(nameof(IsPortrait));
                OnPropertyChanged(nameof(IsLandscape));
            }
        }

        public string ShowPictureText { get; set; }

        public int Moves { get; set; }

        public string UsedTime => (model.UsedTime.Hours > 0) ? model.UsedTime.ToString("hh\\:mm\\:ss") : model.UsedTime.ToString("mm\\:ss");

        public int UsedMoves => model.UsedMoves;

        public bool ShowTiles => !showPicture;

        public ICommand ShowPictureCommand { get; set; }

        public ICommand SettingCommand { get; set; }

        public MainViewModel(Models._15Puzzle model)
        {
            showPictureText = AppResource.ShowPictureText;
            hidePictureText = AppResource.HidePictureText;
            isPortrait = true;
            this.model = model;
            this.model.PropertyChanged += MainViewModel_PropertyChanged;
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
            StartBreakCommand = new Command(() =>
            {
                if ((model.Status == Models._15Puzzle.GameStatus.None) ||
                    (model.Status == Models._15Puzzle.GameStatus.Inactiv))
                    model.Status = Models._15Puzzle.GameStatus.Activ;
                else
                    model.Status = Models._15Puzzle.GameStatus.Inactiv;
            });
            ShowPictureText = showPictureText;
            ShowPictureCommand = new Command(() =>
            {
                if (showPicture)
                {
                    ShowPicture = false;
                    showPictureCount = -1;
                }
                else
                {
                    showPictureCount = 5;
                    ShowPicture = true;
                }
            });
            Tiles = new TileViewModel[model.Tiles.Count];
            places = new int[model.Dimension][];
            lastPlaces = new int[model.Dimension][];
            for (int i = 0; i < model.Dimension; i++)
            {
                places[i] = new int[model.Dimension];
                lastPlaces[i] = new int[model.Dimension];
            }
            foreach (var tile in model.Tiles)
            {
                Tiles[tile.Index] = new TileViewModel(tile);
                Tiles[tile.Index].Picture = $"{model.Picture}{tile.Index0Y}{tile.Index0X}"; 
                Tiles[tile.Index].PropertyChanged += MainViewModel_PropertyChanged;
                Tiles[tile.Index].canMoveX = CanMoveX;
                Tiles[tile.Index].canMoveY = CanMoveY;
            }
            FillPlaces();
            Device.StartTimer(TimeSpan.FromSeconds(1),TimerOnTick );
        }

        internal bool TimerOnTick()
        {
            if (timerWorking) return true;
            timerWorking = true;
            if (ShowPicture)
            {
                showPictureCount--;
                ShowPicture = showPictureCount >= 0;
            }
            else if (!isMoving())
            {
                FillPlaces();
                OnTilesMoved?.Invoke();
                OnPropertyChanged(nameof(UsedTime));
            }
            timerWorking = false;
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
                    moveDirection = tile.MoveDirection;
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
                            if ((indexX >= 0) && (places[indexX][indexY] >= 0))
                            {
                                Tiles[places[indexX][indexY]].Move(tile.Delta, true);
                                indexX--;
                                if ((indexX >= 0) && (places[indexX][indexY] >= 0))
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
                    if (!timerWorking)
                    {
                        FillPlaces();
                        OnTilesMoved?.Invoke();
                    }
                    moveDirection = TileViewModel.Direction.None;
                }
            }
            else if (e.PropertyName == "Status")
            {
                OnPropertyChanged(nameof(StartBreakText));
                OnPropertyChanged(nameof(StartBreakPicture));
                OnPropertyChanged(nameof(ShowShuffle));
            }
            else if (e.PropertyName == "UsedMoves")
            {
                OnPropertyChanged(nameof(UsedMoves));
            }
        }

        private bool CanMoveX(double x, double y, double dx)
        {
            if (model.Status != Models._15Puzzle.GameStatus.Activ) return false;
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
            if (model.Status != Models._15Puzzle.GameStatus.Activ) return false;
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
            if (model.Status == Models._15Puzzle.GameStatus.None)
                IsEqualSet(lastPlaces, places);
            else
            {
                bool hole = false;
                for (var i = 0; ok && (i < 4); i++)
                    for (var j = 0; ok && (j < 4); j++)
                        if (places[i][j] == -1)
                            if (hole)
                                ok = false;
                            else
                                hole = true;
                if (ok && hole)
                {
                    if (!IsEqualSet(lastPlaces, places))
                        model.CheckFinished(places);
                }
                else
                {
                    foreach (var tile in model.Tiles)
                    {
                        Tiles[tile.Index].X = tile.IndexX;
                        Tiles[tile.Index].Y = tile.IndexY;
                    }
                }
            }
        }

        private bool IsEqualSet(int[][] last, int[][] actual)
        {
            var result = true;
            var dimension = actual.Length;
            for (var j = 0; j < dimension; j++)
                for (var i = 0; i < dimension; i++)
                {
                    result &= last[i][j] == actual[i][j];
                    last[i][j] = actual[i][j];
                }
            return result;
        }

        private int GetIndex(double value)
        {
            var index = (int)value;
            if ((moveDirection == TileViewModel.Direction.Down) || (moveDirection == TileViewModel.Direction.Right))
            {
                if ((value - index) > 0.1) index++;
            }
            else if ((moveDirection == TileViewModel.Direction.Up) ||
                     (moveDirection == TileViewModel.Direction.Left))
            {
                if ((value - index) > 0.9) index++;
                else if ((value - index) > 0.1)
                        ;
            }
            else
                if ((value - index) > 0.5) index++;
            if (index < 0) index = 0;
            if (index > 3) index = 3;
            return index;
        }

        private bool isMoving()
        {
            var result = false;
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
