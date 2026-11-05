using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace OrgTechApp
{
    public partial class UsersWindow : Window
    {
        public UsersWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadRoles();
            LoadUsers();
        }

        private void LoadRoles()
        {
            cmbRole.ItemsSource = Db.Query("SELECT KodRoli, Naimenovanie FROM dbo._05_02_Rol WHERE Aktivno = 1 ORDER BY Naimenovanie").DefaultView;
        }

        private void LoadUsers()
        {
            try
            {
                dgUsers.ItemsSource = Db.Query("SELECT p.KodPolzovatelya, p.Login, p.KodRoli, r.Naimenovanie AS Rol, p.ElektronnayaPochta, p.Aktivno, CASE WHEN p.Aktivno = 1 THEN N'Да' ELSE N'Нет' END AS AktivnoText, CONVERT(NVARCHAR(16), p.DataPoslednegoVhoda, 120) AS DataPoslednegoVhoda FROM dbo._05_02_Polzovatel AS p JOIN dbo._05_02_Rol AS r ON r.KodRoli = p.KodRoli ORDER BY p.Login").DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки пользователей: " + ex.Message);
            }
        }

        private DataRowView SelectedUser
        {
            get { return dgUsers.SelectedItem as DataRowView; }
        }

        private void dgUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var row = SelectedUser;
            if (row != null)
                cmbRole.SelectedValue = row["KodRoli"].ToString();
        }

        private void cmbRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void btnApplyRole_Click(object sender, RoutedEventArgs e)
        {
            var row = SelectedUser;
            if (row == null)
            {
                MessageBox.Show("Выберите пользователя.");
                return;
            }

            var role = cmbRole.SelectedValue as string;
            if (string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("Выберите роль.");
                return;
            }

            if (Convert.ToInt32(row["KodPolzovatelya"]) == Session.UserId && role != "ADMIN")
            {
                MessageBox.Show("Нельзя снять роль ADMIN с текущей учётной записи.");
                return;
            }

            try
            {
                Db.Execute("UPDATE dbo._05_02_Polzovatel SET KodRoli = @role, DataIzmeneniya = SYSDATETIME() WHERE KodPolzovatelya = @id", Db.Param("@role", role), Db.Param("@id", row["KodPolzovatelya"]));
                LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка смены роли: " + ex.Message);
            }
        }

        private void btnToggleActive_Click(object sender, RoutedEventArgs e)
        {
            var row = SelectedUser;
            if (row == null)
            {
                MessageBox.Show("Выберите пользователя.");
                return;
            }

            var id = Convert.ToInt32(row["KodPolzovatelya"]);
            if (id == Session.UserId)
            {
                MessageBox.Show("Нельзя деактивировать текущую учётную запись.");
                return;
            }

            var active = Convert.ToBoolean(row["Aktivno"]);
            try
            {
                Db.Execute("UPDATE dbo._05_02_Polzovatel SET Aktivno = @active, DataIzmeneniya = SYSDATETIME() WHERE KodPolzovatelya = @id", Db.Param("@active", !active), Db.Param("@id", id));
                LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка изменения статуса: " + ex.Message);
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
