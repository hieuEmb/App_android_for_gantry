
using App_android_for_gantry.Services;
using Microsoft.Maui.Networking;
using System.ComponentModel;
using System.Diagnostics;
using App_android_for_gantry.ViewModels;
namespace App_android_for_gantry

{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            Connectivity.ConnectivityChanged += OnConnectivityChanged;
            BindingContext = ViewModel;

        }

        private ModbusService _modbusService = new ModbusService();
        private bool isConnected = false;
        //public double RealPosX { get; set; }
        public MainViewModel ViewModel { get; set; } = new MainViewModel();


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

        // Disconnection
        private async void Dis_Modbus_Tcp_System(object sender, EventArgs e)
        {
            await _modbusService.Disconnect();
            Modbus_Tcp.BackgroundColor = Colors.Red;
        }

        private async Task TryConnectModbusAsync()
        {
            if (!isConnected)
            {
                isConnected = await _modbusService.ConnectPLCAsync();
            }

            if (isConnected)
            {
                await _modbusService.StartReadingCoilAsync(Conveyor, 1, 1); // Đọc trạng thái coil
                Conveyor.Color = Colors.Green; // Màu xanh khi kết nối thành công
            }
            else
            {
                Conveyor.Color = Colors.LightGray; // Màu xám khi chưa kết nối được
            }
        }

        // XỬ LÝ KHI MẠNG THAY ĐỔI
        private async void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess == NetworkAccess.Internet && !isConnected)
            {
                await TryConnectModbusAsync();
            }
        }

        ~MainPage()
        {
            // Hủy đăng ký sự kiện khi trang bị đóng
            Connectivity.ConnectivityChanged -= OnConnectivityChanged;
        }


        // Test
        private async void Test_System_Pressed(object sender, EventArgs e)
        {
            
            await _modbusService.WriteHoldingRegisterAsync(1, 50, 1); // Ghi giá trị 1 vào MW0
            Test.Background = Colors.Green;
        }

        private async void Test_System_Released(object sender, EventArgs e)
        {           
            await _modbusService.WriteHoldingRegisterAsync(1, 50, 0); // Reset về 0
            Test.Background = Colors.LightGray;
        }


        
        // Start
        private async void Start_System(object sender, EventArgs e)
        {
            Start.Background = Colors.Green;
            Stop.Background = Colors.LightGray;
            await _modbusService.WriteHoldingRegisterAsync(1, 0, 1); // Ghi giá trị 1 vào MW0
            await Task.Delay(500);
            await _modbusService.WriteHoldingRegisterAsync(1, 0, 0); // Reset về 0
        }

        // Stop
        private async void Stop_System(object sender, EventArgs e)
        {
            await _modbusService.WriteHoldingRegisterAsync(1, 1, 1); // Ghi giá trị 1 vào MW1
            await Task.Delay(500);
            await _modbusService.WriteHoldingRegisterAsync(1, 1, 0); // Reset về 0
            Start.Background = Colors.LightGray;
            Stop.Background = Colors.Red;
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

    }

}
