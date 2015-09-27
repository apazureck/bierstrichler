using Bierstrichler.Data.Items;
using Bierstrichler.Data.Persons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bierstrichler.Functional
{
    public static class BeerCharts
    {
        public static event EventHandler ChartsUpdated;

        private static SortedList<int, List<Consumer>> chartList = new SortedList<int, List<Consumer>>();

        public static SortedList<int, List<Consumer>> ChartList { get { return chartList; } }

        public static PersonList Persons {get; set;}

        public static ItemList Items { get; set; }

        public static void AddToCharts(Consumer c)
        {
            if(c.ListedInCharts)
            {
                c.MakeTotalChartDrinks();
                AddToSubList(c.TotalChartDrinks, c);
            }
        }

        public static int GetTotalChartDrinks(Consumer c)
        {
            int i = 0;
            foreach(Consumed con in c.History)
            {
                try
                {
                    Item itm = Items.Find(x => x.ID == con.ItemID);
                    if (itm!=null&&itm.Coleurfaehig)
                        i++;
                }
                catch { }
            }
            return i;
        }

        private static void AddToSubList(int key, Consumer c)
        {
            List<Consumer> subList;
            if(chartList.TryGetValue(key, out subList))
                subList.Add(c);
            else
            {
                subList = new List<Consumer>();
                subList.Add(c);
                chartList.Add(key, subList);
            }
        }

        public static void UpdateCharts()
        {
            chartList.Clear();
            foreach(Consumer c in Persons)
            {
                try
                {
                    AddToCharts(c);
                }
                catch {}
            }
            if (ChartsUpdated != null)
                ChartsUpdated(null, new EventArgs());
        }

        public static int GetRank(Consumer c)
        {
            int rank = chartList.Count;
            foreach(List<Consumer> subList in chartList.Values)
            {
                if(subList.Contains(c))
                    return rank;
                rank--;
            }
            throw new Exception("User not listed in Charts!");
        }

        public static async void UpdateChartsAsync()
        {
            await Task.Run(() =>
            {
                UpdateCharts();
            });
        }

        public static int GetDeficite(Consumer consumer)
        {
            int key = consumer.TotalChartDrinks;
            int place = chartList.IndexOfKey(key);
            int advancedDrinks = chartList.Keys[place+1];
            return advancedDrinks - key;
        }

        public static int GetAdvance(Consumer consumer)
        {
            int key = consumer.TotalChartDrinks;
            int place = chartList.IndexOfKey(key);
            int advancedDrinks = chartList.Keys[place - 1];
            return key - advancedDrinks;
        }
    }
}
