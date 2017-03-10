using System.ComponentModel;
using System.Runtime.CompilerServices;
using HP41CV.Annotations;

namespace HP41CV.ViewModels
{
    public class HelpCourseVewModel : INotifyPropertyChanged
    {
        public bool Title { get; set; }
        public bool Line { get; set; }
        public bool Shift { get; set; }
        public string Key { get; set; }
        public string Text { get; set; }
        public string Remark { get; set; }
        public double Width { get; set; }

        public HelpCourseVewModel(string text, bool title = false)
        {
            Title = title;
            Text = text;
        }

        public HelpCourseVewModel(string key, string text)
        {
            Line = true;
            Key = key;
            Text = text;
            Width = 30;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
