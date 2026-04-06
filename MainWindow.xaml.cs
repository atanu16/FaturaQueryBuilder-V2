using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace FaturaQueryBuilder
{
    public partial class MainWindow : Window
    {
        private bool _isDarkMode;
        private static readonly Regex FaturaPattern = new(
            @"^\s*Fatura\s+\d+\s+cliente\s+\d+\s*$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public MainWindow()
        {
            InitializeComponent();
        }

        // ───────────────────── Input Changed ─────────────────────

        private void InputTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var text = InputTextBox.Text ?? "";
            InputPlaceholder.Visibility = string.IsNullOrEmpty(text)
                ? Visibility.Visible
                : Visibility.Collapsed;

            var lines = text.Split('\n', StringSplitOptions.None);
            var nonEmpty = lines.Count(l => !string.IsNullOrWhiteSpace(l));

            LineCountText.Text = $"{nonEmpty} line{(nonEmpty != 1 ? "s" : "")}";
            CharCountText.Text = $"{text.Length:N0} chars";

            // Hide any previous error when user types
            ErrorBorder.Visibility = Visibility.Collapsed;
        }

        // ───────────────────── Process ─────────────────────

        private async void ProcessButton_Click(object sender, RoutedEventArgs e)
        {
            var rawInput = InputTextBox.Text?.Trim();

            if (string.IsNullOrEmpty(rawInput))
            {
                ShowError("Please paste some Fatura lines before processing.");
                return;
            }

            // Show loading
            LoadingBorder.Visibility = Visibility.Visible;
            ErrorBorder.Visibility = Visibility.Collapsed;
            OutputTextBox.Text = "";
            QueryCountBadge.Visibility = Visibility.Collapsed;
            OutputCharCount.Text = "";

            // Simulate brief processing delay for UX
            await Task.Delay(350);

            var lines = rawInput
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToList();

            // Validate lines
            var invalidLines = lines
                .Select((line, idx) => new { line, idx = idx + 1 })
                .Where(x => !FaturaPattern.IsMatch(x.line))
                .ToList();

            if (invalidLines.Count > 0 && invalidLines.Count == lines.Count)
            {
                LoadingBorder.Visibility = Visibility.Collapsed;
                ShowError($"No valid lines found. Expected format: \"Fatura <number> cliente <number>\"");
                return;
            }

            // Build query — skip invalid, process valid
            var validLines = lines.Where(l => FaturaPattern.IsMatch(l)).ToList();

            var queryParts = validLines.Select(line => $"Subject: \"{line}\"");
            var result = string.Join(" OR ", queryParts);

            await Task.Delay(150); // smooth finish

            LoadingBorder.Visibility = Visibility.Collapsed;
            OutputTextBox.Text = result;

            // Stats
            QueryCountBadge.Visibility = Visibility.Visible;
            QueryCountText.Text = $"{validLines.Count} quer{(validLines.Count != 1 ? "ies" : "y")}";
            OutputCharCount.Text = $"{result.Length:N0} chars";

            // Warn about skipped lines
            if (invalidLines.Count > 0)
            {
                ShowError($"{invalidLines.Count} invalid line{(invalidLines.Count != 1 ? "s" : "")} skipped (line{(invalidLines.Count != 1 ? "s" : "")} {string.Join(", ", invalidLines.Select(x => x.idx))}).");
            }
        }

        // ───────────────────── Copy ─────────────────────

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            var text = OutputTextBox.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                ShowError("Nothing to copy. Process your input first.");
                return;
            }

            try
            {
                Clipboard.SetText(text);
                ShowToast("Copied to clipboard!");
            }
            catch (Exception)
            {
                ShowError("Failed to copy to clipboard. Please try again.");
            }
        }

        // ───────────────────── Clear ─────────────────────

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            InputTextBox.Text = "";
            OutputTextBox.Text = "";
            ErrorBorder.Visibility = Visibility.Collapsed;
            QueryCountBadge.Visibility = Visibility.Collapsed;
            OutputCharCount.Text = "";
            InputTextBox.Focus();
        }

        // ───────────────────── Dark Mode ─────────────────────

        private void DarkModeToggle_Changed(object sender, RoutedEventArgs e)
        {
            _isDarkMode = DarkModeToggle.IsChecked == true;
            App.SwitchTheme(_isDarkMode);
        }

        // ───────────────────── Error Display ─────────────────────

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorBorder.Visibility = Visibility.Visible;
        }

        // ───────────────────── Toast Notification ─────────────────────

        private void ShowToast(string message)
        {
            ToastText.Text = message;

            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            var slideIn = new DoubleAnimation(20, 0, TimeSpan.FromMilliseconds(250))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300))
            {
                BeginTime = TimeSpan.FromSeconds(1.8),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };
            var slideOut = new DoubleAnimation(0, 20, TimeSpan.FromMilliseconds(300))
            {
                BeginTime = TimeSpan.FromSeconds(1.8),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };

            ToastBorder.BeginAnimation(OpacityProperty, fadeIn);
            ToastTranslate.BeginAnimation(System.Windows.Media.TranslateTransform.YProperty, slideIn);

            ToastBorder.BeginAnimation(OpacityProperty, fadeOut);
            ToastTranslate.BeginAnimation(System.Windows.Media.TranslateTransform.YProperty, slideOut);

            // Chain: fade in then fade out
            var storyboard = new Storyboard();

            var opIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(opIn, ToastBorder);
            Storyboard.SetTargetProperty(opIn, new PropertyPath(OpacityProperty));

            var trIn = new DoubleAnimation(20, 0, TimeSpan.FromMilliseconds(250))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(trIn, ToastBorder);
            Storyboard.SetTargetProperty(trIn, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));

            var opOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300))
            {
                BeginTime = TimeSpan.FromSeconds(1.8),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };
            Storyboard.SetTarget(opOut, ToastBorder);
            Storyboard.SetTargetProperty(opOut, new PropertyPath(OpacityProperty));

            var trOut = new DoubleAnimation(0, 20, TimeSpan.FromMilliseconds(300))
            {
                BeginTime = TimeSpan.FromSeconds(1.8),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };
            Storyboard.SetTarget(trOut, ToastBorder);
            Storyboard.SetTargetProperty(trOut, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));

            storyboard.Children.Add(opIn);
            storyboard.Children.Add(trIn);
            storyboard.Children.Add(opOut);
            storyboard.Children.Add(trOut);

            storyboard.Begin();
        }
    }
}
