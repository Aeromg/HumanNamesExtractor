using IndexerWpf.ViewModels;
using System.Windows.Controls;

namespace IndexerWpf.Views
{
    /// <summary>
    /// Interaction logic for KnownPersonEditorPage.xaml
    /// </summary>
    public partial class KnownPersonEditorPage : Page
    {
        public KnownPersonEditorPage()
        {
            InitializeComponent();
        }

        public KnownPersonEditorPage(KnownPersonEditorViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}
