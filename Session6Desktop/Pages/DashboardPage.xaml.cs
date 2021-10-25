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

            ChartRatio.ChartAreas.Add(new ChartArea("Main"));
            ChartMonthly.ChartAreas.Add(new ChartArea("Main"));

            var departmentsSeries = new Series("Departments") { IsValueShownAsLabel = true };
            ChartRatio.Series.Add(departmentsSeries);

            Series currentSeries = new Series();
            currentSeries.ChartType = SeriesChartType.Pie;
            currentSeries.Points.Clear();

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
