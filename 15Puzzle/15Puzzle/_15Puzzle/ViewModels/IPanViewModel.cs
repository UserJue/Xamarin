namespace _15Puzzle.ViewModels
{
    public interface IPanViewModel
    {
        void OnMove(double dX, double dY);
        void OnMoved(double dX, double dY);
    }
}
