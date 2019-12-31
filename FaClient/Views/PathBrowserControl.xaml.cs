using FaClient.Annotations;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace FaClient.Views
{
    public partial class PathBrowserControl : UserControl, INotifyPropertyChanged
    {
        static PathBrowserControl()
        {
            CaptionProperty = DependencyProperty.Register("Caption",
                typeof(string), typeof(PathBrowserControl),
                new PropertyMetadata(string.Empty, OnCaptionPropertyChanged));
            PathNameProperty = DependencyProperty.Register("PathName", typeof(string),
                typeof(PathBrowserControl),
                new PropertyMetadata(string.Empty, OnPathNamePropertyChanged));
        }

        public PathBrowserControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty CaptionProperty;

        public static DependencyProperty PathNameProperty;

        public string PathName
        {
            get => (string)GetValue(PathNameProperty);
            set => SetValue(PathNameProperty, value);
        }

        public string Caption
        {
            get => (string)GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static void OnCaptionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PathBrowserControl pathBrowserControl)
            {
                pathBrowserControl.OnPropertyChanged(nameof(Caption));
                pathBrowserControl.OnCaptionPropertyChanged(e);
            }
        }

        private void OnCaptionPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            CaptionTextBlock.Text = Caption;
        }

        private static void OnPathNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PathBrowserControl pathBrowserControl)
            {
                pathBrowserControl.OnPropertyChanged(nameof(PathName));
                pathBrowserControl.OnPathNamePropertyChanged(e);
            }
        }

        private void OnPathNamePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            PathTextBox.Text = PathName;
        }

        private void PathTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            e.Handled = true;
            if (sender is TextBox tb)
            {
                PathName = tb.Text;
                OnPropertyChanged(nameof(PathName));
            }
        }

        private void BrowseButton_OnClick(object sender, RoutedEventArgs e)
        {
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            var dialog = new CommonOpenFileDialog { IsFolderPicker = true, InitialDirectory = PathName };
            if (dialog.ShowDialog(window) == CommonFileDialogResult.Ok)
            {
                PathName = dialog.FileName;
            }
        }
    }
}
