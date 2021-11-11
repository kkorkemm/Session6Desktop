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
        private List<OrderItems> partList = new List<OrderItems>();

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

        /// <summary>
        /// Поиск деталей
        /// </summary>
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
                try
                {
                    decimal amount = Convert.ToDecimal(TextAmount.Text);
                    list = list.Where(p => p.Amount == amount).ToList();
                }
                catch
                {
                    MessageBox.Show("Количеством может быть положительное число", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }  
            }

            // метод сортировки
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
                    list = list.OrderBy(p => p.UnitPrice * p.Amount).ToList();
                }
            }

            GridAllocatedParts.ItemsSource = list;
        }

        /// <summary>
        /// Добавление деталей в список
        /// </summary>
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
                partList.Add(i as OrderItems);
            }

            /// НАДО БЫЛО ДОБАВБЛЯТЬ В КОНЦЕ TOLIST() АААААААААААААА
            GridAssignedParts.ItemsSource = partList.ToList();
        }

        /// <summary>
        /// Удаление деталей из списка
        /// </summary>
        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button).DataContext as OrderItems;
            partList.Remove(item);

            GridAssignedParts.ItemsSource = partList.ToList();
        }

        /// <summary>
        /// Сохранение (Submit)
        /// </summary>
        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (GridAssignedParts.Items.Count == 0)
            {
                MessageBox.Show("Для сохранения необходимо выбрать не менее одной части", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var EM = ComboAsset.SelectedItem as EmergencyMaintenances;
            var warehouse = ComboWarehouse.SelectedItem as Warehouses;

            // сохранение заказа
            Orders newOrder = new Orders()
            {
                TransactionTypeID = 3,
                EmergencyMaintenancesID = EM.ID,
                DestinationWarehouseID = warehouse.ID,
                Date = DateTime.Now,
            };

            try
            {
                AppData.GetContext().Orders.Add(newOrder);
                AppData.GetContext().SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // получение ID только что созданного заказа
            var newOrdersID = AppData.GetContext().Orders.OrderBy(p => p.ID).ToList().LastOrDefault();

            // добавление частей
            for (int i = 0; i < GridAssignedParts.Items.Count; i++)
            {
                var item = GridAssignedParts.Items[i] as OrderItems;

                OrderItems newItem = new OrderItems()
                {
                    OrderID = newOrdersID.ID,
                    PartID = item.PartID,
                    Amount = item.Amount,
                    UnitPrice = item.UnitPrice,
                    BatchNumber = item.BatchNumber,
                    Stock = item.Stock
                };

                AppData.GetContext().OrderItems.Add(newItem);
            }

            // сохранение
            try
            {
                AppData.GetContext().SaveChanges();
                MessageBox.Show("Детали успешно сохранены!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Information);
                Navigation.MainFrame.Navigate(new DashboardPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
