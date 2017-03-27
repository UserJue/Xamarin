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
        private bool showPreview;
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

        public bool ShowPreview
        {
            get { return showPreview; }
            set
            {
                if (value == showPreview) return;
                showPreview = value;
                Application.Current.Properties["ShowPreview"] = showPreview;
                OnPropertyChanged(nameof(ShowPreview));
            }
        }

        public Settings()
        {
            PuzzleInfos = new List<PuzzleInfo>();
        }

        public bool LoadFromProperties()
        {
            IDictionary<string, object> settingsDictionary = Application.Current.Properties;
            if (settingsDictionary.ContainsKey("ShowPreview"))
            {
                bool? show = settingsDictionary["ShowPreview"] as bool?;
                showPreview = (show != null) && show.Value;
            }
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
            Application.Current.Properties["ShowPreview"] = ShowPreview;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
