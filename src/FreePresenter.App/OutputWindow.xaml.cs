using System.Windows;

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
    }
}
