using System.Windows;
using System.Windows.Input;

namespace FreePresenter.App
{
    public partial class OutputWindow : Window
    {
        public OutputWindow()
        {
            InitializeComponent();
        }

        public void DisplaySlide(string title, string content)
        {
            TitleText.Text = title;
            BodyText.Text = content;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
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
    }
}
