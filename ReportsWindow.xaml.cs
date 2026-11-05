using System;
using System.Windows;

namespace OrgTechApp
{
    public partial class ReportsWindow : Window
    {
        public ReportsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadReports();
        }

        private void LoadReports()
        {
            try
            {
                dgAvailability.ItemsSource = Db.Query("SELECT Podrazdelenie, Kolichestvo FROM dbo._05_02_vw_NalichiePoPodrazdeleniyam ORDER BY Podrazdelenie").DefaultView;

                dgWrittenOff.ItemsSource = Db.Query("SELECT DataOperatsii, InventarnyyNomer, Naimenovanie, Model, SeriynyyNomer, Prichina, NomerDokumenta, Ispolnitel FROM dbo._05_02_vw_IstoriyaPeremeshcheniy WHERE KodOperatsii = 'WRT' ORDER BY DataOperatsii DESC").DefaultView;

                dgHistory.ItemsSource = Db.Query("SELECT TOP (300) DataOperatsii, Operatsiya, InventarnyyNomer, Naimenovanie, Ispolnitel, Poluchatel, NomerDokumenta FROM dbo._05_02_vw_IstoriyaPeremeshcheniy ORDER BY DataOperatsii DESC, KodPeremeshcheniya DESC").DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка отчётов: " + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadReports();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
