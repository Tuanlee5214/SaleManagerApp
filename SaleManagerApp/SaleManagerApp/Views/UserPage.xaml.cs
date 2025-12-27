using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using SaleManagerApp.ViewModels;
using SaleManagerApp.Model;

namespace SaleManagerApp.Views
{
    public partial class UserPage : UserControl
    {
        private UserPageViewModel _viewModel;

        public UserPage()
        {
            InitializeComponent();
            _viewModel = new UserPageViewModel();
            this.DataContext = _viewModel;
        }

        private void CheckInButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedStaff = StaffDataGrid.SelectedItem as Staff;
            if (selectedStaff == null)
            {
                MessageBox.Show("Vui lòng chọn một nhân viên để chấm công!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Hiển thị dialog nhập mã nhân viên
            string inputEmployeeId = ShowEmployeeIdInputDialog();

            if (string.IsNullOrWhiteSpace(inputEmployeeId))
            {
                return; // User đã cancel
            }

            // Kiểm tra mã nhân viên có khớp không
            if (inputEmployeeId.Trim().ToUpper() != selectedStaff.StaffId.Trim().ToUpper())
            {
                MessageBox.Show("Mã nhân viên không khớp! Vui lòng kiểm tra lại.", "Lỗi xác thực",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Thực hiện chấm công
            _viewModel.CheckIn(selectedStaff.StaffId);
        }

        private void CheckOutButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedStaff = StaffDataGrid.SelectedItem as Staff;
            if (selectedStaff == null)
            {
                MessageBox.Show("Vui lòng chọn một nhân viên để chấm công!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // KIỂM TRA: Phải chấm công vào trước
            if (!selectedStaff.CheckInTime.HasValue)
            {
                MessageBox.Show("Nhân viên chưa chấm công vào!\n\nVui lòng chấm công vào trước khi chấm công ra.",
                    "Không thể chấm công ra",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Hiển thị dialog nhập mã nhân viên
            string inputEmployeeId = ShowEmployeeIdInputDialog();

            if (string.IsNullOrWhiteSpace(inputEmployeeId))
            {
                return; // User đã cancel
            }

            // Kiểm tra mã nhân viên có khớp không
            if (inputEmployeeId.Trim().ToUpper() != selectedStaff.StaffId.Trim().ToUpper())
            {
                MessageBox.Show("Mã nhân viên không khớp! Vui lòng kiểm tra lại.", "Lỗi xác thực",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Thực hiện chấm công
            _viewModel.CheckOut(selectedStaff.StaffId);
        }

        // DIALOG NHẬP MÃ NHÂN VIÊN
        private string ShowEmployeeIdInputDialog()
        {
            Window inputDialog = new Window
            {
                Title = "Xác thực mã nhân viên",
                Width = 400,
                Height = 220,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = WindowStyle.None,
                Background = System.Windows.Media.Brushes.Transparent,
                AllowsTransparency = true
            };

            Border border = new Border
            {
                Background = System.Windows.Media.Brushes.White,
                CornerRadius = new CornerRadius(15),
                Padding = new Thickness(30),
                BorderBrush = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(220, 220, 220)),
                BorderThickness = new Thickness(1)
            };

            StackPanel stackPanel = new StackPanel();

            // Title
            TextBlock title = new TextBlock
            {
                Text = "Nhập mã nhân viên",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 20),
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(17, 24, 39))
            };

            // TextBox
            TextBox employeeIdTextBox = new TextBox
            {
                Height = 45,
                FontSize = 16,
                Padding = new Thickness(15, 0, 15, 0),
                VerticalContentAlignment = VerticalAlignment.Center,
                MaxLength = 7,
                Background = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(248, 248, 248)),
                BorderThickness = new Thickness(0),
                Margin = new Thickness(0, 0, 0, 20)
            };

            // Style cho TextBox
            Border textBoxBorder = new Border
            {
                CornerRadius = new CornerRadius(8),
                Background = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(248, 248, 248)),
                Margin = new Thickness(0, 0, 0, 20),
                Child = employeeIdTextBox
            };

            // Buttons
            Grid buttonGrid = new Grid();
            buttonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            buttonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(10) });
            buttonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Cancel Button
            Button cancelButton = new Button
            {
                Content = "Hủy",
                Height = 40,
                Background = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(128, 128, 128)),
                Foreground = System.Windows.Media.Brushes.White,
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Cursor = System.Windows.Input.Cursors.Hand,
                BorderThickness = new Thickness(0)
            };

            cancelButton.Template = CreateRoundedButtonTemplate();
            cancelButton.Click += (s, e) =>
            {
                inputDialog.Tag = null;
                inputDialog.Close();
            };

            // Confirm Button
            Button confirmButton = new Button
            {
                Content = "Xác nhận",
                Height = 40,
                Background = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(64, 123, 255)),
                Foreground = System.Windows.Media.Brushes.White,
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Cursor = System.Windows.Input.Cursors.Hand,
                BorderThickness = new Thickness(0)
            };

