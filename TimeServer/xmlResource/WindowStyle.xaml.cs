using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TimeClient
{
    public partial class WindowStyle
    {
        private Window GetWindowParentInWindowStyle(DependencyObject sender)
        {
            Window result;

            Grid parrentGrid = VisualTreeHelper.GetParent(sender as DependencyObject) as Grid;

            parrentGrid = VisualTreeHelper.GetParent(parrentGrid as DependencyObject) as Grid;

            Border parrentBorder = VisualTreeHelper.GetParent(parrentGrid as DependencyObject) as Border;

            parrentBorder = VisualTreeHelper.GetParent(parrentBorder as DependencyObject) as Border;

            result = VisualTreeHelper.GetParent(parrentBorder as DependencyObject) as Window;

            return result;
        }

        private void BdrMinimeze_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Border currentBorder = sender as Border;

            GetWindowParentInWindowStyle(currentBorder as DependencyObject).WindowState = WindowState.Minimized;
        }

        private void BdrClose_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Border currentBorder = sender as Border;

            GetWindowParentInWindowStyle(currentBorder as DependencyObject).Close();
        }

        private void LbTitle_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Label currentLabel = sender as Label;
            GetWindowParentInWindowStyle(currentLabel as DependencyObject).DragMove();
        }
    }
}
