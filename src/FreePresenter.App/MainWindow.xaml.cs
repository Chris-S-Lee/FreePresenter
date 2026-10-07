using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace FreePresenter.App
{
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<Slide> _slides = new();
        private OutputWindow? _outputWindow;

        public MainWindow()
        {
            InitializeComponent();
            SlidesList.ItemsSource = _slides;
        }

        private void AddSlide_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                title = $"슬라이드 {_slides.Count + 1}";
            }

            var slide = new Slide(title, ContentInput.Text);
            _slides.Add(slide);

            SlidesList.SelectedItem = slide;
            SlidesList.ScrollIntoView(slide);
        }

        private void UpdateSlide_Click(object sender, RoutedEventArgs e)
        {
            if (SlidesList.SelectedItem is not Slide slide)
            {
                MessageBox.Show("먼저 수정할 슬라이드를 목록에서 선택하세요.");
                return;
            }

            string title = TitleInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                title = $"슬라이드 {SlidesList.SelectedIndex + 1}";
            }

            slide.Title = title;
            slide.Content = ContentInput.Text;

            // 제목이 목록에 바로 반영되도록 새로 고칩니다.
            SlidesList.Items.Refresh();
            SlidesList.SelectedItem = slide;
        }

        private void DeleteSlide_Click(object sender, RoutedEventArgs e)
        {
            if (SlidesList.SelectedItem is not Slide slide)
            {
                MessageBox.Show("먼저 삭제할 슬라이드를 목록에서 선택하세요.");
                return;
            }

            int index = SlidesList.SelectedIndex;
            _slides.Remove(slide);

            if (_slides.Count > 0)
            {
                SlidesList.SelectedIndex = Math.Min(index, _slides.Count - 1);
            }
            else
            {
                TitleInput.Clear();
                ContentInput.Clear();
            }
        }

        private void SlidesList_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (SlidesList.SelectedItem is Slide slide)
            {
                TitleInput.Text = slide.Title;
                ContentInput.Text = slide.Content;
            }
        }

        private void ShowOutput_Click(object sender, RoutedEventArgs e)
        {
            if (SlidesList.SelectedItem is not Slide slide)
            {
                MessageBox.Show("먼저 출력할 슬라이드를 목록에서 선택하세요.");
                return;
            }

            if (_outputWindow == null || !_outputWindow.IsVisible)
            {
                _outputWindow = new OutputWindow();
                _outputWindow.Show();
            }

            _outputWindow.DisplaySlide(slide.Title, slide.Content);
            _outputWindow.Activate();
        }

        private sealed class Slide
        {
            public string Title { get; set; }
            public string Content { get; set; }

            public Slide(string title, string content)
            {
                Title = title;
                Content = content;
            }

            public override string ToString()
            {
                return Title;
            }
        }
    }
}
