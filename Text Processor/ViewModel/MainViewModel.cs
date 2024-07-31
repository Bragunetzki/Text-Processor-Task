using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Text_Processor.Model;

namespace Text_Processor.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private TextProcessor textProcessor;
        private string outputFile;
        private int minWordLength;
        private bool removePunctuation;
        private string status;

        public ObservableCollection<string> InputFiles { get; set; }
        public string OutputFile
        {
            get => outputFile;
            set
            {
                outputFile = value;
                OnPropertyChanged(nameof(OutputFile));
            }
        }
        public int MinWordLength
        {
            get => minWordLength;
            set
            {
                minWordLength = value;
                OnPropertyChanged(nameof(MinWordLength));
            }
        }
        public bool RemovePunctuation
        {
            get => removePunctuation;
            set
            {
                removePunctuation = value;
                OnPropertyChanged(nameof(RemovePunctuation));
            }
        }
        public string Status
        {
            get => status;
            set
            {
                status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public ICommand SelectInputFilesCommand { get; }
        public ICommand SelectOutputFileCommand { get; }
        public ICommand ProcessFilesCommand { get; }
        public MainViewModel()
        {
            textProcessor = new TextProcessor();
            InputFiles = new ObservableCollection<string>();
            SelectInputFilesCommand = new RelayCommand(SelectInputFiles);
            SelectOutputFileCommand = new RelayCommand(SelectOutputFile);
            ProcessFilesCommand = new RelayCommand(ProcessFiles);
        }

        private void SelectInputFiles()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                InputFiles.Clear();
                foreach (var file in openFileDialog.FileNames)
                {
                    InputFiles.Add(file);
                }
            }
        }

        private void SelectOutputFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                OutputFile = saveFileDialog.FileName;
            }
        }

        private async void ProcessFiles()
        {
            if (InputFiles.Count == 0)
            {
                MessageBox.Show("Please select input files", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrEmpty(OutputFile))
            {
                MessageBox.Show("Please select an output file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (InputFiles.Contains(OutputFile))
            {
                MessageBox.Show("Input files cannot contain output file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Status = "Processing...";
            try
            {
                await Task.Run(() =>
                {
                    textProcessor.ProcessFiles(InputFiles, outputFile, MinWordLength, RemovePunctuation);
                });

                Status = "Processing complete.";
                MessageBox.Show("Processing complete", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Status = "Error occurred during processing.";
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
            : this(o => execute(), o => canExecute == null || canExecute())
        {
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
