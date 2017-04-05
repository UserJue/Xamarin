using System;
using Xamarin.Forms;
using _15Puzzle.Models;
using _15Puzzle.ViewModels;

namespace _15Puzzle
{
    public partial class MainPage : ContentPage
    {
        private Models._15Puzzle model;
        private Settings settings;
        private MainViewModel mainViewModel;
        private ContentPage contentView;
        private double tileSize;
        private bool init;
        private double lastBoardLayoutHight = -1;
        private double dimensionX;
        private double dimensionY;

        public MainPage()
        {
            InitializeComponent();
            settings = new Settings();
            settings.PuzzleInfos.Add(new PuzzleInfo("Frankenberg", "Frankenberg",15,false));
            settings.PuzzleInfos.Add(new PuzzleInfo("USA_Zion", "USA_Zion", 23, true));
            settings.PuzzleInfos.Add(new PuzzleInfo("RotenburgOdT", "RotenburgOdT", 23, false));
            if (!settings.LoadFromProperties())
                settings.PuzzleInfo = settings.PuzzleInfos[0]; 
            model = new Models._15Puzzle();
            if (!model.LoadFromProperties() || (model.Picture != settings.PuzzleInfo.Picture) || (model.Status == Models._15Puzzle.GameStatus.None) || (model.Status == Models._15Puzzle.GameStatus.Finished))
                model.Create(settings.PuzzleInfo.Tiles, settings.PuzzleInfo.Picture, settings.PuzzleInfo.Landscape);
            mainViewModel = new MainViewModel(model,settings) {OnTilesMoved = SetTiles};
            BindingContext = mainViewModel;
        }
        
        private void MainPage_OnSizeChanged(object sender, EventArgs e)
        {
            contentView = (ContentPage)sender;
            if (contentView.Width > 0)
            {
                SetSize();
                SetTiles();
            }
        }

        private void SetSize()
        {
			var landScape = contentView.Width > contentView.Height;
            var hight = contentView.Height;
            var width = contentView.Width;
            if (Device.OS == TargetPlatform.iOS)
            {
                if (landScape)
			        contentView.Padding = new Thickness(0, 0, 0, 0);
			    else
			    {
                    contentView.Padding = new Thickness(0, 20, 0, 0);
                    hight -= 20;
			    }
            }
            AbsoluteLayout.SetLayoutBounds(PuzzleView,new Rectangle(0,0,width,hight));
            AbsoluteLayout.SetLayoutBounds(AboutView, new Rectangle(0, 0, width, hight));
            AbsoluteLayout.SetLayoutBounds(SettingsView, new Rectangle(0, 0, width, hight));
			if (BoardLayout.Height > 100)
			{
                hight = BoardLayout.Height;
                width = BoardLayout.Width;
                init = true;
			    lastBoardLayoutHight = BoardLayout.Height;
			}
            tileSize = Math.Min(hight/model.DimensionY, width/model.DimensionX);
            dimensionX = model.DimensionX;
            dimensionY = model.DimensionY;
            AbsoluteLayout.SetLayoutBounds(Board, new Rectangle(0, 0, tileSize*model.DimensionX, tileSize*model.DimensionY));
			if (mainViewModel.IsLandscape != landScape)
			{
				mainViewModel.IsLandscape = landScape;
                contentView.ForceLayout();
				init = false;
			}
            PreviewP.HeightRequest = 120;
            PreviewL.WidthRequest = 120;
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            var result = base.OnMeasure(widthConstraint, heightConstraint);
            SetSize();
            return result;
        }

