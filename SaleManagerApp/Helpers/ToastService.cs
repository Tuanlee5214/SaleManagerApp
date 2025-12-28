using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace SaleManagerApp.Helpers
{
    public static class ToastService
    {
        private static Grid _rootGrid;

        // Gán Grid gốc của MainLayout/HomePage vào đây khi khởi tạo
        public static void Initialize(Grid rootGrid)
        {
            _rootGrid = rootGrid;
        }

        public static void Show(string message, int durationSeconds = 2)
        {
            // Nếu đang ở trong Window riêng, hiển thị trên Window đó
            var activeWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
            if (activeWindow != null && activeWindow.Content is Grid windowGrid)
            {
                ShowOnGrid(windowGrid, message, Color.FromRgb(16, 185, 129), durationSeconds);
                return;
            }

            // Nếu không, hiển thị trên _rootGrid
            if (_rootGrid == null) return;
            ShowOnGrid(_rootGrid, message, Color.FromRgb(16, 185, 129), durationSeconds);
        }

        public static void ShowError(string message, int durationSeconds = 2)
        {
            // Nếu đang ở trong Window riêng, hiển thị trên Window đó
            var activeWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
            if (activeWindow != null && activeWindow.Content is Grid windowGrid)
            {
                ShowOnGrid(windowGrid, message, Color.FromRgb(239, 68, 68), durationSeconds);
                return;
            }

            // Nếu không, hiển thị trên _rootGrid
            if (_rootGrid == null) return;
            ShowOnGrid(_rootGrid, message, Color.FromRgb(239, 68, 68), durationSeconds);
        }

        public static void ShowErrorLogin(string message, int durationSeconds = 2)
        {
            if (_rootGrid == null) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                var toast = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(239, 68, 68)),
                    CornerRadius = new CornerRadius(10),
                    Padding = new Thickness(20, 20, 20, 20),
                    MinWidth = 200,
                    MaxWidth = 400,
                    Child = new TextBlock
                    {
                        Text = message,
                        Foreground = Brushes.White,
                        FontWeight = FontWeights.Bold,
                        HorizontalAlignment = HorizontalAlignment.Center
                    },
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(10, 10, 10, 140),
                    Opacity = 0
                };

                Panel.SetZIndex(toast, 9999);
                _rootGrid.Children.Add(toast);

                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.3));
                var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.3))
                {
                    BeginTime = TimeSpan.FromSeconds(durationSeconds)
                };

                var sb = new Storyboard();
                sb.Children.Add(fadeIn);
                sb.Children.Add(fadeOut);

                Storyboard.SetTarget(fadeIn, toast);
                Storyboard.SetTargetProperty(fadeIn, new PropertyPath(Border.OpacityProperty));

                Storyboard.SetTarget(fadeOut, toast);
                Storyboard.SetTargetProperty(fadeOut, new PropertyPath(Border.OpacityProperty));

                sb.Completed += (s, e) => _rootGrid.Children.Remove(toast);
                sb.Begin();
            });
        }

        // Phương thức helper để hiển thị toast trên bất kỳ Grid nào
        private static void ShowOnGrid(Grid targetGrid, string message, Color backgroundColor, int durationSeconds)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var toast = new Border
                {
                    Background = new SolidColorBrush(backgroundColor),
                    CornerRadius = new CornerRadius(10),
                    Padding = new Thickness(20, 20, 20, 20),
                    MinWidth = 200,
                    MaxWidth = 400,
                    Child = new TextBlock
                    {
                        Text = message,
                        Foreground = Brushes.White,
                        FontWeight = FontWeights.Bold,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        TextWrapping = TextWrapping.Wrap
                    },
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(10, 10, 10, 10),
                    Opacity = 0
                };

                // ĐÚNG CÁCH: Dùng Panel.SetZIndex thay vì gán trực tiếp
                Panel.SetZIndex(toast, 9999);

                targetGrid.Children.Add(toast);

                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.3));
                var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.3))
                {
                    BeginTime = TimeSpan.FromSeconds(durationSeconds)
                };

                var sb = new Storyboard();
                sb.Children.Add(fadeIn);
                sb.Children.Add(fadeOut);

                Storyboard.SetTarget(fadeIn, toast);
                Storyboard.SetTargetProperty(fadeIn, new PropertyPath(Border.OpacityProperty));

                Storyboard.SetTarget(fadeOut, toast);
                Storyboard.SetTargetProperty(fadeOut, new PropertyPath(Border.OpacityProperty));

                sb.Completed += (s, e) => targetGrid.Children.Remove(toast);
                sb.Begin();
            });
        }
    }
}