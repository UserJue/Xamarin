using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Common.Models;
using Xamarin.Forms;

namespace _15Puzzle.ViewModels
{
    public class ScoreListViewModel : INotifyPropertyChanged
    {
        private IScoreList<IntScoreValue> model;

        private bool isVisible;

        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                if (value == isVisible) return;
                isVisible = value;
                if (value)
                {
                    ScoreList.Clear();
                    FillScoreList();
                    OnPropertyChanged(nameof(ScoreList));
                }
                OnPropertyChanged(nameof(IsVisible));
                OnPropertyChanged(nameof(IsBack1Visible));
                OnPropertyChanged(nameof(IsBack2Visible));
            }
        }

        private void FillScoreList()
        {
            foreach (IScore<IntScoreValue> score in model)
            {
                ScoreList.Add(new ScoreListEntryViewModel(score));
            }
        }

        private bool back1;

        public bool IsBack1Visible
        {
            get { return isVisible && back1; }
            set
            {
                if (value == isVisible) return;
                isVisible = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsBack2Visible));
            }
        }

        private bool isBack2Visible;

        public bool IsBack2Visible
        {
            get { return isVisible && !back1; }
            set
            {
                if (value == !back1) return;
                back1 = !value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsBack1Visible));
            }
        }

        public Action BackAction;

        public Action AboutAction;

        public ICommand BackCommand { get; private set; }

        public ICommand AboutCommand { get; private set; }

        public string BackText { get; set; }

        public string AboutText { get; set; }

        public string ScoreListCapture => AppResource.ScoreListCapture;

        public ObservableCollection<ScoreListEntryViewModel> ScoreList { get; } 

        public ScoreListViewModel(IScoreList<IntScoreValue> scoreList )
        {
            model = scoreList;
            back1 = true;
            BackCommand = new Command(() => BackAction?.Invoke());
            AboutCommand = new Command(() => AboutAction?.Invoke());
            ScoreList = new ObservableCollection<ScoreListEntryViewModel>();
            FillScoreList();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

