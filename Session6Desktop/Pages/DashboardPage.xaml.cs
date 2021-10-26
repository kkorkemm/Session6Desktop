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

            #region Пока думаю
            //ChartRatio.ChartAreas.Add(new ChartArea("Main"));
            //ChartMonthly.ChartAreas.Add(new ChartArea("Main"));

            //var departmentsSeries = new Series("Departments") { IsValueShownAsLabel = true };
            //ChartRatio.Series.Add(departmentsSeries);
            //var monthSeries = new Series("Months") { IsValueShownAsLabel = true };
            //ChartMonthly.Series.Add(monthSeries);

            //Series currentSeries = new Series();
            //currentSeries.ChartType = SeriesChartType.Pie;
            //currentSeries.Points.Clear();
            #endregion

            // уникальные отделы с запросами на EM
            var departments = AppData.GetContext().EmergencyMaintenances.Where(p => p.EMStartDate != null && p.EMEndDate != null).ToList().Select(p => p.Assets.DepartmentLocations.Departments.Name).Distinct();

            // уникальные даты с запросами на EM
            var dates = AppData.GetContext().Orders.Where(p => p.EmergencyMaintenances.EMStartDate != null && p.EmergencyMaintenances.EMEndDate != null).OrderByDescending(p => p.Date).ToList().Select(p => p.Date.ToString("yyyy/MM")).Distinct();

            foreach (var date in dates)
            {
                GridDepartment.Columns.Add(new DataGridTextColumn { Header = date, Binding = new Binding()});
                GridAssets.Columns.Add(new DataGridTextColumn { Header = date});
                GridParts.Columns.Add(new DataGridTextColumn { Header = date});
            }

            foreach (var department in departments)
            {
                var row = new List<string>();
                row.Add(department);
                 
                foreach (var date in dates)
                {
                    var orders = AppData.GetContext().Orders.Where(p => p.EmergencyMaintenances.EMStartDate != null && p.EmergencyMaintenances.EMEndDate != null && p.EmergencyMaintenances.Assets.DepartmentLocations.Departments.Name == department).ToList();

                    var spendings = orders.Where(p => p.Date.ToString("yyyy/MM") == date).Select(p => p.OrderItems.Sum(a => a.UnitPrice * a.Amount)).Sum().Value.ToString();

                    row.Add(spendings);
                }

                GridDepartment.Items.Add(row.ToArray());
            }
        }

        /// <summary>
        /// Многоязычная поддержка 
        /// </summary>
        private void WorkWithXml()
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(@"C:\Users\Asus\Desktop\WSC2019_TP09_actual\Session6\default.xml");

            XmlElement RootElement = xmlDocument.DocumentElement;
        }

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
    }
}
