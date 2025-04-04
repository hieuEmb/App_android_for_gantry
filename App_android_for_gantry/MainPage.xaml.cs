using Microsoft.Maui.Networking;
using System.ComponentModel;
using System.Diagnostics;
using App_android_for_gantry.ViewModels; // goi App_android_for_gantry.ViewModels de dung cac class cua MainViewModel.cs
using App_android_for_gantry.Services;// goi App_android_for_gantry.Services de dung cac class cua ModbusService.cs
namespace App_android_for_gantry

{
    public partial class MainPage : ContentPage // Ham main mac dinh cua .netmaui, tien hanh viet cac ham con o phia duoi.
    {

        public MainPage()// Day la constructor cua MainPage, khi MainPage duoc khoi tao.
        {
            InitializeComponent();// Khoi tao va tai cac giao dien trong MainPage.xaml, tom lai la hien thi UI
            BindingContext = ViewModel;// Thiet lap Bindingcontext cua MainPage giup trang ket noi du lieu tu ViewModel va Cap nhat giao dien UI  tu dong
            //Connectivity.ConnectivityChanged += OnConnectivityChanged;
        }

        // Khai bao khoi tao thuoc tinh ModbusService, truy cap phuong thuc thong qua  _modbusService
        private ModbusService _modbusService= new ModbusService();
        // Khai bao  khoi tao thuoc tinh MainViewModel, truy cap phuong thuc thong qua ViewModel
        public MainViewModel ViewModel { get; set; } = new MainViewModel();
        private bool isConnected = false;// Xu ly nut nhan connection
        //public double RealPosX { get; set; }
       

/// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Tu ket noi khi mo App
        protected override async void OnAppearing()
        {          
            base.OnAppearing();
            await _modbusService.ConnectPLCAsync();// Boxview HOMEx, Homey, HomeZ
            _ = _modbusService.StartConnectionMonitoringAsync(Connection); // Ham TryConnectModbusAsync() va StartConnectionMonitoringAsync, se duoc goi cung luc
            _ =TryConnectModbusAsync();// Boxview ket noi, tu dong ket noi lai
            ViewModel.StartReadingPositions();
        }
        //protected override async void OnDisappearing()
        //{
        //    base.OnDisappearing();
        //    // Ngắt kết nối Modbus nếu đang kết nối
        //    if (_modbusService != null && _modbusService.IsConnected)
        //    {
        //        await _modbusService.Disconnect();
        //    }
        //}
        //// Xu ly mang
        //private async void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        //{
        //    if (e.NetworkAccess == NetworkAccess.Internet)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"value:");
        //        //await _modbusService.ConnectPLCAsync();
        //        _ = TryConnectModbusAsync();
        //        ViewModel.StartReadingPositions();// Doc Pos sau khi co mang tro lai

        //    }
        //    else
        //    {
        //        Console.WriteLine("🚨 Mất kết nối mạng, không thể kết nối với PLC.");
        //    }
        //}

        /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Read 1 bit  registerhoding
        private async Task TryConnectModbusAsync()
        {
            System.Diagnostics.Debug.WriteLine($"IsConnected_1: {_modbusService.IsConnected}");
            if (_modbusService.IsConnected) // Kiểm tra kết nối trực tiếp
            {
                await Task.WhenAll(
                    _modbusService.StartReadingRegisterAsync(Home_X, 1, 13),
                    _modbusService.StartReadingRegisterAsync(Home_Y, 1, 14),
                    _modbusService.StartReadingRegisterAsync(Home_Z, 1, 15)
                );
            }
            else
            {
                Home_X.Color = Colors.LightGray;
                Home_Y.Color = Colors.LightGray;
                Home_Z.Color = Colors.LightGray;
            }
        }
        /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //Write 16 bit registerhoding

        // Start
        private async void Start_System(object sender, EventArgs e)
        {
            Start.Background = Colors.Green;
            await _modbusService.WriteHoldingRegisterAsync(1, 0, 1); // Ghi giá trị 1 vào MW0
            await Task.Delay(500);
            await _modbusService.WriteHoldingRegisterAsync(1, 0, 0); // Reset về 0
            await Task.Delay(500);
            Stop.Background = Colors.LightGray;
        }

        // Stop
        private async void Stop_System(object sender, EventArgs e)
        {
            Stop.Background = Colors.Green;
            await _modbusService.WriteHoldingRegisterAsync(1, 1, 1); // Ghi giá trị 1 vào MW1
            await Task.Delay(500);
            await _modbusService.WriteHoldingRegisterAsync(1, 1, 0); // Reset về 0
            Start.Background = Colors.LightGray;
        }

        // Auto_Mode
        private async void Auto_Mode_System(object sender, EventArgs e)
        {
            Auto_Mode.Background = Colors.Green;
            await _modbusService.WriteHoldingRegisterAsync(1, 10, 1); // Ghi giá trị 1 vào MW10
            Man_Mode.Background = Colors.LightGray;
        }

        // Man_mode
        private async void Man_Mode_System(object sender, EventArgs e)
        {
            Man_Mode.Background = Colors.Green;
            await _modbusService.WriteHoldingRegisterAsync(1, 12, 1); // Ghi giá trị 1 vào MW10
            Auto_Mode.Background = Colors.LightGray;
        }

        // Import_Mode
        private async void Improt_Mode_System(object sender, EventArgs e)
        {
            Improt_Mode.Background = Colors.Green;
            await _modbusService.WriteHoldingRegisterAsync(1, 18, 1); // Ghi giá trị 1 vào MW10
            Export_Mode.Background = Colors.LightGray;
        }

        // Export_mode
        private async void Export_Mode_System(object sender, EventArgs e)
        {
            Export_Mode.Background = Colors.Green;
            await _modbusService.WriteHoldingRegisterAsync(1, 22, 1); // Ghi giá trị 1 vào MW10
            Improt_Mode.Background = Colors.LightGray;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }



    }
