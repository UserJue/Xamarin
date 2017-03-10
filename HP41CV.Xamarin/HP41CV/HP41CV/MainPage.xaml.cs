using System;
using HP41CV.ViewModels;
using Xamarin.Forms;

namespace HP41CV
{
    public partial class MainPage : ContentPage
    {
        private MainViewModel mainViewModel;
        public MainPage()
        {
            InitializeComponent();
            mainViewModel =new MainViewModel();
            BindingContext = mainViewModel;
        }

        private void DoShow(bool showCalculator)
        {
        }

        private void MainPage_OnSizeChanged(object sender, EventArgs e)
        {
            var contentView = (ContentPage) sender;
            if ((contentView.Width > 0) && (calculator != null))
            {
                var hight = Device.OS == TargetPlatform.iOS ? 800 : 780;
                var viewHight = Device.OS == TargetPlatform.iOS ? contentView.Height - 20 : contentView.Height;
                var width = 400.0;

                bool singleView = false;
                var scale = Math.Min(contentView.Width/ width, contentView.Height/ hight);
                var posX = (scale - 1) * 200;
                var posXH = scale* width;
                var posXA = scale * width;
                var posY = (scale - 1) * 390;
                if ((contentView.Width/ width > contentView.Height/ hight) && ((scale*800) > contentView.Width))
                {
                    width = contentView.Width/scale;
                    singleView = true;
                    if (width < 520)
                        posX = (scale - 1) * width/2;
                    else
                    {
                        posX = (scale - 1) * 240 + scale * (width-480)/2;
                        width = 480;
                    }
                    posXH = posXA = 0;
                }
                var sizeXH = contentView.Width - posXH;
                var sizeXA = contentView.Width - posXA;
                if ((scale * 1200) < contentView.Width)
                {
                    sizeXA = scale*400;
                    posXA = contentView.Width - sizeXA;
                    sizeXH = contentView.Width - sizeXA - posXH;
                    mainViewModel.ThreeViews = true;
                }
                else
                    mainViewModel.ThreeViews = false;
                calculator.Scale = scale;
                mainViewModel.OneView = singleView;
                mainViewModel.CalculatorViewModel.Width = width;
                AbsoluteLayout.SetLayoutBounds(calculator,new Rectangle( posX, posY ,AbsoluteLayout.AutoSize,AbsoluteLayout.AutoSize));
                AbsoluteLayout.SetLayoutBounds(AboutView, new Rectangle(posXA, 0,sizeXA , viewHight));
                AbsoluteLayout.SetLayoutBounds(HelpView, new Rectangle(posXH, 0, sizeXH, viewHight));
            }
        }
    }
}
