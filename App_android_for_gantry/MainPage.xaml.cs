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
        }

        // Khai bao khoi tao thuoc tinh ModbusService, truy cap phuong thuc thong qua  _modbusService
        private ModbusService _modbusService = new ModbusService();
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
            _ =TryConnectModbusAsync();// Boxview ket noi, tu dong ket noi lai
            _ =_modbusService.StartConnectionMonitoringAsync(Connection); // Ham TryConnectModbusAsync() va StartConnectionMonitoringAsync, se duoc goi cung luc
        }
        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            // Ngắt kết nối Modbus nếu đang kết nối
            if (_modbusService != null && _modbusService.IsConnected)
            {
                await _modbusService.Disconnect();
            }
        }
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
        // Connection
        private async void Modbus_Tcp_System(object sender, EventArgs e)
        {
            isConnected = await _modbusService.ConnectPLCAsync();
            if (!isConnected)
            {
                isConnected = await _modbusService.ConnectPLCAsync();
            }
            Modbus_Tcp.BackgroundColor = isConnected ? Colors.Green : Colors.Red;

        }
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

        // Bang_tai
        private async void Bang_Tai_Man_System_Pressed(object sender, EventArgs e)
        {
            await _modbusService.WriteHoldingRegisterAsync(1, 24, 1); // Ghi giá trị 1 vào MW30
            Bang_Tai_Man.Background = Colors.Green;
        }

        private async void Bang_Tai_Man_System_Released(object sender, EventArgs e)
        {
            await _modbusService.WriteHoldingRegisterAsync(1, 24, 0); // Ghi giá trị 1 vào MW30
            Bang_Tai_Man.Background = Colors.LightGray;
        }

        // Vancum
        private async void Vacum_Man_System(object sender, EventArgs e)
        {
            Vacum_Man.Background = Colors.Green;
            await _modbusService.WriteHoldingRegisterAsync(1, 16, 1); // Ghi giá trị 1 vào MW16

        }

        // JOG_X
        private async void Jog_X_Down_System_Pressed(object sender, EventArgs e)
        {
            await _modbusService.WriteHoldingRegisterAsync(1, 30, 1); // Ghi giá trị 1 vào MW30
            Jog_X_Down.Background = Colors.Green;
        }

        private async void Jog_X_Down_System_Released(object sender, EventArgs e)
        {
            await _modbusService.WriteHoldingRegisterAsync(1, 30, 0); // Ghi giá trị 1 vào MW30
            Jog_X_Down.Background = Colors.LightGray;
        }

        private async void Jog_X_Up_System_Pressed(object sender, EventArgs e)
        {
            await _modbusService.WriteHoldingRegisterAsync(1, 35, 1); // Ghi giá trị 1 vào MW30
            Jog_X_Up.Background = Colors.Green;
        }

        private async void Jog_X_Up_System_Released(object sender, EventArgs e)
        {
            await _modbusService.WriteHoldingRegisterAsync(1, 35, 0); // Ghi giá trị 1 vào MW30
            Jog_X_Up.Background = Colors.LightGray;
        }

        // JOG_Y
        private async void Jog_Y_Right_System_Pressed(object sender, EventArgs e)
        {
            await _modbusService.WriteHoldingRegisterAsync(1, 20, 1); // Ghi giá trị 1 vào MW20
            Jog_Y_Right.Background = Colors.Green;
        }

        private async void Jog_Y_Right_System_Released(object sender, EventArgs e)
        {
            await _modbusService.WriteHoldingRegisterAsync(1, 20, 0); // Ghi giá trị 1 vào MW20
            Jog_Y_Right.Background = Colors.LightGray;
        }

        private async void Jog_Y_Left_System_Pressed(object sender, EventArgs e)
        {
            await _modbusService.WriteHoldingRegisterAsync(1, 25, 1); // Ghi giá trị 1 vào MW25
            Jog_Y_Left.Background = Colors.Green;
        }

        private async void Jog_Y_Left_System_Released(object sender, EventArgs e)
        {
            await _modbusService.WriteHoldingRegisterAsync(1, 25, 0); // Ghi giá trị 1 vào MW25
            Jog_Y_Left.Background = Colors.LightGray;
        }


        // JOG_Z
        private async void Jog_Z__Down_System_Pressed(object sender, EventArgs e)
        {
            await _modbusService.WriteHoldingRegisterAsync(1, 40, 1); // Ghi giá trị 1 vào MW30
            Jog_Z__Down.Background = Colors.Green;
        }

        private async void Jog_Z__Down_System_Released(object sender, EventArgs e)
        {
            await _modbusService.WriteHoldingRegisterAsync(1, 40, 0); // Ghi giá trị 1 vào MW30
            Jog_Z__Down.Background = Colors.LightGray;
        }

        private async void Jog_Z_Up_System_Pressed(object sender, EventArgs e)
        {
            await _modbusService.WriteHoldingRegisterAsync(1, 45, 1); // Ghi giá trị 1 vào MW30
            Jog_Z_Up.Background = Colors.Green;
        }

        private async void Jog_Z_Up_System_Released(object sender, EventArgs e)
        {
            await _modbusService.WriteHoldingRegisterAsync(1, 45, 0); // Ghi giá trị 1 vào MW30
            Jog_Z_Up.Background = Colors.LightGray;
        }
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }

}
