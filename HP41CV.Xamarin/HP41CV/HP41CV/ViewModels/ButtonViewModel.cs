using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using HP41CV.Annotations;
using HP41CV.Models;
using Xamarin.Forms;

namespace HP41CV.ViewModels
{
    public class ButtonViewModel : INotifyPropertyChanged
    {
        private double width;
        protected CalculatorViewModel Parent;
        private bool _alpha;

        public KeyModel MainModel { get; private set; }
        public KeyModel AlphaModel { get; private set; }
        public string TextShift { get; private set; }
        public string Text { get; private set; }
        public string TextAlpha { get; private set; }
        public int IndexMain { get; private set; }
        public int IndexShift { get; private set; }
        public Color Color { get; set; }
        public KeyModel Key { get { return AlphaModel; } }
        public double SelectedHeight { get; set; }

        public ICommand PerfomKeyCommand { get; private set; }

        public double Width
        {
            get { return width; }
            set { width = value; }
        }

        public double Width2 => width + 8;

        public ButtonViewModel(string text,string textShift,  string textAlpha)
        {
            TextShift = textShift;
            Text = text;
            TextAlpha = textAlpha;
            Color = Color.FromHex("#FF4D2A18");
            Width = 20;

            PerfomKeyCommand = new Command(PerformKey);
        }

        public ButtonViewModel(CalculatorViewModel parent, int indexMain, int indexShift, KeyModel mainModel, KeyModel alphaModel)
        {
            Parent = parent;
            IndexMain = indexMain;
            IndexShift = indexShift;
            Width = 20;
            Color = Color.FromHex("#FF4D2A18");
            MainModel = mainModel;
            AlphaModel = alphaModel;
            SetAlpha(false, false);

            PerfomKeyCommand = new Command(PerformKey);
        }


        public void SetAlpha(bool alpha, bool user)
        {
            _alpha = alpha;
            if (MainModel != null)
            {
                if (_alpha)
                    TextAlpha = MainModel.GetAction(false, user)?.Text;
                else
                {
                    TextShift = MainModel.GetAction(true, user)?.Text;
                    Text = MainModel.GetAction(false, user)?.Text;
                }
            }
            else
            {
                if (_alpha)
                    TextAlpha = " ";
                else
                {
                    TextShift = " ";
                    Text = " ";
                }
            }
            if (AlphaModel != null)
            {
                if (!_alpha)
                    TextAlpha = AlphaModel.MainAction?.Text;
                else
                {
                    TextShift = AlphaModel.ShiftAction?.Text;
                    Text = AlphaModel.MainAction?.Text;
                }
            }
            else
            {
                if (!_alpha)
                    TextAlpha = " ";
                else
                {
                    TextShift = " ";
                    Text = " ";
                }
            }
            OnPropertyChanged(nameof(TextShift));
            OnPropertyChanged(nameof(Text));
            OnPropertyChanged(nameof(TextAlpha));
        }

        public virtual void PerformKey()
        {
            if (Parent == null) return;
            Parent.PerformKey(!_alpha ? MainModel : AlphaModel, IndexMain, IndexShift);
        }

        public void SetAction(ActionModel action, bool shift, bool alpha, bool user)
        {
            if (shift)
                MainModel.UserShiftAction = action;
            else
                MainModel.UserMainAction = action;
            if (user && !alpha)
            {
                TextShift = MainModel.GetAction(true, user).Text;
                Text = MainModel.GetAction(false, user).Text;
                OnPropertyChanged(nameof(TextShift));
                OnPropertyChanged(nameof(Text));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
