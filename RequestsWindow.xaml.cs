using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace OrgTechApp
{
    public partial class RequestsWindow : Window
    {
        public RequestsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            pnlCreate.Visibility = Session.CanCreateRequest ? Visibility.Visible : Visibility.Collapsed;
            pnlProcess.Visibility = Session.CanProcessRequests ? Visibility.Visible : Visibility.Collapsed;
            lblTitle.Text = Session.CanCreateRequest ? "Мои заявки" : "Заявки на обслуживание";
            lblSubtitle.Text = Session.CanCreateRequest
                ? "Создайте заявку на ремонт или обслуживание и следите за её статусом"
                : "Обработка заявок пользователей";

            cmbRequestType.ItemsSource = new List<StatusItem>
            {
                new StatusItem { Code = "Ремонт", Name = "Ремонт" },
                new StatusItem { Code = "Обслуживание", Name = "Обслуживание" },
                new StatusItem { Code = "Замена", Name = "Замена" },
                new StatusItem { Code = "Консультация", Name = "Консультация" },
                new StatusItem { Code = "Другое", Name = "Другое" }
            };
            cmbRequestType.SelectedIndex = 0;

            cmbFilterStatus.ItemsSource = new List<StatusItem>
            {
                new StatusItem { Code = "", Name = "Все статусы" },
                new StatusItem { Code = "Новая", Name = "Новая" },
                new StatusItem { Code = "В работе", Name = "В работе" },
                new StatusItem { Code = "Выполнена", Name = "Выполнена" },
                new StatusItem { Code = "Отклонена", Name = "Отклонена" }
            };
            cmbFilterStatus.SelectedIndex = 0;

            LoadEquipment();
            LoadRequests();
        }

        private void LoadEquipment()
        {
            var list = new List<EquipmentLookup>();
            var table = Db.Query("SELECT KodOrgtehniki, InventarnyyNomer, Naimenovanie, Model FROM dbo._05_02_Orgtehnika WHERE Status <> N'WRITTEN_OFF' ORDER BY Naimenovanie, Model");

            foreach (DataRow row in table.Rows)
            {
                list.Add(new EquipmentLookup
                {
                    Id = Convert.ToInt32(row["KodOrgtehniki"]),
                    Display = row["InventarnyyNomer"] + " — " + row["Naimenovanie"] + " (" + row["Model"] + ")"
                });
            }
            cmbRequestEquipment.ItemsSource = list;
        }

        private void LoadRequests()
        {
            try
            {
                var status = cmbFilterStatus.SelectedValue as string;
                var onlyUser = Session.CanViewAllRequests ? 0 : Session.UserId;

                var table = Db.Query("SELECT KodZayavki, KodOrgtehniki, InventarnyyNomer, Naimenovanie, Model, TipZayavki, TipNaimenovanie, Opisanie, Status, StatusNaimenovanie, DataSozdaniya, DataObrabotki, AvtorLogin, IspolnitelLogin, KommentariyAdministratora FROM dbo._05_02_vw_Zayavki WHERE (@onlyUser = 0 OR KodPolzovatelya = @onlyUser) AND (@status IS NULL OR Status = @status) ORDER BY DataSozdaniya DESC, KodZayavki DESC", Db.Param("@onlyUser", onlyUser), Db.Param("@status", string.IsNullOrWhiteSpace(status) ? (object)DBNull.Value : status));

                dgRequests.ItemsSource = table.DefaultView;
                if (table.Rows.Count == 0)
                {
                    lblSelectedRequest.Text = "Заявки не найдены";
                    txtAdminComment.Text = string.Empty;
                }
                else
                {
                    dgRequests.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки заявок: " + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private DataRowView SelectedRequest
        {
            get { return dgRequests.SelectedItem as DataRowView; }
        }

        private void dgRequests_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var row = SelectedRequest;
            if (row == null)
            {
                lblSelectedRequest.Text = "Выберите заявку";
                txtAdminComment.Text = string.Empty;
                return;
            }

            lblSelectedRequest.Text = row["InventarnyyNomer"] + " — " + row["Naimenovanie"] + "\n" +
                                      row["TipNaimenovanie"] + ": " + row["Opisanie"] + "\n" +
                                      "Автор: " + row["AvtorLogin"] + " • " + row["StatusNaimenovanie"];
            txtAdminComment.Text = row["KommentariyAdministratora"] == DBNull.Value
                ? string.Empty
                : row["KommentariyAdministratora"].ToString();
        }

        private void cmbFilterStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
                LoadRequests();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadRequests();
        }

        private void btnCreateRequest_Click(object sender, RoutedEventArgs e)
        {
            if (!Session.CanCreateRequest)
                return;

            if (cmbRequestEquipment.SelectedValue == null)
            {
                MessageBox.Show("Выберите оборудование.");
                return;
            }

            var type = cmbRequestType.SelectedValue as string;
            if (string.IsNullOrWhiteSpace(type))
            {
                MessageBox.Show("Выберите тип заявки.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtRequestDescription.Text))
            {
                MessageBox.Show("Опишите проблему.");
                return;
            }

            try
            {
                Db.Execute("INSERT INTO dbo._05_02_Zayavka (KodOrgtehniki, KodPolzovatelya, TipZayavki, Opisanie, Status) VALUES (@equipment, @user, @type, @description, N'Новая')", Db.Param("@equipment", cmbRequestEquipment.SelectedValue), Db.Param("@user", Session.UserId), Db.Param("@type", type), Db.Param("@description", txtRequestDescription.Text.Trim()));

                MessageBox.Show("Заявка отправлена.", "Готово",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                txtRequestDescription.Clear();
                LoadRequests();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка создания заявки: " + ex.Message);
            }
        }

        private void btnTake_Click(object sender, RoutedEventArgs e)
        {
            ChangeStatus("В работе");
        }

        private void btnComplete_Click(object sender, RoutedEventArgs e)
        {
            ChangeStatus("Выполнена");
        }

        private void btnReject_Click(object sender, RoutedEventArgs e)
        {
            ChangeStatus("Отклонена");
        }

        private void ChangeStatus(string status)
        {
            if (!Session.CanProcessRequests)
                return;

            var row = SelectedRequest;
            if (row == null)
            {
                MessageBox.Show("Выберите заявку.");
                return;
            }

            try
            {
                Db.Execute("UPDATE dbo._05_02_Zayavka SET Status = @status, KodIspolnitelya = @executor, DataObrabotki = CASE WHEN @status IN (N'Выполнена', N'Отклонена') THEN SYSDATETIME() ELSE DataObrabotki END, KommentariyAdministratora = @comment WHERE KodZayavki = @id", Db.Param("@status", status), Db.Param("@executor", Session.UserId), Db.Param("@comment", string.IsNullOrWhiteSpace(txtAdminComment.Text) ? (object)DBNull.Value : txtAdminComment.Text.Trim()), Db.Param("@id", row["KodZayavki"]));

                LoadRequests();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка изменения статуса: " + ex.Message);
            }
        }
    }
}
