using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Session6Desktop.Pages
{
    using Base;

    /// <summary>
    /// Логика взаимодействия для ControlPage.xaml
    /// </summary>
    public partial class ControlPage : Page
    {
        public ControlPage()
        {
            InitializeComponent();

            ComboWarehouse.ItemsSource = AppData.GetContext().Warehouses.ToList();
            ComboAsset.ItemsSource = AppData.GetContext().EmergencyMaintenances.Where(p => p.EMEndDate == null && p.EMStartDate != null).ToList();
        }

        /// <summary>
        /// Изменение элементов в ComboAssets
        /// </summary>
        /// 
        private void UpdateComboAssets()
        {
            Warehouses sourceWarehouse = ComboWarehouse.SelectedItem as Warehouses;
            EmergencyMaintenances EM = ComboAsset.SelectedItem as EmergencyMaintenances;

            if (sourceWarehouse == null)
                return;

            var list = AppData.GetContext().OrderItems.Where(p => p.Orders.SourceWarehouseID == sourceWarehouse.ID && p.Orders.Date < DateDate.SelectedDate).ToList();

            ComboParts.ItemsSource = list;

            if (EM == null)
                return;

            GridAssignedParts.ItemsSource = list.Where(p => p.Orders.EmergencyMaintenancesID == EM.ID).ToList();
        }

        private void ComboWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateComboAssets();
        }

        private void DateDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateComboAssets();
        }

        private void BtnAllocate_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (ComboAsset.SelectedItem == null)
                errors.AppendLine("Выберите актив");
            if (DateDate.SelectedDate == null)
                errors.AppendLine("Выберите дату");
            if (ComboWarehouse.SelectedItem == null)
                errors.AppendLine("Выберите склад");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var list = AppData.GetContext().OrderItems.ToList();

            if (ComboParts.SelectedItem != null)
            {
                OrderItems part = ComboParts.SelectedItem as OrderItems;
                list = list.Where(p => p.PartID == part.PartID).ToList();
            }
            if (!string.IsNullOrWhiteSpace(TextAmount.Text))
            {
                decimal amount = Convert.ToDecimal(TextAmount.Text);
                list = list.Where(p => p.Amount == amount).ToList();
            }
            if (ComboMethod.SelectedItem != null)
            {
                if (ComboMethod.SelectedIndex == 0)
                {
                    list = list.OrderBy(p => p.Orders.Date).ToList();
                }
                if (ComboMethod.SelectedIndex == 1)
                {
                    list = list.OrderByDescending(p => p.Orders.Date).ToList();
                }
                if (ComboMethod.SelectedIndex == 2)
                {
                    list = list.OrderBy(p => p.UnitPrice).ToList();
                }
            }

            GridAllocatedParts.ItemsSource = list;
        }

        private void BtnAssign_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = GridAllocatedParts.SelectedItems;

            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Выберите не менее одной части", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var EM = ComboAsset.SelectedItem as EmergencyMaintenances;
            var order = AppData.GetContext().Orders.Where(p => p.EmergencyMaintenancesID == EM.ID).FirstOrDefault();

            foreach (var i in selectedItems)
            {

            }

            GridAssignedParts.ItemsSource = selectedItems;
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (GridAssignedParts.Items.Count == 0)
            {
                MessageBox.Show("Для сохранения необходимо выбрать не менее одной части", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        /// <summary>
        /// Назад (cancel)
        /// </summary>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Navigation.MainFrame.GoBack();
        }
    }
}
