using Bierstrichler.Data.Persons;
using Bierstrichler.Functional;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bierstrichler.ViewModels
{
    public class BeerChartsViewModel : ViewModelBase
    {
        public BeerChartsViewModel()
        {
            BeerCharts.ChartsUpdated += BeerCharts_ChartsUpdated;
        }

        void BeerCharts_ChartsUpdated(object sender, EventArgs e)
        {
            MakeChartview();
        }

        private async void MakeChartview()
        {
            await Task.Run(() =>
            {
                chartList.Clear();
                int k = 0;
                for (int j = Model.Count - 1; j > -1; j--)
                {
                    k++;
                    foreach (Consumer c in Model.ElementAt(j).Value)
                        chartList.Add(new KeyValuePair<int, Consumer>(k, c));
                }
            });
            RaisePropertyChangedForAll();

        }
        System.Collections.Generic.SortedList<int, List<Consumer>> Model { get { return BeerCharts.ChartList; } }

        public ObservableCollection<KeyValuePair<int, Consumer>> ChartList
        {
            get 
            {
                return new ObservableCollection<KeyValuePair<int,Consumer>>(chartList);
            }
        }

        private List<KeyValuePair<int, Consumer>> chartList = new List<KeyValuePair<int, Consumer>>();
        

    }
}
