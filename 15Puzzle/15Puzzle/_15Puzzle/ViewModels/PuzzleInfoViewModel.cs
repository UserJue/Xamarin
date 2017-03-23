using System.ComponentModel;
using System.Runtime.CompilerServices;
using _15Puzzle.Annotations;
using _15Puzzle.Models;

namespace _15Puzzle.ViewModels
{
    public class PuzzleInfoViewModel : INotifyPropertyChanged
    {
        private PuzzleInfo model;

        public int Tiles => model.Tiles;

        public string Name => model.Name;

        public string Picture => model.Picture;

        public PuzzleInfo Model => model;

        public PuzzleInfoViewModel(PuzzleInfo model)
        {
            this.model = model;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