        private void SetTiles()
        {
            if (contentView.Width > 0)
            {
                if (!init || (BoardLayout.Height != lastBoardLayoutHight) || (dimensionX != model.DimensionX) || (dimensionY != model.DimensionY))
                {
                    SetSize();
                }
                AbsoluteLayout.SetLayoutBounds(Tile1, new Rectangle(tileSize * mainViewModel.Tiles[0].X, tileSize * mainViewModel.Tiles[0].Y, tileSize, tileSize));
                AbsoluteLayout.SetLayoutBounds(Tile2, new Rectangle(tileSize * mainViewModel.Tiles[1].X, tileSize * mainViewModel.Tiles[1].Y, tileSize, tileSize));
                AbsoluteLayout.SetLayoutBounds(Tile3, new Rectangle(tileSize * mainViewModel.Tiles[2].X, tileSize * mainViewModel.Tiles[2].Y, tileSize, tileSize));
                AbsoluteLayout.SetLayoutBounds(Tile4, new Rectangle(tileSize * mainViewModel.Tiles[3].X, tileSize * mainViewModel.Tiles[3].Y, tileSize, tileSize));
                AbsoluteLayout.SetLayoutBounds(Tile5, new Rectangle(tileSize * mainViewModel.Tiles[4].X, tileSize * mainViewModel.Tiles[4].Y, tileSize, tileSize));
                AbsoluteLayout.SetLayoutBounds(Tile6, new Rectangle(tileSize * mainViewModel.Tiles[5].X, tileSize * mainViewModel.Tiles[5].Y, tileSize, tileSize));
                AbsoluteLayout.SetLayoutBounds(Tile7, new Rectangle(tileSize * mainViewModel.Tiles[6].X, tileSize * mainViewModel.Tiles[6].Y, tileSize, tileSize));
                AbsoluteLayout.SetLayoutBounds(Tile8, new Rectangle(tileSize * mainViewModel.Tiles[7].X, tileSize * mainViewModel.Tiles[7].Y, tileSize, tileSize));
                AbsoluteLayout.SetLayoutBounds(Tile9, new Rectangle(tileSize * mainViewModel.Tiles[8].X, tileSize * mainViewModel.Tiles[8].Y, tileSize, tileSize));
                AbsoluteLayout.SetLayoutBounds(Tile10, new Rectangle(tileSize * mainViewModel.Tiles[9].X, tileSize * mainViewModel.Tiles[9].Y, tileSize, tileSize));
                AbsoluteLayout.SetLayoutBounds(Tile11, new Rectangle(tileSize * mainViewModel.Tiles[10].X, tileSize * mainViewModel.Tiles[10].Y, tileSize, tileSize));
                AbsoluteLayout.SetLayoutBounds(Tile12, new Rectangle(tileSize * mainViewModel.Tiles[11].X, tileSize * mainViewModel.Tiles[11].Y, tileSize, tileSize));
                AbsoluteLayout.SetLayoutBounds(Tile13, new Rectangle(tileSize * mainViewModel.Tiles[12].X, tileSize * mainViewModel.Tiles[12].Y, tileSize, tileSize));
                AbsoluteLayout.SetLayoutBounds(Tile14, new Rectangle(tileSize * mainViewModel.Tiles[13].X, tileSize * mainViewModel.Tiles[13].Y, tileSize, tileSize));
                AbsoluteLayout.SetLayoutBounds(Tile15, new Rectangle(tileSize * mainViewModel.Tiles[14].X, tileSize * mainViewModel.Tiles[14].Y, (mainViewModel.Tiles[14].TileVisible) ? tileSize : 0, (mainViewModel.Tiles[14].TileVisible) ? tileSize : 0));
                AbsoluteLayout.SetLayoutBounds(Tile16, new Rectangle(tileSize * mainViewModel.Tiles[15].X, tileSize * mainViewModel.Tiles[15].Y, (mainViewModel.Tiles[15].TileVisible) ? tileSize : 0, (mainViewModel.Tiles[15].TileVisible) ? tileSize : 0));
                AbsoluteLayout.SetLayoutBounds(Tile17, new Rectangle(tileSize * mainViewModel.Tiles[16].X, tileSize * mainViewModel.Tiles[16].Y, (mainViewModel.Tiles[16].TileVisible) ? tileSize : 0, (mainViewModel.Tiles[16].TileVisible) ? tileSize : 0));
                AbsoluteLayout.SetLayoutBounds(Tile18, new Rectangle(tileSize * mainViewModel.Tiles[17].X, tileSize * mainViewModel.Tiles[17].Y, (mainViewModel.Tiles[17].TileVisible) ? tileSize : 0, (mainViewModel.Tiles[17].TileVisible) ? tileSize : 0));
                AbsoluteLayout.SetLayoutBounds(Tile19, new Rectangle(tileSize * mainViewModel.Tiles[18].X, tileSize * mainViewModel.Tiles[18].Y, (mainViewModel.Tiles[18].TileVisible) ? tileSize : 0, (mainViewModel.Tiles[18].TileVisible) ? tileSize : 0));
                AbsoluteLayout.SetLayoutBounds(Tile20, new Rectangle(tileSize * mainViewModel.Tiles[19].X, tileSize * mainViewModel.Tiles[19].Y, (mainViewModel.Tiles[19].TileVisible) ? tileSize : 0, (mainViewModel.Tiles[19].TileVisible) ? tileSize : 0));
                AbsoluteLayout.SetLayoutBounds(Tile21, new Rectangle(tileSize * mainViewModel.Tiles[20].X, tileSize * mainViewModel.Tiles[20].Y, (mainViewModel.Tiles[20].TileVisible) ? tileSize : 0, (mainViewModel.Tiles[20].TileVisible) ? tileSize : 0));
                AbsoluteLayout.SetLayoutBounds(Tile22, new Rectangle(tileSize * mainViewModel.Tiles[21].X, tileSize * mainViewModel.Tiles[21].Y, (mainViewModel.Tiles[21].TileVisible) ? tileSize : 0, (mainViewModel.Tiles[21].TileVisible) ? tileSize : 0));
                AbsoluteLayout.SetLayoutBounds(Tile23, new Rectangle(tileSize * mainViewModel.Tiles[22].X, tileSize * mainViewModel.Tiles[22].Y, (mainViewModel.Tiles[22].TileVisible) ? tileSize : 0, (mainViewModel.Tiles[22].TileVisible) ? tileSize : 0));
                AbsoluteLayout.SetLayoutBounds(Tile24, new Rectangle(tileSize * mainViewModel.Tiles[23].X, tileSize * mainViewModel.Tiles[23].Y, (mainViewModel.Tiles[23].TileVisible) ? tileSize : 0, (mainViewModel.Tiles[23].TileVisible) ? tileSize : 0));
            }
        }
    }
}
