using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using _15Puzzle.Annotations;

namespace _15Puzzle.ViewModels
{
    public class TileViewModel : INotifyPropertyChanged, IPanViewModel
    {
        public enum Direction
        {
            None,Up,Right,Down,Left
        }
        private double x0;
        private double y0;
        private double x;
        private double y;
        private bool moving;

        public Func<double, double, double, bool> canMoveX;
        public Func<double, double, double, bool> canMoveY;

        public Direction MoveDirection { get; set; }

        public int IndexX => (int) x0;

        public int IndexY => (int) y0;

        public double Delta { get; set; }

        public bool Moving
        {
            get { return moving; }
            set
            {
                if (value == moving) return;
                moving = value;
                OnPropertyChanged();
            }
        }

        public double X
        {
            get { return x; }
            set
            {
                if (value.Equals(x)) return;
                x = value;
                OnPropertyChanged();
            }
        }

        public double Y
        {
            get { return y; }
            set
            {
                if (value.Equals(y)) return;
                y = value;
                OnPropertyChanged();
            }
        }

        public string Text { get; set; }

        public TileViewModel(int index)
        {
            Text = (index+1).ToString();
            Y = (int) (index/4);
            X = index - 4*Y;
            x0 = X;
            y0 = Y;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Move(double delta, bool horizontal)
        {
            if (horizontal)
                X = x0 + delta;
            else
                Y = y0 + delta;
        }

        public void Set(double x, double y)
        {
            x0 = X = x;
            y0 = Y = y;
        }

        public void OnMove(double dX, double dY)
        {
            if (!Moving)
            {
                x0 = X;
                y0 = Y;
                Moving = true;
            }
            if ((canMoveX != null) && canMoveX(x0, y0, dX))
            {
                Delta = dX;
                MoveDirection = dX > 0 ? Direction.Right : Direction.Left;
                X = x0 + dX;
            }
            else if ((canMoveY != null) && canMoveY(x0, y0, dY))
            {
                Delta = dY;
                MoveDirection = dY > 0 ? Direction.Down : Direction.Up;
                Y = y0 + dY;
            }
        }

        public void OnMoved(double dX, double dY)
        {
            Moving = false;
            MoveDirection = Direction.None;
            Delta = 0;
        }
    }
}
