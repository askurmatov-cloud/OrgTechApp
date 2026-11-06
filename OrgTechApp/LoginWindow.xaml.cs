using System;
using System.Data;
using System.Windows;
using Microsoft.Data.SqlClient;

namespace OrgTechApp
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var login = txtLogin.Text.Trim();
            var password = txtPassword.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrEmpty(password))
            {
                txtStatus.Text = "Введите логин и пароль.";
                return;
            }

            try
            {
                var table = Db.Query("SELECT p.KodPolzovatelya, p.Login, p.HeshParolya, p.Soly, p.KodRoli, r.Naimenovanie AS Rol FROM dbo._05_02_Polzovatel AS p JOIN dbo._05_02_Rol AS r ON r.KodRoli = p.KodRoli WHERE p.Login = @login AND p.Aktivno = 1 AND r.Aktivno = 1", Db.Param("@login", login));

                if (table.Rows.Count == 0)
                {
                    txtStatus.Text = "Неверный логин или пароль.";
                    return;
                }

                var row = table.Rows[0];
                var salt = row["Soly"].ToString();
                var expectedHash = row["HeshParolya"].ToString();
                var actualHash = PasswordHelper.Hash(salt, password);

                if (!string.Equals(expectedHash, actualHash, StringComparison.OrdinalIgnoreCase))
                {
                    txtStatus.Text = "Неверный логин или пароль.";
                    return;
                }

                Session.UserId = Convert.ToInt32(row["KodPolzovatelya"]);
                Session.Login = row["Login"].ToString();
                Session.Role = row["KodRoli"].ToString();
                Session.RoleName = row["Rol"].ToString();

                Db.Execute("UPDATE dbo._05_02_Polzovatel SET DataPoslednegoVhoda = SYSDATETIME() WHERE KodPolzovatelya = @id", Db.Param("@id", Session.UserId));

                var main = new MainWindow();
                main.Show();
                Close();
            }
            catch (Exception ex)
            {
                txtStatus.Text = "Ошибка подключения: " + ex.Message;
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            var window = new RegisterWindow { Owner = this };
            window.ShowDialog();
        }
    }
}
