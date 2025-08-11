using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace RestaurantPOS.Controls
{
    public partial class PasswordTextBox : UserControl
    {
        public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<PasswordTextBox, string>(
                nameof(Text), string.Empty, defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

        public static readonly StyledProperty<bool> IsPasswordVisibleProperty =
            AvaloniaProperty.Register<PasswordTextBox, bool>(
                nameof(IsPasswordVisible));

        public string Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public bool IsPasswordVisible
        {
            get => GetValue(IsPasswordVisibleProperty);
            set => SetValue(IsPasswordVisibleProperty, value);
        }

        public PasswordTextBox()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void ToggleVisibility_Click(object? sender, RoutedEventArgs e)
        {
            IsPasswordVisible = !IsPasswordVisible;
        }
    }
}
