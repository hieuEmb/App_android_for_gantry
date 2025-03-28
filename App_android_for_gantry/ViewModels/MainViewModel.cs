using App_android_for_gantry.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace App_android_for_gantry.ViewModels // Dung namespace de to chuc file code
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private double _realPosX;
        public double RealPosX
        {
            get => _realPosX;
            set
            {
                if (_realPosX != value)
                {
                    _realPosX = value;
                    OnPropertyChanged();
                }
            }
        }
        private double _realPosY;
        public double RealPosY
        {
            get => _realPosY;
            set
            {
                if (_realPosY != value)
                {
                    _realPosY = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _realPosZ;
        public double RealPosZ
        {
            get => _realPosZ;
            set
            {
                if (_realPosZ != value)
                {
                    _realPosZ = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly ModbusService _modbusService;

        public MainViewModel()
        {
            _modbusService = new ModbusService(); // Giả sử bạn đã có class ModbusService
            StartReadingPositions();
        }

        private void StartReadingPositions()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        if (!_modbusService.IsConnected)
                        {
                            bool success = await _modbusService.ConnectPLCAsync();
                            if (!success)
                            {
                                await Task.Delay(2000);
                                continue;
                            }
                        }

                        RealPosX = await _modbusService.ReadLREALAsync(60);
                        RealPosY = await _modbusService.ReadLREALAsync(4);
                        RealPosZ = await _modbusService.ReadLREALAsync(6);
                    }
                    catch
                    {


                        // Bỏ qua lỗi, tiếp tục vòng lặp
                    }

                    await Task.Delay(500);
                }
            });
        }
    }
}
