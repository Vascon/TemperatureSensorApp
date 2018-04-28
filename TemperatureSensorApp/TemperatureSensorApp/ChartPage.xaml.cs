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

        int timeinterval = 1;

        public ChartPage()
        {
            InitializeComponent();

            WoodPicker.ItemsSource = new List<string> { "1","2","3","4"};

            WoodPicker.SelectedIndex = 0;

            WoodPicker.SelectedIndexChanged += (s, e) =>
            {
                timeinterval = Convert.ToInt32(WoodPicker.SelectedItem);
            };

            //Здесь хранится история температур
            history = Application.Current.Properties;

           

            ////Здесь хранятся средние значение для каждого часа за предыдущие 5 часов
            //var displaylist = new List<KeyValuePair<string, object>>();

            ////Рассчитываем среднее значение для каждого часа за предыдущие 5 часов
            //for (int i = 0; i <= 5; i++)
            //{
            //    var temporarylist = new List<KeyValuePair<string, object>>();

            //    foreach (KeyValuePair<string, object> temp in thishour)
            //    {
            //        if ((Convert.ToDateTime(temp.Key)).Minute == DateTime.Now.Minute - i )
            //        {
            //            temporarylist.Add(temp);
            //        }
            //    }
            //    if (temporarylist.Count() > 0)
            //        displaylist.Add(temporarylist.Average());
            //}

            //Создание графика по полученным значениям
            MakeChart(GetIntervals(timeinterval));
        }

        List<double> GetIntervals(int hourinterval)
        {
            var nhour = new List<double>();

            foreach (KeyValuePair<string, object> keyvalue in history)
            {
                if ((Convert.ToDateTime(keyvalue.Key).Hour <= DateTime.Now.Hour && (Convert.ToDateTime(keyvalue.Key).Hour > hourinterval) && (Convert.ToDateTime(keyvalue.Key).Day == DateTime.Now.Day) && (Convert.ToDateTime(keyvalue.Key).Month == DateTime.Now.Month) && (Convert.ToDateTime(keyvalue.Key).Year == DateTime.Now.Year)))
                {
                    nhour.Add(Convert.ToDouble(keyvalue.Value));
                }
            }

            nhour.Reverse();

            return nhour;
        }



        void Rebuild_Clicked(object sender, System.EventArgs e)
        {
            MakeChart(GetIntervals(1));


            //    try
            //    {
            //        var choosenhours = new List<double>();

            //        for (int i = StartTimePicker.Time.Hours; i <= EndTimePicker.Time.Hours; i++)
            //        {
            //            //Рассчитываем среднее значение для каждого часа за предыдущие 5 часов

            //            var hour = new List<double>();
            //            foreach (KeyValuePair<string, object> temp in history)
            //            {
            //                if ((Convert.ToDateTime(temp.Key)).Hour == i && (Convert.ToDateTime(temp.Key)).Day == DateTime.Today.Day && (Convert.ToDateTime(temp.Key)).Month == DateTime.Today.Month && (Convert.ToDateTime(temp.Key)).Year == DateTime.Today.Year)
            //                {
            //                    hour.Add(Convert.ToDouble(temp.Value));
            //                }
            //            }
            //            if (hour.Count() > 0)
            //                choosenhours.Add(hour.Average());
            //        }

            //        //Создание графика по полученным значениям
            //        MakeChart(choosenhours);
            //    }
            //    catch
            //    {
            //        DisplayAlert("", "The time frame is selected incorrectly", "Ok");
            //    }
            //}
        }

        //Метод для создание графика по полученным значениям
        private void MakeChart(List<double> hours)
        {
            //"Вхождения" для графика
            var entries = new List<Microcharts.Entry>();

            for (int i = hours.Count() - 1; i >= 0; i--)
            {
                if (i % 100 == 0)
                {
                    entries.Add(new Microcharts.Entry((float)hours[i])
                    {
                        //Label = $"{DateTime.Now.Hour - i}:00",
                        ValueLabel = hours[i].ToString("0.#"),
                        Color = SKColor.Parse("#266489")
                    });
                }
                else
                {
                    entries.Add(new Microcharts.Entry((float)hours[i])
                    {
                        //Label = $"{DateTime.Now.Hour - i}:00",
                        //ValueLabel = hours[i].ToString("0.#"),
                        Color = SKColor.Parse("#266489")
                    });
                }

            }

            //Заполняем график "вхождениями"
            var chart = new LineChart() { LabelTextSize = 40, Entries = entries };

            this.chartView.Chart = chart;
        }
    }
}
