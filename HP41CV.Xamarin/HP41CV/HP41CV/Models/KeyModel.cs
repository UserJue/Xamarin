using System.ComponentModel;
using System.Runtime.CompilerServices;
using HP41CV.Annotations;

namespace HP41CV.Models
{
    public class KeyModel : INotifyPropertyChanged
    {
        private ActionModel _mainAction;
        private ActionModel _shiftAction;

        public ActionModel MainAction { get; private set; }
        public ActionModel ShiftAction { get; private set; }
        public ActionModel UserMainAction { get { return _mainAction ?? MainAction; } set { _mainAction = value; } }
        public ActionModel UserShiftAction { get { return _shiftAction ?? ShiftAction; } set { _shiftAction = value; } }

        public KeyModel(ActionModel mainAction, ActionModel shiftAction)
        {
            _mainAction = null;
            _shiftAction = null;
            MainAction = mainAction;
            ShiftAction = shiftAction;
        }

        public ActionModel GetAction(bool shift, bool user)
        {
            if (user)
                return shift ? UserShiftAction : UserMainAction;
            return shift ? ShiftAction : MainAction;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
