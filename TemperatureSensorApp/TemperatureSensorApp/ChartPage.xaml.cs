using System;
using System.Collections.Generic;
using SkiaSharp;
using Xamarin.Forms;
using Microcharts.Forms;
using Microcharts;
using System.Collections;
using System.Linq;

namespace TemperatureSensorApp
{
    public partial class ChartPage : ContentPage
    {
        IDictionary<string, object> history;

        public ChartPage()
        {
            InitializeComponent();

            //Здесь хранится история температур
            history = Application.Current.Properties;

            //Здесь хранятся средние значение для каждого часа за предыдущие 5 часов
            var lasthours = new List<double>();

            //Рассчитываем среднее значение для каждого часа за предыдущие 5 часов
            for (int i = 0; i <= 5;i++)
            {
                var hour = new List<double>();
                foreach(KeyValuePair<string,object> temp in history)
                {
                    if ((Convert.ToDateTime(temp.Key)).Hour==DateTime.Now.Hour-i)
                    {
                        hour.Add(Convert.ToDouble(temp.Value));
                    }
                }
                if (hour.Count()>0)
                    lasthours.Add(hour.Average());
            }

            //Создание графика по полученным значениям
            MakeChart(lasthours);
        }

        void Rebuild_Clicked(object sender, System.EventArgs e)
        {
            try
            {
                var choosenhours = new List<double>();

                for (int i = StartTimePicker.Time.Hours; i < EndTimePicker.Time.Hours; i++)
                {
                    //Рассчитываем среднее значение для каждого часа за предыдущие 5 часов

                    var hour = new List<double>();
                    foreach (KeyValuePair<string, object> temp in history)
                    {
                        if ((Convert.ToDateTime(temp.Key)).Hour == DateTime.Now.Hour - i)
                        {
                            hour.Add(Convert.ToDouble(temp.Value));
                        }
                    }
                    if (hour.Count() > 0)
                        choosenhours.Add(hour.Average());
                }

                //Создание графика по полученным значениям
                MakeChart(choosenhours);
            }
            catch
            {
                DisplayAlert("", "The time frame is selected incorrectly", "Ok");
            }
        }

        //Метод для создание графика по полученным значениям
        private void MakeChart(List<double> hours)
        {
            //"Вхождения" для графика
            var entries = new List<Microcharts.Entry>();

            for (int i = hours.Count() - 1; i >= 0; i--)
            {
                entries.Add(new Microcharts.Entry((float)hours[i])
                {
                    Label = $"{DateTime.Now.Hour - i}:00",
                    ValueLabel = hours[i].ToString("0.#"),
                    Color = SKColor.Parse("#266489")
                });

            }

            //Заполняем график "вхождениями"
            var chart = new LineChart() { LabelTextSize = 40, Entries = entries };

            this.chartView.Chart = chart;
        }
    }
}
