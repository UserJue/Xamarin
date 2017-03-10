using HP41CV.Models;
using Xamarin.Forms;

namespace HP41CV.ViewModels
{
    public class ShiftButtonViewModel : ButtonViewModel
    {
        public ShiftButtonViewModel(CalculatorViewModel parent, int indexMain, int indexShift) : base(parent, indexMain, indexShift, null, null)
        {
            Color = Color.FromHex("#FFDFC60F");
        }

        public override void PerformKey()
        {
            Parent.DoShift(null);
        }
    }
}
