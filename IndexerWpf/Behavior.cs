using System.Threading.Tasks;
using IndexerLib;
using IndexerWpf.ViewModels;
using IndexerWpf.Views;

namespace IndexerWpf
{
    public static class Behavior
    {
        public static void ProcessReading(ITextSource source)
        {
            MainWindow.Current.Navigate(new ProgressPage());

            var task = new Task<IBook>(() => Essentials.ReadBook(source));
            task.ContinueWith((result) => MainWindow.Current.Dispatcher.Invoke(() => ShowResults(result.Result)));
            task.Start();
        }

        public static void ShowResults(IBook book)
        {
            MainWindow.Current.Navigate(new SearchResultPage(new SearchResultViewModel(book)));
        }

        public static void GoMain()
        {
            MainWindow.Current.Navigate(new StartPage());
        }

        public static void GoConfig()
        {
            MainWindow.Current.ShowModal(new ConfigurePage());
        }
    }
}
