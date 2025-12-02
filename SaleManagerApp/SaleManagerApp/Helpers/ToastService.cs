using System;
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
            if (_rootGrid == null) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                var toast = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(16, 185, 129)), // màu xanh success
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
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(10, 10, 10, 10),
                    Opacity = 0
                };

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
    }
}
