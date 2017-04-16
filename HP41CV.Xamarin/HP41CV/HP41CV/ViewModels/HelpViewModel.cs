using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using HP41CV.Annotations;
using HP41CV.Models;
using Xamarin.Forms;

namespace HP41CV.ViewModels
{
    public class HelpViewModel : INotifyPropertyChanged
    {
        private bool isVisible;

        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                if (value == isVisible) return;
                isVisible = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsBack1Visible));
                OnPropertyChanged(nameof(IsBack2Visible));
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

        public ICommand BackCommand { get; private set; }

        public string BackText { get; set; }

        public Action AboutAction;
        private int selectedIndex;

        public ICommand AboutCommand { get; private set; }

        public string AboutText { get; set; }

        public Action OptionAction;

        public ICommand OptionCommand { get; private set; }

        public string OptionText { get; set; }

        public string Title { get; set; }

        public string Topic { get; set; }

        public bool KeysVisible { get; set; }

        public bool CourseVisible { get; set; }

        public List<string> Topics { get; set; }

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                if (value == selectedIndex) return;
                selectedIndex = value;
                KeysVisible = selectedIndex == 0;
                CourseVisible = selectedIndex == 1;
                OnPropertyChanged(nameof(KeysVisible));
                OnPropertyChanged(nameof(CourseVisible));
            }
        }

        public Collection<HelpKeyViewModel> Keys { get; set; } 
        public Collection<HelpCourseVewModel> Course { get; set; } 

        public HelpViewModel()
        {
            bool useAppResource = ((App)App.Current).UseAppResource;
            back1 = true;
            Title = !useAppResource ? "HelpTitle" : AppResources.HelpTitle;
            AboutText = !useAppResource ? "AboutCommand" : AppResources.AboutCommand;
            OptionText = !useAppResource ? "OptionCommand" : AppResources.OptionCommand;
            Topics = new List<string> { !useAppResource ? "FunctionTitle" : AppResources.FunctionTitle, !useAppResource ? "UPNCourseTitle" : AppResources.UPNCourseTitle, };
            SelectedIndex = 0;
            KeysVisible = true;
            CourseVisible = false;
            BackCommand = new Command(() => BackAction?.Invoke());
            AboutCommand = new Command(() => AboutAction?.Invoke());
            OptionCommand = new Command(() => OptionAction?.Invoke());

            Keys = new Collection<HelpKeyViewModel>();
            Keys.Add(new HelpKeyViewModel(null,AppResources.HelpStandard));
            foreach (var action in ActionModels.GetActionDescritions(m => !string.IsNullOrEmpty(m.Description)))
            {
                Keys.Add(new HelpKeyViewModel(action.Item1,action.Item2));
            }
            Keys.Add(new HelpKeyViewModel(null, AppResources.HelpExtention));
            foreach (var action in ActionModels.GetExtentionActionDescritions(m => !string.IsNullOrEmpty(m.Description)))
            {
                Keys.Add(new HelpKeyViewModel(action.Item1, action.Item2));
            }
            Course = new Collection<HelpCourseVewModel>
            {
                new HelpCourseVewModel(!useAppResource ? "Chapter1Title" : AppResources.Chapter1Title, true),
                new HelpCourseVewModel(!useAppResource ? "Chapter1Description" : AppResources.Chapter1Description),
                new HelpCourseVewModel(!useAppResource ? "Chapter1Example1" : AppResources.Chapter1Example1),
                new HelpCourseVewModel("2","2_"),
                new HelpCourseVewModel("5","25_"),
                new HelpCourseVewModel("1/X", "0.0400") {Remark = !useAppResource ? "result" : AppResources.ResultText},
                new HelpCourseVewModel(!useAppResource ? "Chapter1Example2" : AppResources.Chapter1Example2),
                new HelpCourseVewModel("4", "4_"),
                new HelpCourseVewModel("5","45_"),
                new HelpCourseVewModel("SIN", "0.7071") {Remark = !useAppResource ? "result" : AppResources.ResultText},
                new HelpCourseVewModel(!useAppResource ? "Chapter2Title" : AppResources.Chapter2Title, true),
                new HelpCourseVewModel(!useAppResource ? "Chapter2Description" : AppResources.Chapter2Description),
                new HelpCourseVewModel(!useAppResource ? "Chapter2Example1" : AppResources.Chapter2Example1),
                new HelpCourseVewModel("1", "1_"),
                new HelpCourseVewModel("5","15_"),
                new HelpCourseVewModel("ENTER↑", "15.0000") {Width = 60},
                new HelpCourseVewModel("7", "7_"),
                new HelpCourseVewModel("+", "22.0000") {Remark = !useAppResource ? "result" : AppResources.ResultText},
                new HelpCourseVewModel(!useAppResource ? "Chapter2Example2" : AppResources.Chapter2Example2),
                new HelpCourseVewModel("4", "4_"),
                new HelpCourseVewModel("ENTER↑", "4.0000") {Width = 60},
                new HelpCourseVewModel("3", "3_"),
                new HelpCourseVewModel("Y˟", "64.0000") {Shift = true, Remark = !useAppResource ? "result" : AppResources.ResultText},
                new HelpCourseVewModel(!useAppResource ? "Chapter3Title" : AppResources.Chapter3Title, true),
                new HelpCourseVewModel(!useAppResource ? "Chapter3Description" : AppResources.Chapter3Description),
                new HelpCourseVewModel(!useAppResource ? "Chapter3Example" : AppResources.Chapter3Example),
                new HelpCourseVewModel("5","5_"),
                new HelpCourseVewModel("ENTER↑", "5.0000") {Width = 60},
                new HelpCourseVewModel("7", "7_"),
                new HelpCourseVewModel("+", "12.0000") {Remark = !useAppResource ? "result" : AppResources.IntermediateText},
                new HelpCourseVewModel("3","3_"),
                new HelpCourseVewModel("ENTER↑", "3.0000") {Width = 60},
                new HelpCourseVewModel("1", "1_"),
                new HelpCourseVewModel("-", "2.0000") {Remark = !useAppResource ? "result" : AppResources.IntermediateText},
                new HelpCourseVewModel("x", "24.0000") {Remark = !useAppResource ? "result" : AppResources.ResultText},
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
