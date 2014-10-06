using IndexerLib.Readers.Pdf;
using IndexerWpf.Views;
using Microsoft.Win32;
using System;
using System.Windows.Input;

namespace IndexerWpf.ViewModels
{
    public class StartViewModel : BaseViewModel
    {
        string _file;
        public string File {
            get
            {
                return _file;
            }
            set
            {
                if (_file == value)
                    return;

                _file = value;
                RaisePropertyChanged("File");
                RaisePropertyChanged("FileNotice");
            }
        }

        public string ReleaseInfo
        {
            get
            {
                return
                    String.Format("build:{0}", App.RetrieveLinkerTimestampVersion().ToString());
            }
        }

        public string FileNotice
        {
            get
            {
                return String.IsNullOrEmpty(File) ? @"Нажмите кнопку обзор чтобы выбрать файл" : File;
            }
        }

        public ICommand SelectFile { get; set; }

        public ICommand Configure { get; set; }

        public StartViewModel()
        {
            SelectFile = new LambdaCommand()
            {
                ExecuteAction = () => SelectFileImpl()
            };

            Configure = new LambdaCommand()
            {
                ExecuteAction = () => MainWindow.Current.CurrentPage = new ConfigurePage()
            };
        }

        void SelectFileImpl()
        {
            var dialog = new OpenFileDialog
            {
                // Filter = "PDF Files (.pdf)|*.pdf|Text Files (.txt)|*.txt|All Files (*.*)|*.*",
                Filter = "PDF Files (.pdf)|*.pdf",
                FilterIndex = 1,
                Multiselect = false
            };

            var userClickedOk = dialog.ShowDialog();

            if (userClickedOk == true)
            {
                File = dialog.FileName;
                Behavior.ProcessReading(new PdfTextSource(File));
            }
        }
    }
}
