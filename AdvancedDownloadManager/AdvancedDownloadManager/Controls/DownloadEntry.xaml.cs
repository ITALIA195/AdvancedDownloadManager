using System.Windows.Input;

namespace AdvancedDownloadManager.Controls
{
    public partial class DownloadEntry
    {
        //TODO: Remove code from this class
        private readonly DownloadEntryViewModel viewModel;
        
        public DownloadEntry(FileProperties file)
        {
            InitializeComponent();
            
            DataContext = viewModel = new DownloadEntryViewModel(file);
        }

        private void OnPauseButton(object sender, MouseButtonEventArgs e)
        {
            viewModel.Downloader.Pause();
        }

        private void OnCancelButton(object sender, MouseButtonEventArgs e)
        {
            viewModel.Downloader.Cancel();
        }

        private void OnResumeButton(object sender, MouseButtonEventArgs e)
        {
            viewModel.Downloader.Resume();
        }

        private void OnFileNameClick(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void OnDownloadUrlClick(object sender, MouseButtonEventArgs e)
        {
            
        }
    }
}
