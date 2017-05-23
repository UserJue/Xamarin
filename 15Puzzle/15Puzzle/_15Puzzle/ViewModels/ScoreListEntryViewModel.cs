using System.ComponentModel;
using System.Runtime.CompilerServices;
using Common.Models;
using _15Puzzle.Annotations;

namespace _15Puzzle.ViewModels
{
    public class ScoreListEntryViewModel : INotifyPropertyChanged
    {
        private IScore<IntScoreValue> model;

        public string Date => model.TimeStamp.ToString();

        public string User => model.User;

        public string Name => model.Value.Name;

        public string Score => model.Value.Value.ToString();

        public ScoreListEntryViewModel(IScore<IntScoreValue> entry)
        {
            model = entry;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
