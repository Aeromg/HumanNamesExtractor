using System.Windows.Input;
using IndexerWpf.Views;

namespace IndexerWpf.ViewModels
{
    public class ConfigureViewModel : BaseViewModel
    {
        public ICommand GoBack { get; private set; }

        public ConfigurePersonsViewModel PersonsViewModel { get; private set; }

        public ConfigureStopWordsViewModel StopWordsViewModel { get; private set; }

        public ConfigureViewModel()
        {
            GoBack = new LambdaCommand()
            {
                ExecuteAction = () => MainWindow.Current.CurrentPage = new StartPage()
            };

            PersonsViewModel = new ConfigurePersonsViewModel();

            StopWordsViewModel = new ConfigureStopWordsViewModel();
        }
    }
}
