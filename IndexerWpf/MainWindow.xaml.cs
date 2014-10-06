using IndexerLib.Persist;
using IndexerWpf.Views;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;

namespace IndexerWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ModalInvoker _currentModal;
        Page _currentPage;

        public Page CurrentPage 
        {
            get
            {
                return _currentPage;
            }
            set
            {
                if (_currentPage == value)
                    return;
                
                _currentPage = value;
                Navigate(_currentPage);
            }
        }

        public static MainWindow Current { get; private set; }

        public void Navigate(Page page)
        {
            Frame.Navigate(page);
            ModalFrame.Visibility = System.Windows.Visibility.Collapsed;
        }

        public ModalInvoker ShowModal(Page page)
        {
            Frame.IsEnabled = false;
            ModalFrame.Navigate(page);
            ModalFrame.Visibility = System.Windows.Visibility.Visible;
            Frame.Effect = new BlurEffect()
            {
                Radius = 3,
                RenderingBias = RenderingBias.Quality,
                KernelType = KernelType.Gaussian
            };

            _currentModal = new ModalInvoker();

            return _currentModal;
        }

        public void CloseModal()
        {
            Frame.Effect = null;
            ModalFrame.Visibility = System.Windows.Visibility.Collapsed;
            ModalFrame.Navigate(null);
            Frame.IsEnabled = true;

            if (_currentModal != null)
            {
                _currentModal.OnModalClose();
                _currentModal = null;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            Context.Cached.WarmingBegins += Cached_WarmingBegins;
            Context.Cached.WarmingEnds += Cached_WarmingEnds;

            CurrentPage = new StartPage();
            //Navigate(new SearchResultPage());
            Current = this;
            AfterWindowShow();
        }

        void Cached_WarmingEnds(object sender, EventArgs e)
        {
            var task = new Task(() => Dispatcher.Invoke(() => ProgressBar.Visibility = Visibility.Collapsed));
            task.Start();
        }

        void Cached_WarmingBegins(object sender, EventArgs e)
        {
            var task = new Task(() => Dispatcher.Invoke(() => ProgressBar.Visibility = Visibility.Visible));
            task.Start();
        }

        public void ShowBackgroundAction(BackgroundGuiAction action)
        {
            this.ProgressBar.Visibility = Visibility.Visible;

            action.ContinueWith.Add(() =>
            {
                this.ProgressBar.Visibility = Visibility.Collapsed;
            });
        }

        void AfterWindowShow()
        {
            var warm = new Task(() => Context.Cached.Warm());
            warm.Start();
        /*
            var initBackgroundAction = new BackgroundGuiAction()
            {
                Action = Context.Cached.Warm
            };

            ShowBackgroundAction(initBackgroundAction);
            initBackgroundAction.Start(); */
        }
    }

    public class ModalInvoker
    {
        public Action ModalClosed { get; set; }

        public void OnModalClose()
        {
            var act = ModalClosed;
            if (act != null)
                act();
        }
    }
}
