using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Gpio;
using Windows.UI.Popups;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Lab3
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        GpioController gpio;
        GpioPin systemstatus;
        GpioPin sensor;
        GpioPin sensorstatus;
        int count = 0;

        public MainPage()
        {
            this.InitializeComponent();
            initializeGPIO();
        }

        private async void initializeGPIO()
        {
            gpio = GpioController.GetDefault();

            if (gpio == null)
            {   
                MessageDialog dialog = new MessageDialog("This device doesn't have any GPIO Controller.");
                await dialog.ShowAsync();
                return;
            }

            // define and enable sensor status pin
            sensorstatus = gpio.OpenPin(24);
            sensorstatus.SetDriveMode(GpioPinDriveMode.Output);

            // define and enable sensor pin
            sensor = gpio.OpenPin(16);
            sensor.SetDriveMode(GpioPinDriveMode.Input);
            sensor.ValueChanged += sensor_ValueChanged;

            // define and enable system ready pin
            systemstatus = gpio.OpenPin(23);
            systemstatus.SetDriveMode(GpioPinDriveMode.Output);
            systemstatus.Write(GpioPinValue.High);
            rect.Fill = new SolidColorBrush(Colors.Green);
            
        }

        private void sensor_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            GpioPinValue status = sensor.Read();

            if (status == GpioPinValue.High)
            {
                // do your work here.

                count++;
                var task = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    countTB.Text = count.ToString();
                });
                
            }

            // make sure the sensor status remain on when the system is on
            while (status == GpioPinValue.High)
            {
                sensorstatus.Write(GpioPinValue.High);
                status = sensor.Read();
            }

            // turn off the sensor status light
            sensorstatus.Write(GpioPinValue.Low);
        }


    }
}