            confirmButton.Template = CreateRoundedButtonTemplate();
            confirmButton.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(employeeIdTextBox.Text))
                {
                    MessageBox.Show("Vui lòng nhập mã nhân viên!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                inputDialog.Tag = employeeIdTextBox.Text;
                inputDialog.Close();
            };

            Grid.SetColumn(cancelButton, 0);
            Grid.SetColumn(confirmButton, 2);

            buttonGrid.Children.Add(cancelButton);
            buttonGrid.Children.Add(confirmButton);

            stackPanel.Children.Add(title);
            stackPanel.Children.Add(textBoxBorder);
            stackPanel.Children.Add(buttonGrid);

            border.Child = stackPanel;
            inputDialog.Content = border;

            // Focus vào TextBox khi mở
            inputDialog.Loaded += (s, e) => employeeIdTextBox.Focus();

            // Enter để confirm
            employeeIdTextBox.KeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Enter)
                {
                    confirmButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
            };

            inputDialog.ShowDialog();

            return inputDialog.Tag as string;
        }

        private ControlTemplate CreateRoundedButtonTemplate()
        {
            var template = new ControlTemplate(typeof(Button));

            var borderFactory = new System.Windows.FrameworkElementFactory(typeof(Border));
            borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(8));
            borderFactory.SetValue(Border.BackgroundProperty,
                new System.Windows.TemplateBindingExtension(Button.BackgroundProperty));
            borderFactory.SetValue(Border.PaddingProperty, new Thickness(10));

            var contentPresenterFactory = new System.Windows.FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenterFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentPresenterFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

            borderFactory.AppendChild(contentPresenterFactory);
            template.VisualTree = borderFactory;

            return template;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.SearchStaff(SearchBox.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ThemNhanVien();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Logic khi chọn row
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            string employeeId = btn.Tag as string;
            if (string.IsNullOrEmpty(employeeId)) return;

            ContextMenu contextMenu = new ContextMenu
            {
                FontSize = 14,
                MinWidth = 150
            };

            MenuItem editItem = new MenuItem
            {
                Header = "✏️ Sửa thông tin",
                Padding = new Thickness(10, 8, 10, 8)
            };
            editItem.Click += (s, args) =>
            {
                _viewModel.EditStaff(employeeId);
            };

            MenuItem deleteItem = new MenuItem
            {
                Header = "🗑️ Xóa nhân viên",
                Foreground = System.Windows.Media.Brushes.Red,
                Padding = new Thickness(10, 8, 10, 8)
            };
            deleteItem.Click += (s, args) =>
            {
                _viewModel.DeleteStaff(employeeId);
            };

            contextMenu.Items.Add(editItem);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(deleteItem);

            contextMenu.PlacementTarget = btn;
            contextMenu.Placement = PlacementMode.Bottom;
            contextMenu.IsOpen = true;
        }
    }
}