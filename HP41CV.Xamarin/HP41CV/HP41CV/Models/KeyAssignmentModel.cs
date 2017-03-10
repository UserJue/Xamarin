using System.ComponentModel;
using System.Runtime.CompilerServices;
using HP41CV.Annotations;

namespace HP41CV.Models
{
    public class KeyAssignmentModel : INotifyPropertyChanged
    {
        private int _index;
        private bool isDefault;
        private CalculatorModel _model;

        public ActionModel Action { get; internal set; }
        public int Button { get; private set; }
        public bool IsDefault { get { return isDefault; } }

        public KeyAssignmentModel(int index, CalculatorModel model)
        {
            _index = index;
            _model = model;
            Action = null;
            Button = 0;
            isDefault = true;
        }

        public KeyAssignmentModel(int index, ActionModel action, int button, bool isDefault = true)
        {
            _index = index;
            _model = null;
            Action = action;
            Button = button;
            this.isDefault = isDefault;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
