using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows;
using Microsoft.Data.SqlClient;

namespace OrgTechApp
{
    public partial class EquipmentEditWindow : Window
    {
        private readonly int? _equipmentId;
        private string _photoPath = PhotoHelper.NoPhoto;

        public EquipmentEditWindow(int? equipmentId)
        {
            InitializeComponent();
            _equipmentId = equipmentId;
            LoadLookups();
            dpReceived.SelectedDate = DateTime.Today;

            if (_equipmentId.HasValue)
            {
                lblTitle.Text = "Редактирование оборудования";
                LoadEquipment();
            }
            else
            {
                lblTitle.Text = "Добавление оборудования";
                SetPhoto(PhotoHelper.NoPhoto);
            }
        }

        private void LoadLookups()
        {
            var suppliers = new List<LookupItem>();
            var supplierTable = Db.Query("SELECT KodPostavshchika, Naimenovanie FROM dbo._05_02_Postavshchik WHERE Aktivno = 1 ORDER BY Naimenovanie");
            foreach (DataRow row in supplierTable.Rows)
            {
                suppliers.Add(new LookupItem
                {
                    Id = Convert.ToInt32(row["KodPostavshchika"]),
                    Name = row["Naimenovanie"].ToString()
                });
            }
            cmbSupplier.ItemsSource = suppliers;

            cmbStatus.ItemsSource = new List<StatusItem>
            {
                new StatusItem { Code = "IN_USE", Name = "В эксплуатации" },
                new StatusItem { Code = "IN_REPAIR", Name = "В ремонте" },
                new StatusItem { Code = "WRITTEN_OFF", Name = "Списана" }
            };
            cmbStatus.SelectedIndex = 0;
        }

        private void LoadEquipment()
        {
            var table = Db.Query("SELECT * FROM dbo._05_02_Orgtehnika WHERE KodOrgtehniki = @id", Db.Param("@id", _equipmentId.Value));

            if (table.Rows.Count == 0)
            {
                MessageBox.Show("Оборудование не найдено.");
                Close();
                return;
            }

            var row = table.Rows[0];
            txtInventory.Text = row["InventarnyyNomer"].ToString();
            txtName.Text = row["Naimenovanie"].ToString();
            txtModel.Text = row["Model"].ToString();
            txtSerial.Text = row["SeriynyyNomer"].ToString();
            cmbSupplier.SelectedValue = Convert.ToInt32(row["KodPostavshchika"]);
            dpReceived.SelectedDate = Convert.ToDateTime(row["DataPostupleniya"]);
            txtWarranty.Text = row["SrokGarantiiMesyatsev"].ToString();
            txtCost.Text = Convert.ToDecimal(row["Stoimost"]).ToString("0.00");
            cmbStatus.SelectedValue = row["Status"].ToString();
            txtComment.Text = row["Kommentariy"] == DBNull.Value ? string.Empty : row["Kommentariy"].ToString();

            var photo = row["FotoPut"] == DBNull.Value ? PhotoHelper.NoPhoto : row["FotoPut"].ToString();
            SetPhoto(photo);
        }

        private void SetPhoto(string path)
        {
            _photoPath = string.IsNullOrWhiteSpace(path) ? PhotoHelper.NoPhoto : path;
            txtPhoto.Text = _photoPath;
            imgPreview.Source = PhotoHelper.Load(_photoPath);
        }

        private void btnChoosePhoto_Click(object sender, RoutedEventArgs e)
        {
            var selected = PhotoHelper.ChooseAndCopy();
            if (!string.IsNullOrWhiteSpace(selected))
                SetPhoto(selected);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtInventory.Text) ||
                string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtModel.Text) ||
                string.IsNullOrWhiteSpace(txtSerial.Text))
            {
                MessageBox.Show("Заполните обязательные поля: инвентарный номер, наименование, модель, серийный номер.");
                return;
            }

            if (cmbSupplier.SelectedValue == null)
            {
                MessageBox.Show("Выберите поставщика.");
                return;
            }

            if (!dpReceived.SelectedDate.HasValue)
            {
                MessageBox.Show("Укажите дату поступления.");
                return;
            }

            if (!int.TryParse(txtWarranty.Text, out var warranty) || warranty < 0)
            {
                MessageBox.Show("Срок гарантии должен быть неотрицательным целым числом.");
                return;
            }

            if (!TryParseDecimal(txtCost.Text, out var cost) || cost < 0)
            {
                MessageBox.Show("Стоимость должна быть неотрицательным числом.");
                return;
            }

            var status = cmbStatus.SelectedValue as string;
            if (string.IsNullOrWhiteSpace(status))
                status = "IN_USE";

            try
            {
                if (_equipmentId.HasValue)
                {
                    Db.Execute("UPDATE dbo._05_02_Orgtehnika SET InventarnyyNomer = @inventory, Naimenovanie = @name, Model = @model, SeriynyyNomer = @serial, KodPostavshchika = @supplier, DataPostupleniya = @received, SrokGarantiiMesyatsev = @warranty, Stoimost = @cost, Status = @status, Kommentariy = @comment, FotoPut = @photo, DataIzmeneniya = SYSDATETIME() WHERE KodOrgtehniki = @id", Db.Param("@inventory", txtInventory.Text.Trim()), Db.Param("@name", txtName.Text.Trim()), Db.Param("@model", txtModel.Text.Trim()), Db.Param("@serial", txtSerial.Text.Trim()), Db.Param("@supplier", cmbSupplier.SelectedValue), Db.Param("@received", dpReceived.SelectedDate.Value.Date), Db.Param("@warranty", warranty), Db.Param("@cost", cost), Db.Param("@status", status), Db.Param("@comment", string.IsNullOrWhiteSpace(txtComment.Text) ? (object)DBNull.Value : txtComment.Text.Trim()), Db.Param("@photo", _photoPath), Db.Param("@id", _equipmentId.Value));
                }
                else
                {
                    Db.Execute("INSERT INTO dbo._05_02_Orgtehnika (InventarnyyNomer, Naimenovanie, Model, SeriynyyNomer, KodPostavshchika, DataPostupleniya, SrokGarantiiMesyatsev, Stoimost, Status, Kommentariy, FotoPut) VALUES (@inventory, @name, @model, @serial, @supplier, @received, @warranty, @cost, @status, @comment, @photo)", Db.Param("@inventory", txtInventory.Text.Trim()), Db.Param("@name", txtName.Text.Trim()), Db.Param("@model", txtModel.Text.Trim()), Db.Param("@serial", txtSerial.Text.Trim()), Db.Param("@supplier", cmbSupplier.SelectedValue), Db.Param("@received", dpReceived.SelectedDate.Value.Date), Db.Param("@warranty", warranty), Db.Param("@cost", cost), Db.Param("@status", status), Db.Param("@comment", string.IsNullOrWhiteSpace(txtComment.Text) ? (object)DBNull.Value : txtComment.Text.Trim()), Db.Param("@photo", _photoPath));
                }

                DialogResult = true;
                Close();
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                MessageBox.Show("Инвентарный или серийный номер уже используется.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message);
            }
        }

        private static bool TryParseDecimal(string text, out decimal value)
        {
            if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out value))
                return true;

            return decimal.TryParse(text.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out value);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
