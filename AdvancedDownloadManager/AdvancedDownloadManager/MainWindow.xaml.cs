using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AdvancedDownloadManager
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnPauseButton(object sender, MouseButtonEventArgs e)
        {
            if (sender is Viewbox box)
            {
                if (box.Child is Path path)
                {
                    var pause = FindResource("cancelButton");
                    path.Data = (Geometry) pause;
                }
            }
        }
    }
}