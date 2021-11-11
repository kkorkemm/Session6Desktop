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
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;

namespace Session6Desktop.Pages
{
    using Base;

    /// <summary>
    /// Логика взаимодействия для DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();

            DownloadDiagrams();
            DownloadGrids();
        }

        private void DownloadDiagrams()
        {
            /// уникальные департаменты
            var departments = AppData.GetContext().EmergencyMaintenances.Where(p => p.EMEndDate != null).Select(p => p.Assets.DepartmentLocations.Departments.Name).ToList().Distinct();

            /// уникальные даты
            var dates = AppData.GetContext().Orders.Where(p => p.EmergencyMaintenances.EMEndDate != null).ToList().OrderByDescending(p => p.Date).Select(p => p.Date.ToString("yyyy/MM")).Distinct();

            #region RATIO

            ChartRatio.ChartAreas.Add(new ChartArea("Main"));
            var ratioSeries = new Series("Departments") { IsValueShownAsLabel = true };
            ratioSeries.ChartType = SeriesChartType.Pie;
            ChartRatio.Series.Add(ratioSeries);

            foreach (var department in departments)
            {
                var orders = AppData.GetContext().Orders.Where(p => p.EmergencyMaintenances.EMEndDate != null && p.EmergencyMaintenances.Assets.DepartmentLocations.Departments.Name == department).ToList();

                var y = ratioSeries.Points.AddY(orders.Sum(p => p.OrderItems.Sum(a => a.UnitPrice * a.Amount)));
                ratioSeries.Points[y].AxisLabel = department;
            }

            #endregion

            #region MONTHLY

            ChartMonthly.ChartAreas.Add(new ChartArea("Main"));
            var monthSeries = new Series("Spendings") { IsXValueIndexed = true };
            monthSeries.ChartType = SeriesChartType.Column;
            ChartMonthly.Series.Add(monthSeries);

            foreach (var department in departments)
            {
                var orders = AppData.GetContext().Orders.Where(p => p.EmergencyMaintenances.EMEndDate != null && p.EmergencyMaintenances.Assets.DepartmentLocations.Departments.Name == department).ToList();

                foreach (var date in dates)
                {
                    monthSeries.Points.AddXY(date, orders.Where(p => p.Date.ToString("yyyy/MM") == date).Sum(p => p.OrderItems.Sum(a => a.UnitPrice * a.Amount)));
                }
            }

            #endregion
        }

        private void DownloadGrids()
        {
            var dates = AppData.GetContext().Orders.Where(p => p.EmergencyMaintenances.EMEndDate != null).ToList().OrderByDescending(p => p.Date).Select(p => p.Date.ToString("yyyy/MM")).Distinct();

            foreach (var date in dates)
            {
                GridAssets.Columns.Add(new DataGridTextColumn { Header = date });
                GridDepartment.Columns.Add(new DataGridTextColumn { Header = date });
                GridParts.Columns.Add(new DataGridTextColumn { Header = date });
            }
        }


        #region Навигация и выход
        /// <summary>
        /// Переход на страницу Inventory Control
        /// </summary>
        private void BtnControl_Click(object sender, RoutedEventArgs e)
        {
            Navigation.MainFrame.Navigate(new ControlPage());
        }

        /// <summary>
        /// Закрытие приложения
        /// </summary>
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        #endregion
    }
}
