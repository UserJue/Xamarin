using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using _15Puzzle.Annotations;

namespace _15Puzzle.Models
{
    public class Settings : INotifyPropertyChanged
    {
        private PuzzleInfo puzzleInfo;
        public List<PuzzleInfo> PuzzleInfos { get; }

        public PuzzleInfo PuzzleInfo
        {
            get { return puzzleInfo; }
            set
            {
                if (Equals(value, puzzleInfo)) return;
                puzzleInfo = value;
                OnPropertyChanged(nameof(PuzzleInfo));
            }
        }

        public Settings()
        {
            PuzzleInfos = new List<PuzzleInfo>();
        }

        public bool LoadFromProperties()
        {
            IDictionary<string, object> settingsDictionary = Application.Current.Properties;
            if (settingsDictionary.ContainsKey("SelectedName"))
            {
                var name = settingsDictionary["SelectedName"] as string;
                foreach (var info in PuzzleInfos)
                {
                    if (info.Name == name)
                    {
                        puzzleInfo = info;
                        return true;
                    }
                }
            }
            return false;
        }

        public void SaveToProperties()
        {
            Application.Current.Properties["SelectedName"] = PuzzleInfo.Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
