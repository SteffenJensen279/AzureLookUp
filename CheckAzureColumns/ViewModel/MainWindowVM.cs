using Microsoft.SqlServer.Management.Common;
using System.Linq;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using Smo = Microsoft.SqlServer.Management.Smo;
using System.Data;
using System.Collections.Generic;

namespace CheckAzureColumns.ViewModel
{
    public class MainWindowVM : ViewModelBase
    {
        private Smo.Server _theServer;
        ServerConnection _serverConnection;

        public MainWindowVM()
        {
            SelectedTables = new ObservableCollection<Smo.Table>();
            Server = Properties.Settings.Default.ServerName;
            //Server = "sje-sql-server.database.windows.net";
            UserName = Properties.Settings.Default.UserName;
            //UserName = "sje";
            Password = Properties.Settings.Default.PassWord;
        }
        public RelayCmd LogOnCmd
        {
            get { return new RelayCmd(param => _LogOn(), param => _CanLogOn()); }
        }
        private void _LogOn()
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "sje-sql-server.database.windows.net";
                builder.UserID = UserName;
                builder.Password = Password;
                _GetDatabasesFromServer(builder.ConnectionString);
            }
            catch (Exception excp)
            {
                MessageBox.Show(excp.ToString(), "An Error Happened in Logon");
            }
        }

        private void _GetDatabasesFromServer(string connString)
        {
            SqlConnection conn = new SqlConnection(connString);
            _serverConnection = new ServerConnection(conn);
            _theServer = new Smo.Server(_serverConnection);
            Databases = new ObservableCollection<Smo.Database>();
            foreach (Smo.Database database in _theServer.Databases)
            {
                Databases.Add(database);
            }
        }

        private bool _CanLogOn()
        {
            return true;
        }


        private string _server;
        public string Server
        {
            get { return _server; }
            set { SetAndInvoke(ref _server, value); }
        }
        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { SetAndInvoke(ref _userName, value); }
        }
        private string _password;
        public string Password
        {
            get { return _password; }
            set { SetAndInvoke(ref _password, value); }
        }


        private ObservableCollection<Smo.Database> _databases;
        public ObservableCollection<Smo.Database> Databases
        {
            get { return _databases; }
            set { SetAndInvoke(ref _databases, value); }
        }

        private Smo.Database _selectedDatabase;
        public Smo.Database SelectedDatabase
        {
            get { return _selectedDatabase; }
            set { SetAndInvoke(ref _selectedDatabase, value); _GetTables(); _GetSql(); }
        }

        private void _GetTables()
        {
            Tables = new ObservableCollection<Smo.Table>();
            foreach (Smo.Table table in SelectedDatabase.Tables)
            {
                Tables.Add(table);
            }
        }

        private ObservableCollection<Smo.Table> _tables;
        public ObservableCollection<Smo.Table> Tables
        {
            get { return _tables; }
            set { SetAndInvoke(ref _tables, value); }
        }
        private Smo.Table _selectedTable;
        public Smo.Table SelectedTable
        {
            get { return _selectedTable; }
            set { SetAndInvoke(ref _selectedTable, value); }
        }


        private ObservableCollection<Smo.Table> _selectedTables;
        public ObservableCollection<Smo.Table> SelectedTables
        {
            get { return _selectedTables; }
            set { SetAndInvoke(ref _selectedTables, value); }
        }
        
        private string _nonEmptyCols;
        public string NonEmptyCols
        {
            get { return _nonEmptyCols; }
            set { SetAndInvoke(ref _nonEmptyCols, value); }
        }

        private bool _autoBuildSql;
        public bool AutoBuildSql
        {
            get { return _autoBuildSql; }
            set { SetAndInvoke(ref _autoBuildSql, value); }
        }


        private void _GetSql()
        {
            try
            {
                if (!AutoBuildSql) return;
                NonEmptyCols = string.Empty;
                foreach (Smo.Table table in _selectedDatabase.Tables)
                {
                    _GetSqlForTable(table);
                }
            }
            catch (Exception excp)
            {
                MessageBox.Show(excp.ToString(), "Error happened");
            }
        }
        private void _GetSqlForTable(Smo.Table table)
        {
            try
            {
                NonEmptyCols = $"{NonEmptyCols}{Environment.NewLine}SELECT";
                foreach (Smo.Column col in table.Columns)
                {
                    TheData = SelectedDatabase.ExecuteWithResults($"exec SelectColContent '{table.Name}', '{col.Name}'").Tables[0].DefaultView;
                    int rowCount = (int)TheData[0][0];
                    if (rowCount > 0)
                        NonEmptyCols = $"{NonEmptyCols}{Environment.NewLine}{col.Name},";
                }
                NonEmptyCols = NonEmptyCols.Trim(',');
                NonEmptyCols = $"{NonEmptyCols}{Environment.NewLine}FROM {table.Name}{Environment.NewLine}";  
            }
            catch (Exception excp)
            {
                MessageBox.Show(excp.ToString(), "Error happened");
            }
        }

        private DataView _theData;
        public DataView TheData
        {
            get { return _theData; }
            set { SetAndInvoke(ref _theData, value); }
        }
        public RelayCmd CopySQLCmd
        {
            get { return new RelayCmd(param => _CopySQL(), param => _CanCopySQL()); }
        }
        private void _CopySQL()
        {
            Clipboard.SetText(NonEmptyCols);
        }
        private bool _CanCopySQL()
        {
            return !string.IsNullOrEmpty(NonEmptyCols);
        }
        public RelayCmd BuildSqlCmd
        {
            get { return new RelayCmd(param => _BuildSql(), param => _CanBuildSql()); }
        }
        private void _BuildSql()
        {
            NonEmptyCols = string.Empty;
            foreach (Smo.Table table in SelectedTables)
            {
                _GetSqlForTable(table);
            }
        }
        private bool _CanBuildSql()
        {
            return !AutoBuildSql;
        }

    }
}
