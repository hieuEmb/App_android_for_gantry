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


        // Medicine_D
        private ushort _WorMedicineD;
        public ushort WorMedicineD
        {
            get => _WorMedicineD;
            set
            {
                if (_WorMedicineD != value)
                {
                    _WorMedicineD = value;
                    OnPropertyChanged();
                }
            }
        }


        // Medicine_E
        private ushort _WorMedicineE;
        public ushort WorMedicineE
        {
            get => _WorMedicineE;
            set
            {
                if (_WorMedicineE != value)
                {
                    _WorMedicineE = value;
                    OnPropertyChanged();
                }
            }
        }


        // Medicine_F
        private ushort _WorMedicineF;
        public ushort WorMedicineF
        {
            get => _WorMedicineF;
            set
            {
                if (_WorMedicineF != value)
                {
                    _WorMedicineF = value;
                    OnPropertyChanged();
                }
            }
        }

        // Medicine_G
        private ushort _WorMedicineG;
        public ushort WorMedicineG
        {
            get => _WorMedicineG;
            set
            {
                if (_WorMedicineG != value)
                {
                    _WorMedicineG = value;
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
                        var readXTask = _modbusService.ReadLREALAsync(2);
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


        // Thêm biến lưu trạng thái lần trước
        private Dictionary<string, ushort> lastQuantities = new();
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
                        // Đọc nhiều giá trị cùng lúc
                        var wordATask = _modbusService.ReadWordAsync(55);
                        var wordBTask = _modbusService.ReadWordAsync(58);
                        var wordCTask = _modbusService.ReadWordAsync(60);
                        var wordDTask = _modbusService.ReadWordAsync(62);
                        var wordETask = _modbusService.ReadWordAsync(67);
                        var wordFTask = _modbusService.ReadWordAsync(70);
                        var wordGTask = _modbusService.ReadWordAsync(72);
                        // Đợi tất cả các tác vụ đọc xong
                        await Task.WhenAll(wordATask, wordBTask, wordCTask);

                        // Cập nhật các giá trị sau khi hoàn thành đọc
                        WorMedicineA = wordATask.Result;
                        WorMedicineB = wordBTask.Result;
                        WorMedicineC = wordCTask.Result;
                        WorMedicineD = wordDTask.Result;
                        WorMedicineE = wordETask.Result;
                        WorMedicineF = wordFTask.Result;
                        WorMedicineG = wordGTask.Result;

                        // Tạo danh sách hiện tại
                        var currentQuantities = new Dictionary<string, ushort>
                        {
                            { "MedicineA", WorMedicineA },
                            { "MedicineB", WorMedicineB },
                            { "MedicineC", WorMedicineC },
                            { "MedicineD", WorMedicineD },
                            { "MedicineE", WorMedicineE },
                            { "MedicineF", WorMedicineF },
                            { "MedicineG", WorMedicineG },
                        };

                        foreach (var (itemType, quantity) in currentQuantities)
                        {
                            if (!lastQuantities.ContainsKey(itemType) || lastQuantities[itemType] != quantity)
                            {
                                // Nếu số lượng thay đổi và lớn hơn 0 thì ghi nhận
                                if (quantity > 0)
                                {
                                    var import_event = new Models.WarehouseEvent
                                    {
                                        Date = DateTime.Now.ToString("dd/MM/yy"),
                                        Time = DateTime.Now.ToString("HH:mm:ss"),
                                        EventType = "Nhập kho",
                                        ItemType = itemType,
                                        Quantity = 1
                                    };

                                    // Lưu vào database
                                    var databaseService = new DatabaseService();
                                    await databaseService.SaveEventAsync(import_event);

                                    Debug.WriteLine($"Đã lưu nhập kho: {itemType} - {quantity}");
                                }

                                // Cập nhật trạng thái cũ
                                lastQuantities[itemType] = quantity;
                            }
                        }

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
