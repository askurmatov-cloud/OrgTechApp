using System;
using System.Windows;

namespace OrgTechApp
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            var login = txtLogin.Text.Trim();
            var email = txtEmail.Text.Trim();
            var password = txtPassword.Password;
            var confirm = txtConfirm.Password;

            if (login.Length < 4)
            {
                txtStatus.Text = "Логин должен содержать не менее 4 символов.";
                return;
            }

            if (password.Length < 6)
            {
                txtStatus.Text = "Пароль должен содержать не менее 6 символов.";
                return;
            }

            if (password != confirm)
            {
                txtStatus.Text = "Пароли не совпадают.";
                return;
            }

            try
            {
                var exists = Convert.ToInt32(Db.Scalar("SELECT COUNT(*) FROM dbo._05_02_Polzovatel WHERE Login = @login", Db.Param("@login", login)));

                if (exists > 0)
                {
                    txtStatus.Text = "Пользователь с таким логином уже существует.";
                    return;
                }

                var salt = PasswordHelper.NewSalt();
                var hash = PasswordHelper.Hash(salt, password);

                Db.Execute("INSERT INTO dbo._05_02_Polzovatel (Login, HeshParolya, Soly, KodRoli, ElektronnayaPochta, Aktivno) VALUES (@login, @hash, @salt, N'USER', @email, 1)", Db.Param("@login", login), Db.Param("@hash", hash), Db.Param("@salt", salt), Db.Param("@email", string.IsNullOrWhiteSpace(email) ? (object)DBNull.Value : email));

                MessageBox.Show("Пользователь зарегистрирован. Войдите под новым логином.",
                    "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                txtStatus.Text = "Ошибка: " + ex.Message;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
