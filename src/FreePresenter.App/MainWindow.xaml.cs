using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfMessageBox = System.Windows.MessageBox;
using WpfTextBox = System.Windows.Controls.TextBox;

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

        private void GenerateSlides_Click(object sender, RoutedEventArgs e)
        {
            string lyrics = LyricsInput.Text
                .Replace("\r\n", "\n")
                .Replace('\r', '\n')
                .Trim();

            if (string.IsNullOrWhiteSpace(lyrics))
            {
                WpfMessageBox.Show("먼저 가사를 입력하세요.");
                return;
            }

            _slides.Clear();

            // 빈 줄(공백만 있는 줄 포함)을 기준으로 나눕니다.
            string[] blocks = Regex.Split(lyrics, @"\n[ \t]*\n+");

            foreach (string block in blocks)
            {
                string slideLyrics = string.Join(
                    Environment.NewLine,
                    block.Split('\n')
                        .Select(line => line.Trim())
                        .Where(line => !string.IsNullOrWhiteSpace(line)));

                if (!string.IsNullOrWhiteSpace(slideLyrics))
                {
                    _slides.Add(new Slide(_slides.Count + 1, slideLyrics));
                }
            }

            SlideCountText.Text = $"슬라이드 목록 ({_slides.Count}장)";

            if (_slides.Count == 0)
            {
                WpfMessageBox.Show("슬라이드로 나눌 가사를 찾지 못했습니다.");
                return;
            }

            SlidesList.SelectedIndex = 0;
            SlidesList.ScrollIntoView(SlidesList.SelectedItem);

            if (_outputWindow != null && _outputWindow.IsVisible)
            {
                _outputWindow.DisplaySlide(_slides[0].Lyrics);
            }
        }

        private void SlidesList_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (SlidesList.SelectedItem is Slide slide &&
                _outputWindow != null &&
                _outputWindow.IsVisible)
            {
                _outputWindow.DisplaySlide(slide.Lyrics);
            }
        }

        private void ShowOutput_Click(object sender, RoutedEventArgs e)
        {
            if (SlidesList.SelectedItem is not Slide slide)
            {
                WpfMessageBox.Show("먼저 가사를 입력하고 슬라이드를 만들어 주세요.");
                return;
            }

            EnsureOutputWindow();
            _outputWindow!.DisplaySlide(slide.Lyrics);
            _outputWindow.Activate();
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

            // 가사 입력 중에는 방향키를 커서 이동에 사용합니다.
            if (Keyboard.FocusedElement is WpfTextBox)
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
            int nextIndex = currentIndex < 0
                ? 0
                : Math.Clamp(currentIndex + direction, 0, _slides.Count - 1);

            SlidesList.SelectedIndex = nextIndex;
            SlidesList.ScrollIntoView(SlidesList.SelectedItem);

            if (SlidesList.SelectedItem is Slide slide)
            {
                EnsureOutputWindow();
                _outputWindow!.DisplaySlide(slide.Lyrics);
            }
        }

        private void EnsureOutputWindow()
        {
            if (_outputWindow != null && _outputWindow.IsVisible)
            {
                return;
            }

            _outputWindow = new OutputWindow
            {
                Owner = this
            };

            _outputWindow.Closed += (_, _) => _outputWindow = null;
            _outputWindow.MoveToSecondaryDisplay();
            _outputWindow.Show();
        }

        private sealed class Slide
        {
            public int Number { get; }
            public string Lyrics { get; }

            public Slide(int number, string lyrics)
            {
                Number = number;
                Lyrics = lyrics;
            }

            public override string ToString()
            {
                string firstLine = Lyrics
                    .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault() ?? "";

                if (firstLine.Length > 28)
                {
                    firstLine = firstLine[..28] + "…";
                }

                return $"{Number:00}  {firstLine}";
            }
        }
    }
}
