using _15Puzzle.ViewModels;

namespace _15Puzzle.Views
{
    public partial class TileView 
    {
        public TileView()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            var viewModel = BindingContext as IPanViewModel;
            if (viewModel != null)
            {
                OnMove = viewModel.OnMove;
                OnMoved = viewModel.OnMoved;
            }
            base.OnBindingContextChanged();
        }
    }
}
