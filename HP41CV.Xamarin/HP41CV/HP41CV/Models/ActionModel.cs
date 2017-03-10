using System.ComponentModel;
using System.Runtime.CompilerServices;
using HP41CV.Annotations;
using HP41CV.Helpers;

namespace HP41CV.Models
{
    public class ActionModel : INotifyPropertyChanged
    {
        private bool isCondition;

        #region Constructors and Destructors

        public ActionModel(string text)
        {
            TextD = Text = text;
            GetValue = true;
            Option = string.Empty;
            IsUpn = true;
            IsFunction = true;
            IsSelectFunction = false;
            HasResult = false;
            Accessory = string.Empty;
        }

        #endregion

        #region Properties

        public string Accessory { get; set; }

        public string Option { get; set; }

        public bool Extension { get; set; }

        public bool GetValue { get; protected set; }

        public bool IsArithmetic { get; protected set; }

        public bool IsConversion { get; internal set; }

        public bool IsFlag { get; internal set; }

        public bool IsFormat { get; internal set; }

        public bool IsLogaritmic { get; internal set; }

        public bool IsLogical { get; internal set; }

        public bool IsCondition
        {
            get { return isCondition | IsLogical; }
            internal set { isCondition = value; }
        }

        public bool IsRegister { get; internal set; }

        public bool IsStatistic { get { return Option.Equals("Statistic"); } }

        public bool IsRobustStatistic { get { return Option.Equals("RobustStatistic"); } }

        public bool IsTrigonometric { get; protected set; }

        public string Text { get; set; }

        public string TextD { get; protected set; }

        public string Description { get; set; }

        private bool IsLight { get { return string.IsNullOrEmpty(Option); } }

        internal bool IsUpn { get; set; }

        internal bool IsFunction { get; set; }

        internal bool IsSelectFunction { get; set; }

        internal bool HasResult { get; set; }

        #endregion

        #region Public Methods

        public void Add(string text)
        {
            TextD = text;
            ActionModels.Add(text, this);
        }

        public virtual void Execute(bool noUpn = false)
        {
            if (CanExecute())
            {
                Messenger.ShowDialogAsync(AppResources.NotYet);
            }
        }

        #endregion

        #region Methods

        protected virtual bool CanExecute()
        {
            if (IsLight || MarketPlace.IsAvtivated(Option))
            {
                return true;
            }

            Messenger.ShowDialogAsync(AppResources.NotLight);
            return false;
        }

        #endregion
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
