using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

            SlidesList.Items.Refresh();
            SlidesList.SelectedItem = slide;
            UpdateOutput(slide);
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

            EnsureOutputWindow();
            UpdateOutput(slide);
            _outputWindow!.Activate();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // 제목이나 내용 입력 중에는 방향키로 커서를 움직일 수 있게 둡니다.
            if (Keyboard.FocusedElement is TextBox)
            {
                return;
            }

            if (e.Key == Key.Right)
            {
                NavigateSlides(1);
                e.Handled = true;
            }
            else if (e.Key == Key.Left)
            {
                NavigateSlides(-1);
                e.Handled = true;
            }
        }

        public void NavigateSlides(int direction)
        {
            if (_slides.Count == 0)
            {
                return;
            }

            int currentIndex = SlidesList.SelectedIndex;
            int nextIndex;

            if (currentIndex < 0)
            {
                nextIndex = 0;
            }
            else
            {
                nextIndex = Math.Clamp(currentIndex + direction, 0, _slides.Count - 1);
            }

            SlidesList.SelectedIndex = nextIndex;
            SlidesList.ScrollIntoView(SlidesList.SelectedItem);

            if (SlidesList.SelectedItem is Slide slide)
            {
                EnsureOutputWindow();
                UpdateOutput(slide);
            }
        }

        private void EnsureOutputWindow()
        {
            if (_outputWindow == null || !_outputWindow.IsVisible)
            {
                _outputWindow = new OutputWindow
                {
                    Owner = this
                };

                _outputWindow.Show();
            }
        }

        private void UpdateOutput(Slide slide)
        {
            if (_outputWindow != null && _outputWindow.IsVisible)
            {
                _outputWindow.DisplaySlide(slide.Title, slide.Content);
            }
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
