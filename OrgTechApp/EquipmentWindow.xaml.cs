using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace OrgTechApp
{
    public partial class EquipmentWindow : Window
    {
        public EquipmentWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cmbStatus.ItemsSource = new List<StatusItem>
            {
                new StatusItem { Code = "", Name = "Все состояния" },
                new StatusItem { Code = "IN_USE", Name = "В эксплуатации" },
                new StatusItem { Code = "IN_REPAIR", Name = "В ремонте" },
                new StatusItem { Code = "WRITTEN_OFF", Name = "Списана" }
            };
            cmbStatus.SelectedIndex = 0;

            var departments = new List<LookupItem> { new LookupItem { Id = 0, Name = "Все подразделения" } };
            var table = Db.Query("SELECT KodPodrazdeleniya, Naimenovanie FROM dbo._05_02_Podrazdelenie WHERE Aktivno = 1 ORDER BY Naimenovanie");
            foreach (DataRow row in table.Rows)
            {
                departments.Add(new LookupItem
                {
                    Id = Convert.ToInt32(row["KodPodrazdeleniya"]),
                    Name = row["Naimenovanie"].ToString()
                });
            }
            cmbDepartment.ItemsSource = departments;
            cmbDepartment.SelectedIndex = 0;

            btnAdd.Visibility = Session.CanEditEquipment ? Visibility.Visible : Visibility.Collapsed;
            btnEdit.Visibility = Session.CanEditEquipment ? Visibility.Visible : Visibility.Collapsed;
            btnOperate.Visibility = Session.CanOperate ? Visibility.Visible : Visibility.Collapsed;

            LoadEquipment();
        }

        private void LoadEquipment()
        {
            try
            {
                var search = txtSearch.Text.Trim();
                var status = cmbStatus.SelectedValue as string;
                var department = cmbDepartment.SelectedValue is int id ? id : 0;

                var table = Db.Query("SELECT KodOrgtehniki, InventarnyyNomer, Naimenovanie, Model, SeriynyyNomer, Status, StatusNaimenovanie, FIO, Podrazdelenie, KodPodrazdeleniya, DataPostupleniya, SrokGarantiiMesyatsev, Stoimost, Kommentariy, FotoPut, KodSotrudnika FROM dbo._05_02_vw_OrgtehnikaSostoyanie WHERE (@search IS NULL OR Naimenovanie LIKE @search OR Model LIKE @search OR SeriynyyNomer LIKE @search OR InventarnyyNomer LIKE @search) AND (@status IS NULL OR Status = @status) AND (@department = 0 OR KodPodrazdeleniya = @department) ORDER BY Naimenovanie, Model", Db.Param("@search", string.IsNullOrWhiteSpace(search) ? (object)DBNull.Value : "%" + search + "%"), Db.Param("@status", string.IsNullOrWhiteSpace(status) ? (object)DBNull.Value : status), Db.Param("@department", department));

                dgEquipment.ItemsSource = table.DefaultView;
                if (table.Rows.Count == 0)
                {
                    ClearDetails();
                }
                else
                {
                    dgEquipment.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки оборудования: " + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ClearDetails()
        {
            imgPhoto.Source = PhotoHelper.Load(PhotoHelper.NoPhoto);
            lblName.Text = "Оборудование не найдено";
            lblStatus.Text = string.Empty;
            lblInventory.Text = string.Empty;
            lblModel.Text = string.Empty;
            lblSerial.Text = string.Empty;
            lblFio.Text = string.Empty;
            lblDepartment.Text = string.Empty;
            lblReceived.Text = string.Empty;
            lblWarranty.Text = string.Empty;
            lblCost.Text = string.Empty;
            lblComment.Text = string.Empty;
        }

        private DataRowView SelectedRow
        {
            get { return dgEquipment.SelectedItem as DataRowView; }
        }

        private void dgEquipment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var row = SelectedRow;
            if (row == null)
            {
                ClearDetails();
                return;
            }

            imgPhoto.Source = PhotoHelper.Load(row["FotoPut"].ToString());
            lblName.Text = row["Naimenovanie"].ToString();
            lblStatus.Text = row["StatusNaimenovanie"].ToString();
            lblInventory.Text = row["InventarnyyNomer"].ToString();
            lblModel.Text = row["Model"].ToString();
            lblSerial.Text = row["SeriynyyNomer"].ToString();
            lblFio.Text = row["FIO"] == DBNull.Value ? "Не закреплено" : row["FIO"].ToString();
            lblDepartment.Text = row["Podrazdelenie"] == DBNull.Value ? "Не закреплено" : row["Podrazdelenie"].ToString();
            lblReceived.Text = Convert.ToDateTime(row["DataPostupleniya"]).ToString("dd.MM.yyyy");
            lblWarranty.Text = row["SrokGarantiiMesyatsev"] + " мес.";
            lblCost.Text = Convert.ToDecimal(row["Stoimost"]).ToString("N2") + " ₽";
            lblComment.Text = row["Kommentariy"] == DBNull.Value ? string.Empty : row["Kommentariy"].ToString();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsLoaded)
                LoadEquipment();
        }

        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
                LoadEquipment();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadEquipment();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!Session.CanEditEquipment)
            {
                MessageBox.Show("Недостаточно прав для добавления оборудования.");
                return;
            }

            var window = new EquipmentEditWindow(null) { Owner = this };
            if (window.ShowDialog() == true)
                LoadEquipment();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            EditSelected();
        }

        private void dgEquipment_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EditSelected();
        }

        private void EditSelected()
        {
            if (!Session.CanEditEquipment)
                return;

            var row = SelectedRow;
            if (row == null)
            {
                MessageBox.Show("Выберите оборудование.");
                return;
            }

            var window = new EquipmentEditWindow(Convert.ToInt32(row["KodOrgtehniki"])) { Owner = this };
            if (window.ShowDialog() == true)
                LoadEquipment();
        }

        private void btnOperate_Click(object sender, RoutedEventArgs e)
        {
            if (!Session.CanOperate)
            {
                MessageBox.Show("Недостаточно прав для оформления операций.");
                return;
            }

            var row = SelectedRow;
            if (row == null)
            {
                MessageBox.Show("Выберите оборудование.");
                return;
            }

            var window = new OperationWindow(Convert.ToInt32(row["KodOrgtehniki"])) { Owner = this };
            window.ShowDialog();
            LoadEquipment();
        }
    }
}
