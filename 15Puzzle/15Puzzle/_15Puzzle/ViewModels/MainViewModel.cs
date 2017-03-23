using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;
using _15Puzzle.Annotations;
using _15Puzzle.Models;

namespace _15Puzzle.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private const int MaxTiles = 24;
        private int holeX = 3;
        private int holeY = 3;
        private bool init = false;

        private int[][] places;
        private int[][] lastPlaces;
        private Models._15Puzzle model;
        private Settings settings;
        private int showPictureCount;
        private bool showPicture;
        private TileViewModel.Direction moveDirection;
        private bool timerWorking;
        private string hidePictureText;
        private string showPictureText;

        public TileViewModel[] Tiles { get; }

        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                if (isVisible == value) return;
                isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }

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
        private bool isVisible;

        public ICommand ShuffleCommand { get;  private set; }

        public ICommand StartBreakCommand { get; set; }

        public string Picture => model.Picture;

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

        public AboutViewModel AboutViewModel { get; }

        public SettingsViewModel SettingsViewModel { get; }

        public MainViewModel(Models._15Puzzle model,Settings settings,bool userTimer = true)
        {
            this.model = model;
            this.model.PropertyChanged += MainViewModel_PropertyChanged;

            this.settings = settings;
            if (settings != null)
                this.settings.PropertyChanged += Settings_PropertyChanged;

            AboutViewModel = new AboutViewModel { Title = AppResource.ApplicationTitle, SubTitle = AppResource.ForW, Version = "1.0", BackText = AppResource.Close };
            switch (Device.OS)
            {
                case TargetPlatform.iOS:
                    AboutViewModel.SubTitle = AppResource.ForI;
                    break;
                case TargetPlatform.Android:
                    AboutViewModel.SubTitle = AppResource.ForD;
                    break;
            }
            AboutViewModel.IsBack2Visible = true;
            AboutViewModel.BackAction = () =>
            {
                AboutViewModel.IsVisible = false;
                IsVisible = true;
            };
            SettingsViewModel = new SettingsViewModel(settings) { BackText = AppResource.Close,AboutText = AppResource.About};
            SettingsViewModel.AboutAction = () =>
            {
                AboutViewModel.IsVisible = true;
                SettingsViewModel.IsVisible = false;
            };
            SettingsViewModel.BackAction = () =>
            {
                SettingsViewModel.IsVisible = false;
                IsVisible = true;
            };
            holeX = model.DimensionX - 1;
            holeY = model.DimensionY - 1;
            showPictureText = AppResource.ShowPictureText;
            hidePictureText = AppResource.HidePictureText;
            isPortrait = true;
            IsVisible = true;
            SettingCommand = new Command(() =>
            {
                IsVisible = false;
                SettingsViewModel.IsVisible = true;
            });
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
            Tiles = new TileViewModel[MaxTiles];
            places = new int[model.DimensionX][];
            lastPlaces = new int[model.DimensionX][];
            for (int i = 0; i < model.DimensionX; i++)
            {
                places[i] = new int[model.DimensionY];
                lastPlaces[i] = new int[model.DimensionY];
            }
            foreach (var tile in model.Tiles)
            {
                Tiles[tile.Index] = new TileViewModel(tile);
                Tiles[tile.Index].Picture = model.Picture == null ? null : $"{model.Picture}{tile.Index0Y}{tile.Index0X}"; 
                Tiles[tile.Index].PropertyChanged += MainViewModel_PropertyChanged;
                Tiles[tile.Index].canMoveX = CanMoveX;
                Tiles[tile.Index].canMoveY = CanMoveY;
            }
            for (int i = model.Tiles.Count; i < MaxTiles; i++)
            {
                Tiles[i] = new TileViewModel(null);
                Tiles[i].PropertyChanged += MainViewModel_PropertyChanged;
                Tiles[i].canMoveX = CanMoveX;
                Tiles[i].canMoveY = CanMoveY;
            }
            FillPlaces();
            if (userTimer)
                Device.StartTimer(TimeSpan.FromSeconds(10),TimerOnTick );
            init = true;
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PuzzleInfo))
            {
                if (init &&(model != null) && (settings.PuzzleInfo != null))
                {
                    model.Create(settings.PuzzleInfo.Tiles, settings.PuzzleInfo.Picture, settings.PuzzleInfo.Landscape);
                    foreach (var tile in model.Tiles)
                    {
                        Tiles[tile.Index].Picture = $"{model.Picture}{tile.Index0Y}{tile.Index0X}";
                        Tiles[tile.Index].TileVisible = true;
                        Tiles[tile.Index].Y = tile.IndexY;
                        Tiles[tile.Index].X = tile.IndexX;
                    }
                    for (int i = model.Tiles.Count; i < MaxTiles; i++)
                    {
                        Tiles[i].Picture = null;
                        Tiles[i].TileVisible = false;
                        Tiles[i].Y = -1;
                        Tiles[i].X = -1;
                    }
                    places = new int[model.DimensionX][];
                    lastPlaces = new int[model.DimensionX][];
                    for (int i = 0; i < model.DimensionX; i++)
                    {
                        places[i] = new int[model.DimensionY];
                        lastPlaces[i] = new int[model.DimensionY];
                    }
                    FillPlaces();
                    OnTilesMoved?.Invoke();
                }
            }
        }

        internal bool TimerOnTick()
        {
            if (timerWorking) return true;
            if (!isVisible) return true;
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
                try
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
                                    {
                                        Tiles[places[indexX][indexY]].Move(tile.Delta, false);
                                        indexY--;
                                        if ((indexY >= 0) && (places[indexX][indexY] >= 0))
                                        {
                                            Tiles[places[indexX][indexY]].Move(tile.Delta, false);
                                            indexY--;
                                            if ((indexY >= 0) && (places[indexX][indexY] >= 0))
                                                Tiles[places[indexX][indexY]].Move(tile.Delta, false);
                                        }
                                    }
                                }
                                break;
                            case TileViewModel.Direction.Right:
                                indexX++;
                                if ((indexX < model.DimensionX - 1) && (places[indexX][indexY] >= 0))
                                {
                                    Tiles[places[indexX][indexY]].Move(tile.Delta, true);
                                    indexX++;
                                    if ((indexX < model.DimensionX - 1) && (places[indexX][indexY] >= 0))
                                    {
                                        Tiles[places[indexX][indexY]].Move(tile.Delta, true);
                                        indexX++;
                                        if ((indexX < model.DimensionX - 1) && (places[indexX][indexY] >= 0))
                                        {
                                            Tiles[places[indexX][indexY]].Move(tile.Delta, true);
                                            indexX++;
                                            if ((indexX < model.DimensionX - 1) && (places[indexX][indexY] >= 0))
                                                Tiles[places[indexX][indexY]].Move(tile.Delta, true);
                                        }
                                    }
                                }
                                break;
                            case TileViewModel.Direction.Down:
                                indexY++;
                                if ((indexY < model.DimensionY - 1) && (places[indexX][indexY] >= 0))
                                {
                                    Tiles[places[indexX][indexY]].Move(tile.Delta, false);
                                    indexY++;
                                    if ((indexY < model.DimensionY - 1) && (places[indexX][indexY] >= 0))
                                    {
                                        Tiles[places[indexX][indexY]].Move(tile.Delta, false);
                                        indexY++;
                                        if ((indexY < model.DimensionY - 1) && (places[indexX][indexY] >= 0))
                                        {
                                            Tiles[places[indexX][indexY]].Move(tile.Delta, false);
                                            indexY++;
                                            if ((indexY < model.DimensionY - 1) && (places[indexX][indexY] >= 0))
                                                Tiles[places[indexX][indexY]].Move(tile.Delta, false);
                                        }
                                    }
                                }
                                break;
                            case TileViewModel.Direction.Left:
                                indexX--;
                                if ((indexX >= 0) && (places[indexX][indexY] >= 0))
                                {
                                    Tiles[places[indexX][indexY]].Move(tile.Delta, true);
                                    indexX--;
                                    if ((indexX >= 0) && (places[indexX][indexY] >= 0))
                                    {
                                        Tiles[places[indexX][indexY]].Move(tile.Delta, true);
                                        indexX--;
                                        if ((indexX >= 0) && (places[indexX][indexY] >= 0))
                                        {
                                            Tiles[places[indexX][indexY]].Move(tile.Delta, true);
                                            indexX--;
                                            if ((indexX >= 0) && (places[indexX][indexY] >= 0))
                                                Tiles[places[indexX][indexY]].Move(tile.Delta, true);
                                        }
                                    }
                                }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        OnTilesMoved?.Invoke();
                    }
                }
                catch (Exception)
                {
                    ;
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

        protected bool CanMoveX(double x, double y, double dx)
        {
            if (model.Status != Models._15Puzzle.GameStatus.Activ) return false;
            if (((x + dx) < 0) || ((x + dx) > model.DimensionX-1)) return false;
            if (holeY != (int) y) return false;
            var hole = holeX;
            if (dx < 0)
            {
                if ((x > 0) && (places[(int)x - 1][(int)y] >= 0))
                {
                    hole++;
                    if ((x > 1) && (places[(int) x - 2][(int) y] >= 0))
                    {
                        hole++;
                        if ((x > 2) && (places[(int) x - 3][(int) y] >= 0))
                        {
                            hole++;
                            if ((x > 3) && (places[(int) x - 4][(int) y] >= 0))
                                hole++;
                        }
                    }
                }
            }
            else
            {
                if ((x < model.DimensionX - 3) && (places[(int)x + 1][(int)y] >= 0))
                {
                    hole--;
                    if ((x < model.DimensionX - 4) && (places[(int) x + 2][(int) y] >= 0))
                    {
                        hole--;
                        if ((x < model.DimensionX - 5) && (places[(int) x + 3][(int) y] >= 0))
                        {
                            hole--;
                            if ((x < model.DimensionX - 6) && (places[(int) x + 4][(int) y] >= 0))
                                hole--;
                        }
                    }
                }
            }
            return (dx < 0) ? (x + dx) > hole : (x + dx) < hole;
        }

        private bool CanMoveY(double x, double y, double dy)
        {
            if (model.Status != Models._15Puzzle.GameStatus.Activ) return false;
            if (((y + dy) < 0) || ((y + dy) > model.DimensionY-1)) return false;
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
            bool hole = false;
            try
            {
                for (var i = 0; i < model.DimensionX; i++)
                    for (var j = 0; j < model.DimensionY; j++)
                        places[i][j] = -1;
                for (var index = 0; index < Tiles.Length; index++)
                {
                    if (!Tiles[index].TileVisible) continue;
                    var indexX = GetIndex(Tiles[index].X, true);
                    var indexY = GetIndex(Tiles[index].Y, false);
                    if (places[indexX][indexY] < 0)
                        places[indexX][indexY] = index;
                    else
                    {
                        ok = false;
                    }
                }
                for (var i = 0; ok && (i < model.DimensionX); i++)
                    for (var j = 0; j < model.DimensionY; j++)
                        if (places[i][j] == -1)
                        {
                            holeX = i;
                            holeY = j;
                        }
                        else
                        {
                            Tiles[places[i][j]].Set(i, j);
                        }
                if (model.Status == Models._15Puzzle.GameStatus.None)
                    IsEqualSet(lastPlaces, places);
                else
                {
                    for (var i = 0; ok && (i < model.DimensionX); i++)
                        for (var j = 0; ok && (j < model.DimensionY); j++)
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
                }
            }
            catch (Exception)
            {
                ok = false;
            }
            if (!ok || !hole)
            {
                foreach (var tile in model.Tiles)
                {
                    Tiles[tile.Index].X = tile.IndexX;
                    Tiles[tile.Index].Y = tile.IndexY;
                }
            }
        }

        private bool IsEqualSet(int[][] last, int[][] actual)
        {
            var result = true;
            for (var i = 0; i < model.DimensionX; i++)
                for (var j = 0; j < model.DimensionY; j++)
                {
                    result &= last[i][j] == actual[i][j];
                    last[i][j] = actual[i][j];
                }
            return result;
        }

        private int GetIndex(double value,bool isX)
        {
            var index = (int)value;
            var maxIndex = isX ? model.DimensionX : model.DimensionY;
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
            if (index > maxIndex) index = maxIndex;
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
