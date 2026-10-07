using System.Windows;
using System.Windows.Input;
using System.Linq;
using Forms = System.Windows.Forms;


namespace FreePresenter.App
{
    public partial class OutputWindow : Window
    {
        public OutputWindow()
        {
            InitializeComponent();
        }

        public void DisplaySlide(string lyrics)
        {
            LyricsText.Text = lyrics;
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Owner is not MainWindow mainWindow)
            {
                return;
            }

            if (e.Key == Key.Right)
            {
                mainWindow.NavigateSlides(1);
                e.Handled = true;
            }
            else if (e.Key == Key.Left)
            {
                mainWindow.NavigateSlides(-1);
                e.Handled = true;
            }
        }

        public void MoveToSecondaryDisplay()
        {
            Forms.Screen? targetScreen =
                Forms.Screen.AllScreens.FirstOrDefault(screen => !screen.Primary)
                ?? Forms.Screen.PrimaryScreen;

            if (targetScreen == null)
            {
                return;
            }

            WindowStartupLocation = WindowStartupLocation.Manual;
            WindowState = WindowState.Normal;
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;

            Left = targetScreen.Bounds.Left;
            Top = targetScreen.Bounds.Top;
            Width = targetScreen.Bounds.Width;
            Height = targetScreen.Bounds.Height;

            WindowState = WindowState.Maximized;
        }
    }
}
