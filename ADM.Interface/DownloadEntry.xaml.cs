using System.Windows.Controls;
using System.Windows.Input;
using ADM.Core;

namespace ADM.Interface
{
    public partial class DownloadEntry : UserControl
    {
        //TODO: Remove code from this class
        private DownloadEntryViewModel viewModel => DataContext as DownloadEntryViewModel;
        
        public DownloadEntry(FileProperties file)
        {
            DataContext = new DownloadEntryViewModel(file);
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
