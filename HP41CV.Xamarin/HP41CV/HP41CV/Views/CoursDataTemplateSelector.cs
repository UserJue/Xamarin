using HP41CV.ViewModels;
using Xamarin.Forms;

namespace HP41CV.Views
{
    public class CoursDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TitleDataTemplate { get; set; }
        public DataTemplate DescriptionDataTemplate { get; set; }
        public DataTemplate LineDataTemplate { get; set; }
        

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return ((HelpCourseVewModel)item).Title ? TitleDataTemplate :
                ((HelpCourseVewModel)item).Line ? LineDataTemplate : DescriptionDataTemplate;
        }
    }
}
