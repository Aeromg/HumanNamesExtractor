using System.ComponentModel;

namespace IndexerWpf.ViewModels
{
    [Magic]
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public void RaisePropertyChanged(string propName)
        {
            var e = PropertyChanged;
            if (e != null)
                e(this, new PropertyChangedEventArgs(propName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
