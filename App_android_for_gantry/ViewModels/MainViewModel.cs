using App_android_for_gantry.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace App_android_for_gantry.ViewModels // Dung namespace de to chuc file code
{
    // Get, gia tri (Pos_X, Pos_Y, Pos_Z) tu Class MainViewModel 
    /// ///////////////////////////////////////////////////////////////////////////////////
    // Pos_X
    public class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        // Pos_X
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

        // Pos_Y
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

        // Pos_Z
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
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////





        /// Medicine
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Medicine_A
        private ushort _WorMedicineA;
        public ushort WorMedicineA
        {
            get => _WorMedicineA;
            set
            {
                if (_WorMedicineA != value)
                {
                    _WorMedicineA = value;
                    OnPropertyChanged();
                }
            }
        }

        // Medicine_B
        private ushort _WorMedicineB;
        public ushort WorMedicineB
        {
            get => _WorMedicineB;
            set
            {
                if (_WorMedicineB != value)
                {
                    _WorMedicineB = value;
                    OnPropertyChanged();
                }
            }
        }

        // Medicine_C
        private ushort _WorMedicineC;
        public ushort WorMedicineC
        {
            get => _WorMedicineC;
            set
            {
                if (_WorMedicineC != value)
                {
                    _WorMedicineC = value;
                    OnPropertyChanged();
                }
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Ham duoc goi khi gia tri cua _realPosX, _realPosY,_realPosZ
        /// //////////////////////////////////////////////////////////////////      
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        private readonly ModbusService _modbusService;
        private bool _isReading_Pos = false;
        public MainViewModel()
        {
            _modbusService = new ModbusService(); // Khoi tao class ModbusService su dung thong qua tu khoa  _modbusService, chi doc ra tu Class ModbusService
            StartReadingPositions();
            StartReadingMedicine();
        }

        // Ham doc RealPosX, RealPosY, RealPosZ 
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void StartReadingPositions()
        {
            Task.Run(async () =>
            {
                await _modbusService.ConnectPLCAsync();
                _isReading_Pos = true;
                while (_isReading_Pos && _modbusService.IsConnected)
                {
                    System.Diagnostics.Debug.WriteLine($"IsConnected_2: {_modbusService.IsConnected}");
                    try
                    {
                        // Đọc ba giá trị cùng lúc
                        var readXTask = _modbusService.ReadLREALAsync(135);
                        var readYTask = _modbusService.ReadLREALAsync(4);
                        var readZTask = _modbusService.ReadLREALAsync(6);

                        // Đợi tất cả các tác vụ đọc xong
                        await Task.WhenAll(readXTask, readYTask, readZTask);

                        // Cập nhật các giá trị sau khi hoàn thành đọc
                        RealPosX = Math.Round(await readXTask, 0);                       
                        RealPosY = Math.Round(await readYTask, 0);
                        RealPosZ = Math.Round(await readZTask, 0);

                    }
                    catch
                    {

                    }

                    await Task.Delay(100);
                }
            });
        }
        // ////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        public void StartReadingMedicine()
        {
            Task.Run(async () =>
            {
                await _modbusService.ConnectPLCAsync();
                _isReading_Pos = true;
                while (_isReading_Pos && _modbusService.IsConnected)
                {                  
                    try
                    {
                        // Đọc ba giá trị cùng lúc
                        var wordATask = _modbusService.ReadWordAsync(190);
                        var wordBTask = _modbusService.ReadWordAsync(58);
                        var wordCTask = _modbusService.ReadWordAsync(60);

                        // Đợi tất cả các tác vụ đọc xong
                        await Task.WhenAll(wordATask, wordBTask, wordCTask);

                        // Cập nhật các giá trị sau khi hoàn thành đọc
                        WorMedicineA = wordATask.Result;
                        WorMedicineB = wordBTask.Result;
                        WorMedicineC = wordCTask.Result;

                    }
                    catch
                    {

                    }

                    await Task.Delay(100);
                }
            });
        }
        // ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
