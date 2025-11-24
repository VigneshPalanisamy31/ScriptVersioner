using System.Text;
using System.Windows;
using System.Windows.Controls;
using ImageManagementApp.ViewModels;

namespace ImageManagementApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();
            _viewModel= new MainViewModel();
            this.DataContext = _viewModel;
        }
        private void AddStepBtn_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddStep();
            UpdateStatus("Step added");
            RefreshGitStatus();
        }

        private void DeleteStepBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedStep != null)
            {
                _viewModel.DeleteStep(_viewModel.SelectedStep);
                UpdateStatus("Step deleted (moved to pending deletion)");
                RefreshGitStatus();
            }
        }

        private void UndoBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.Undo())
            {
                UpdateStatus("Undo performed");
                RefreshGitStatus();
            }
        }

        private void RedoBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.Redo())
            {
                UpdateStatus("Redo performed");
                RefreshGitStatus();
            }
        }

        private void SaveScriptBtn_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveScript();
            UpdateStatus("Script saved successfully");
            RefreshGitStatus();
        }

        private void CloseAppBtn_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.CloseScript();
            UpdateStatus("Script closed - pending deletions cleaned up");
            RefreshGitStatus();
            
        }

        private void RefreshGitStatus()
        {
            GitStatusBox.ItemsSource = _viewModel.GetGitStatus();
        }

        private void UpdateStatus(string message)
        {
            StatusText.Text = message;
        }
    }
}