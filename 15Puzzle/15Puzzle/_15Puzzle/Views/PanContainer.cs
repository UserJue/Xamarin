using System;
using Xamarin.Forms;

namespace _15Puzzle.Views
{
    public class PanContainer : ContentView
    {
        protected Action<double, double> OnMove;
        public Action<double, double> OnMoved;
        protected Action StartMove;

        public PanContainer()
        {
            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += PanGesture_PanUpdated;
            GestureRecognizers.Add(panGesture);
        }

        protected override void OnChildAdded(Element child)
        {
            base.OnChildAdded(child);
            if (Content != null)
            {
                Content.Unfocused += (sender, args) => OnMoved?.Invoke(0, 0);
            }
        }

        private void PanGesture_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    StartMove?.Invoke();
                    break;
                case GestureStatus.Running:
                    OnMove?.Invoke(e.TotalX/Content.Height,e.TotalY / Content.Height);
                    break;
                case GestureStatus.Completed:
                    OnMoved?.Invoke(e.TotalX, e.TotalY);
                    break;
                case GestureStatus.Canceled:
                    OnMoved?.Invoke(0, 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
