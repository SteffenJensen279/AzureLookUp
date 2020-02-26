using CheckAzureColumns.ViewModel;
using System.Windows;
using System.Windows.Controls;
using Smo = Microsoft.SqlServer.Management.Smo;

namespace CheckAzureColumns
{
    public partial class MainWindow : Window
    {
        private MainWindowVM _VM;
        public MainWindow()
        {
            _VM = new MainWindowVM();
            InitializeComponent();
            DataContext = _VM;
            pwdPassword.Password = "Kodeord1";
        }

        private void pwdPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _VM.Password = (sender as PasswordBox).Password;
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            Properties.Settings.Default.ServerName = _VM.Server;
            Properties.Settings.Default.UserName = _VM.UserName;
            Properties.Settings.Default.PassWord = _VM.Password;
            Properties.Settings.Default.Save();
        }

        private void btnBuildSql_Click(object sender, RoutedEventArgs e)
        {
            _VM.SelectedTables.Clear();
            foreach (Smo.Table table in lboTables.SelectedItems)
            {
                _VM.SelectedTables.Add(table);
            }
        }
    }
}
