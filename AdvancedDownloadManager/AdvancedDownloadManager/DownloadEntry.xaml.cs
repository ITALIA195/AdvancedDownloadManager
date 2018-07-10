using System.Windows;

namespace AdvancedDownloadManager
{
    public partial class DownloadEntry
    {
        public DownloadEntry()
        {
            InitializeComponent();
        }

        public string FileName
        {
            get => (string) GetValue(FileNameProperty);
            set => SetValue(FileNameProperty, value);
        }

        public static readonly DependencyProperty FileNameProperty = 
            DependencyProperty.Register(nameof(FileName), typeof(string), typeof(DownloadEntry), new PropertyMetadata("File name"));
    }
}
