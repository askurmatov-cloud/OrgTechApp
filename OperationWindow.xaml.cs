using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;

namespace OrgTechApp
{
    public partial class OperationWindow : Window
    {
        private readonly int? _initialEquipmentId;

        public OperationWindow(int? equipmentId = null)
        {
            InitializeComponent();
            _initialEquipmentId = equipmentId;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadEquipment();
            LoadOperationTypes();
            LoadEmployees();
            dpOperation.SelectedDate = DateTime.Today;

            if (_initialEquipmentId.HasValue)
                cmbEquipment.SelectedValue = _initialEquipmentId.Value;

            if (cmbOperation.Items.Count > 0 && cmbOperation.SelectedIndex < 0)
                cmbOperation.SelectedIndex = 0;

            UpdateFieldState();
            LoadHistory();
        }

        private void LoadEquipment()
        {
            var list = new List<EquipmentLookup>();
            var table = Db.Query("SELECT KodOrgtehniki, InventarnyyNomer, Naimenovanie, Model, Status FROM dbo._05_02_Orgtehnika ORDER BY Naimenovanie, Model");

            foreach (DataRow row in table.Rows)
            {
                list.Add(new EquipmentLookup
                {
                    Id = Convert.ToInt32(row["KodOrgtehniki"]),
                    Display = row["InventarnyyNomer"] + " — " + row["Naimenovanie"] + " (" + row["Model"] + ")"
                });
            }
            cmbEquipment.ItemsSource = list;
        }

        private void LoadOperationTypes()
        {
            cmbOperation.ItemsSource = Db.Query("SELECT KodOperatsii, Naimenovanie FROM dbo._05_02_TipOperatsii WHERE Aktivno = 1 ORDER BY KodOperatsii").DefaultView;
        }

        private void LoadEmployees()
        {
            var list = new List<LookupItem>();
            var table = Db.Query("SELECT KodSotrudnika, Familiya, Imya, Otchestvo FROM dbo._05_02_Sotrudnik WHERE Aktivno = 1 ORDER BY Familiya, Imya");

            foreach (DataRow row in table.Rows)
            {
                list.Add(new LookupItem
                {
                    Id = Convert.ToInt32(row["KodSotrudnika"]),
                    Name = (row["Familiya"] + " " + row["Imya"] + " " + row["Otchestvo"]).Trim()
                });
            }
            cmbRecipient.ItemsSource = list;
        }

        private void cmbOperation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateFieldState();
        }

        private void UpdateFieldState()
        {
            var code = cmbOperation.SelectedValue as string;
            cmbRecipient.IsEnabled = code == "TRN";
            txtReason.IsEnabled = code == "WRT" || code == "REP";
            if (code != "TRN")
                cmbRecipient.SelectedIndex = -1;
        }

        private void LoadHistory()
        {
            try
            {
                var id = cmbEquipment.SelectedValue is int value ? value : 0;
                dgHistory.ItemsSource = Db.Query("SELECT DataOperatsii, Operatsiya, NomerDokumenta, Ispolnitel, Poluchatel, Prichina FROM dbo._05_02_vw_IstoriyaPeremeshcheniy WHERE (@id = 0 OR KodOrgtehniki = @id) ORDER BY DataOperatsii DESC, KodPeremeshcheniya DESC", Db.Param("@id", id)).DefaultView;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Ошибка истории: " + ex.Message;
            }
        }

        private void btnRefreshHistory_Click(object sender, RoutedEventArgs e)
        {
            LoadHistory();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (cmbEquipment.SelectedValue == null)
            {
                MessageBox.Show("Выберите оборудование.");
                return;
            }

            var operation = cmbOperation.SelectedValue as string;
            if (string.IsNullOrWhiteSpace(operation))
            {
                MessageBox.Show("Выберите тип операции.");
                return;
            }

            if (!dpOperation.SelectedDate.HasValue)
            {
                MessageBox.Show("Укажите дату операции.");
                return;
            }

            if (operation == "TRN" && cmbRecipient.SelectedValue == null)
            {
                MessageBox.Show("Для передачи выберите получателя.");
                return;
            }

            if (operation == "WRT" && string.IsNullOrWhiteSpace(txtReason.Text))
            {
                MessageBox.Show("Для списания укажите причину.");
                return;
            }

            try
            {
                using (var connection = Db.Open())
                using (var command = new SqlCommand("dbo._05_02_sp_OformitOperatsiyu", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@KodOrgtehniki", SqlDbType.Int).Value = Convert.ToInt32(cmbEquipment.SelectedValue);
                    command.Parameters.Add("@KodOperatsii", SqlDbType.Char, 3).Value = operation;
                    command.Parameters.Add("@KodIspolnitelya", SqlDbType.Int).Value = Session.UserId;
                    command.Parameters.Add("@KodPoluchatelya", SqlDbType.Int).Value =
                        operation == "TRN" ? cmbRecipient.SelectedValue : (object)DBNull.Value;
                    command.Parameters.Add("@DataOperatsii", SqlDbType.Date).Value = dpOperation.SelectedDate.Value.Date;
                    command.Parameters.Add("@NomerDokumenta", SqlDbType.NVarChar, 50).Value =
                        string.IsNullOrWhiteSpace(txtDocument.Text) ? (object)DBNull.Value : txtDocument.Text.Trim();
                    command.Parameters.Add("@Prichina", SqlDbType.NVarChar, 250).Value =
                        string.IsNullOrWhiteSpace(txtReason.Text) ? (object)DBNull.Value : txtReason.Text.Trim();
                    command.Parameters.Add("@Kommentariy", SqlDbType.NVarChar, 500).Value =
                        string.IsNullOrWhiteSpace(txtComment.Text) ? (object)DBNull.Value : txtComment.Text.Trim();

                    var output = command.Parameters.Add("@KodPeremeshcheniya", SqlDbType.Int);
                    output.Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    lblStatus.Text = "Операция оформлена. № " + output.Value;
                    MessageBox.Show("Операция успешно оформлена.", "Готово",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    txtDocument.Clear();
                    txtReason.Clear();
                    txtComment.Clear();
                    LoadHistory();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Операция не выполнена",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
