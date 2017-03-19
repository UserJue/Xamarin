using System;
using Xamarin.Forms;
using _15Puzzle.ViewModels;

namespace _15Puzzle
{
    public partial class MainPage : ContentPage
    {
        private Models._15Puzzle model;
        private MainViewModel mainViewModel;
        private ContentPage contentView;
        private double tileSize;
        private bool init;
        private double lastFrameHight = -1;

        public MainPage()
        {
            InitializeComponent();
            model = new Models._15Puzzle();
            //model.Create(15,"Frankenberg");
            model.Create(15, "USA_Zion");
            mainViewModel = new MainViewModel(model) {OnTilesMoved = SetTiles};
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
			    if (landScape)
			        contentView.Padding = new Thickness(0, 0, 0, 0);
			    else
			    {
                    contentView.Padding = new Thickness(0, 20, 0, 0);
                    hight -= 20;
			    }
			if (Frame.Height > 100)
			{
                hight = Frame.Height;
                width = Frame.Width;
                //   mainViewModel.Init = true;
                init = true;
			    lastFrameHight = Frame.Height;
			}
            tileSize = Math.Min(hight, width)/4;
            AbsoluteLayout.SetLayoutBounds(Board, new Rectangle(0, 0, tileSize*4, tileSize*4));
//            AbsoluteLayout.SetLayoutBounds(Picture, new Rectangle(0, 0, tileSize * 4, tileSize * 4));
            ShowPictureButton.WidthRequest = tileSize*4;
			if (mainViewModel.IsLandscape != landScape)
			{
				mainViewModel.IsLandscape = landScape;
                contentView.ForceLayout();
			    //mainViewModel.Init = false;
				init = false;
			}
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
                if (!init || (Frame.Height != lastFrameHight))
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
                AbsoluteLayout.SetLayoutBounds(Tile15, new Rectangle(tileSize * mainViewModel.Tiles[14].X, tileSize * mainViewModel.Tiles[14].Y, tileSize, tileSize));
            }
        }
    }
}
