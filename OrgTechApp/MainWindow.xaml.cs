using System;
using System.Data;
using System.Windows;

namespace OrgTechApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            lblUser.Text = Session.Login;
            lblRole.Text = Session.RoleName + " (" + Session.Role + ")";
            lblWelcome.Text = "Вы вошли как " + Session.Login + ". Доступные разделы зависят от роли.";

            btnOperations.Visibility = Session.CanOperate ? Visibility.Visible : Visibility.Collapsed;
            btnReports.Visibility = Session.CanViewReports ? Visibility.Visible : Visibility.Collapsed;
            btnUsers.Visibility = Session.IsAdmin ? Visibility.Visible : Visibility.Collapsed;
            btnRequests.Content = Session.Role == "USER" ? "Мои заявки" : "Заявки";

            LoadDashboard();
        }

        private void LoadDashboard()
        {
            try
            {
                var stats = Db.Query("SELECT COUNT(*) AS Total, SUM(CASE WHEN Status = N'IN_USE' THEN 1 ELSE 0 END) AS InUse, SUM(CASE WHEN Status = N'IN_REPAIR' THEN 1 ELSE 0 END) AS Repair, SUM(CASE WHEN Status = N'WRITTEN_OFF' THEN 1 ELSE 0 END) AS WrittenOff FROM dbo._05_02_Orgtehnika");

                if (stats.Rows.Count > 0)
                {
                    var row = stats.Rows[0];
                    lblTotal.Text = row["Total"].ToString();
                    lblInUse.Text = row["InUse"] == DBNull.Value ? "0" : row["InUse"].ToString();
                    lblRepair.Text = row["Repair"] == DBNull.Value ? "0" : row["Repair"].ToString();
                    lblWrittenOff.Text = row["WrittenOff"] == DBNull.Value ? "0" : row["WrittenOff"].ToString();
                }

                dgRecent.ItemsSource = Db.Query("SELECT TOP (8) DataOperatsii, Operatsiya, InventarnyyNomer, Naimenovanie, Ispolnitel FROM dbo._05_02_vw_IstoriyaPeremeshcheniy ORDER BY DataOperatsii DESC, KodPeremeshcheniya DESC").DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось загрузить данные: " + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnEquipment_Click(object sender, RoutedEventArgs e)
        {
            new EquipmentWindow { Owner = this }.ShowDialog();
            LoadDashboard();
        }

        private void btnOperations_Click(object sender, RoutedEventArgs e)
        {
            new OperationWindow { Owner = this }.ShowDialog();
            LoadDashboard();
        }

        private void btnRequests_Click(object sender, RoutedEventArgs e)
        {
            new RequestsWindow { Owner = this }.ShowDialog();
            LoadDashboard();
        }

        private void btnReports_Click(object sender, RoutedEventArgs e)
        {
            new ReportsWindow { Owner = this }.ShowDialog();
        }

        private void btnUsers_Click(object sender, RoutedEventArgs e)
        {
            new UsersWindow { Owner = this }.ShowDialog();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            Session.Clear();
            new LoginWindow().Show();
            Close();
        }
    }
}
