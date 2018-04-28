using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TemperatureSensorApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TemperaturePage : ContentPage
    {
        private bool isCelsius = true;


        public TemperaturePage()
        {
            InitializeComponent();
           

            this.Appearing += async (s, e) =>
            {
                //ВОТ СЮДА ВСТАВИТЬ ДЕВАЙССЁРЧ!!!!!!!!!!!
                //DeviceSearch()
            };


            //Температура по дефолту
            CurrentTemperatureLabel.Text = "32";

            //Ивент, срабатывающий при изменении единиц измерения (цельсий/фарингейт)
            ScaleSwitch.Toggled += (s, e) =>
            {
                isCelsius = !isCelsius;
                CurrentTemperatureLabel.Text = CelsiusFahrenheit(CurrentTemperatureLabel.Text);
            };

            //Таймер, срабатывающий раз в 1.5 секунды, для обновления интерфейса
            int num = 0;
            // устанавливаем метод обратного вызова
            TimerCallback tm = new TimerCallback(Refresh);
            // создаем таймер
            Timer timer = new Timer(tm, num, 0, 1500);
        }

        //Метод для обновления значения температуры
        private void Refresh(object obj)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                int x = (int)obj;
                var currtemp = (Convert.ToDouble(CurrentTemperatureLabel.Text) + 1).ToString();

                //TODO: получение данных с датчика

                CurrentTemperatureLabel.Text = currtemp;

                //Вычисление цвета для текуще температуры
                var currcolor = ColorFromTemperature(currtemp);

                //Изменение цвета надписи и круга
                CurrentTemperatureLabel.TextColor = currcolor;
                Circle.BorderColor = currcolor;

                //Сохранение температуры в цельсиях;
                var temptosave = currtemp;
                if (!isCelsius)
                {
                    temptosave = ((Convert.ToDouble(currtemp) - 32) * 5 / 9).ToString("0.#");
                }
                Application.Current.Properties[$"{DateTime.Now}"] = temptosave;
                Application.Current.SavePropertiesAsync();
            });
        }

        /// <summary>
        /// Поиск BLE-устройства
        /// </summary>
        public async Task DeviceSearch()
        {
            ActI.IsRunning = true;

            var ble = CrossBluetoothLE.Current;
            var adapter = CrossBluetoothLE.Current.Adapter;
            var deviceList = new List<IDevice>();
            IDevice temperatureSensorDevice = null;
            bool requiredDeviceFound = false;

            //Ивент составление списка найденных устройств
            adapter.DeviceDiscovered += (ss, a) => deviceList.Add(a.Device);

            //Инициализация таска поиска устройств
            await adapter.StartScanningForDevicesAsync();

            try
            {
                //Если хоть что-то найдено
                if (deviceList != null)
                {
                    //Ищем нужное устройство по Name
                    foreach (IDevice device in deviceList)
                    {
                        if (device.Name != null && device.Name.Contains("MI Band 2"))
                        {
                            temperatureSensorDevice = device;
                            requiredDeviceFound = true;
                        }
                    }
                    //Если нужное устройство не найдено
                    if (!requiredDeviceFound)
                    {
                        //Запустить данный метод рекурсивно
                        await DeviceSearch();
                    }
                    //Если нужное устройство найдено и оно не null
                    else
                    if (temperatureSensorDevice != null)
                    {
                        //Коннектимся к нему
                        await adapter.ConnectToDeviceAsync(temperatureSensorDevice);

                        //var services = await temperatureSensorDevice.GetServicesAsync();

                        //Выводим список сервисов данного устройства
                        var service = await temperatureSensorDevice.GetServiceAsync(Guid.Parse("0000180a-0000-1000-8000-00805f9b34fb"));

                        //Выводим список характеристик данного сервиса
                        var characteristics = await service.GetCharacteristicsAsync();

                        //Получаем данные из необходимой характеристики сервиса
                        var temperature = await characteristics[0].ReadAsync();



                        while (!!!!!!!!true)
                        {
                            //Конвертим его из UTF8
                            string result = System.Text.Encoding.UTF8.GetString(temperature);

                            var answer = await DisplayAlert("", result, "Оk","Cancel");

                            if (!answer)
                            {
                                break;
                            }

                            await Task.Delay(TimeSpan.FromSeconds(3));

                        }


                        //var temperature1 = await characteristics[1].WriteAsync(new byte[] { 21, 2, 1 });
                    }
                    else
                    {
                    }
                }

            }
            catch (Exception ee)
            {
                await DisplayAlert("", ee.Message, "Ок");
            }

            ActI.IsRunning = false;
        }


        //Действие по нажатию на кнопку "Build"
        async void Build_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new ChartPage());
        }

        //Действие по нажатию на кнопку "Connect"
        async void ConnectToBLE_Clicked(object sender, System.EventArgs e)
        {
            await DeviceSearch();

            //Navigation.PushAsync(new ChartPage());
        }

        //Метод перевода из цельсия в фаренгейты и наоборот
        private string CelsiusFahrenheit(string temperature)
        {
            if (!isCelsius)
            {
                return (Convert.ToDouble(temperature) * 9 / 5 + 32).ToString("0.#");
            }
            else
            {
                return ((Convert.ToDouble(temperature) - 32) * 5 / 9).ToString("0.#");
            }
        }

        //Метод получения цвета из температуры
        private Color ColorFromTemperature(string temperature)
        {
            var temp = Convert.ToDouble(temperature);
            if (!isCelsius)
            {
                temp = ((Convert.ToDouble(temperature) - 32) * 5 / 9);
            }

            #region const
            if (temp < 34) return Color.FromHex("#9bbcff");
            if (temp >= 34 && temp <= 35) return Color.FromHex("#a1b7ff");
            if (temp >= 35 && temp <= 36) return Color.FromHex("#b5cdff");
            if (temp >= 36 && temp <= 37) return Color.FromHex("#d6e1ff");
            if (temp >= 37 && temp <= 38) return Color.FromHex("#edefff");
            if (temp >= 38 && temp <= 39) return Color.FromHex("#fff9fd");
            if (temp >= 39 && temp <= 40) return Color.FromHex("#ffedda");
            if (temp >= 40 && temp <= 41) return Color.FromHex("#ffe5c8");
            if (temp >= 41 && temp <= 42) return Color.FromHex("#ffc994");
            if (temp >= 42 && temp <= 43) return Color.FromHex("#ffc37c");
            if (temp >= 43 && temp <= 44) return Color.FromHex("#ffa148");
            if (temp >= 44 && temp <= 45) return Color.FromHex("#ff8d0b");
            if (temp >= 45 && temp <= 46) return Color.FromHex("#ff8200");
            if (temp >= 46 && temp <= 47) return Color.FromHex("#ff6500");
            if (temp >= 47) return Color.FromHex("#ff3800");

            #endregion


            return Color.FromHex("#ffece0");
        }

    }
}