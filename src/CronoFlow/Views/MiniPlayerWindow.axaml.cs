using Avalonia;
using Avalonia.Controls;

namespace TimeFlow.Views;

public partial class MiniPlayerWindow : Window
{
    public static MiniPlayerWindow? Instance { get; private set; }

    public MiniPlayerWindow()
    {
        InitializeComponent();
        Instance = this;

        Opened += (_, _) =>
        {
            var workArea = Screens.Primary?.WorkingArea ?? new PixelRect(0, 0, 1920, 1080);
            Position = new PixelPoint(
                workArea.X + workArea.Width - (int)Width - 20,
                workArea.Y + workArea.Height - (int)Height - 20);
        };
    }
}
