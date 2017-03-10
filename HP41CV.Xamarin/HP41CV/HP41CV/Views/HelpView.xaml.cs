using HP41CV.ViewModels;

namespace HP41CV.Views
{
    public partial class HelpView 
    {
        public HelpView()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            var viewModel = BindingContext as HelpViewModel;
            if (viewModel != null)
            {
                this.Topics.Items.Clear();
                foreach (var topic in viewModel.Topics)
                {
                    Topics.Items.Add(topic);
                }
                Topics.SelectedIndex = 0;
            }
            base.OnBindingContextChanged();
        }
    }
}
