using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;
using _15Puzzle.Models;

namespace _15Puzzle.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private bool isVisible;

        private Settings model;

        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                if (value == isVisible) return;
                isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
                OnPropertyChanged(nameof(IsBack1Visible));
                OnPropertyChanged(nameof(IsBack2Visible));
            }
        }

        public int ShowPreviewIndex
        {
            get { return model.ShowPreview ? 1 : 0; }
            set
            {
                if (showPreview != value)
                {
                    model.ShowPreview = value == 1;
                }
            }
        }

        public ICollection<string> ShowPreviewTexts { get; private set; }

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

        public ICommand BackCommand { get; private set; }

        public ICommand AboutCommand { get; private set; }

        public string BackText { get; set; }

        public string AboutText { get; set; }

        public string PictureCapture => AppResource.PictureCapture;

        public Collection<PuzzleInfoViewModel> PuzzleInfos { get; }

        public PuzzleInfoViewModel SelectedPuzzleInfo
        {
            get { return selectedPuzzleInfo; }
            set
            {
                if (Equals(selectedPuzzleInfo,value)) return;
                selectedPuzzleInfo = value;
                if ((selectedPuzzleInfo == null) ||Equals(model.PuzzleInfo,selectedPuzzleInfo.Model)) return;
                model.PuzzleInfo = selectedPuzzleInfo.Model;
                model.SaveToProperties();
            }
        }

        public Action AboutAction;
        private PuzzleInfoViewModel selectedPuzzleInfo;
        private int showPreview;

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsViewModel(Settings settings)
        {
            model = settings;
            back1 = true;
            BackCommand = new Command(() => BackAction?.Invoke());
            AboutCommand = new Command(() => AboutAction?.Invoke());
            PuzzleInfos = new Collection<PuzzleInfoViewModel>();
            foreach (var puzzleInfo in settings.PuzzleInfos)
            {
                var viewModel = new PuzzleInfoViewModel(puzzleInfo);
                PuzzleInfos.Add(viewModel);
                if (puzzleInfo.Name == model.PuzzleInfo.Name)
                    SelectedPuzzleInfo = viewModel;
            }
            ShowPreviewTexts = new List<string> { "show preview", "hide preview"};
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
