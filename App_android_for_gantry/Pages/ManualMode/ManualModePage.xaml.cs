namespace App_android_for_gantry.Pages.ManualMode;
using App_android_for_gantry.Services;
using App_android_for_gantry.ViewModels;
using Microsoft.Maui.Networking;
using System.ComponentModel;
using System.Diagnostics;

public partial class ManualModePage : ContentPage
{
	public ManualModePage()
	{
		InitializeComponent();
        BindingContext = ViewModel;
    }
    private ModbusService _modbusService = new ModbusService();
    public MainViewModel ViewModel { get; set; } = new MainViewModel();

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _modbusService.ConnectPLCAsync();// Boxview HOMEx, Homey, HomeZ
        _ = ReadCoilModbusAsync();
        _ = ReadInputsModbusAsync();
        ViewModel.StartReadingPositions();
    }
    // Bang_tai
    private async void Bang_Tai_Man_System_Pressed(object sender, EventArgs e)
    {
        await _modbusService.WriteHoldingRegisterAsync(1, 24, 1); // Ghi giá trị 1 vào MW30
        await _modbusService.WriteHoldingRegisterAsync(1, 74, 0);
        Bang_Tai_Man.Background = Colors.Green;
    }

    private async void Bang_Tai_Man_System_Released(object sender, EventArgs e)
    {
        await _modbusService.WriteHoldingRegisterAsync(1, 74, 1); // Ghi giá trị 1 vào MW30
        await _modbusService.WriteHoldingRegisterAsync(1, 24, 0);
        Bang_Tai_Man.Background = Colors.LightGray;
    }

    // Vancum
    private async void Vacum_Man_System_Pressed(object sender, EventArgs e)
    {
        await _modbusService.WriteHoldingRegisterAsync(1, 16, 1); // Ghi giá trị 1 vào MW16
        await _modbusService.WriteHoldingRegisterAsync(1, 76, 0);
        Vacum_Man.Background = Colors.Green;
    }

    private async void Vacum_Man_System_Released(object sender, EventArgs e)
    {
        await _modbusService.WriteHoldingRegisterAsync(1, 76, 1); // Ghi giá trị 1 vào MW16
        await _modbusService.WriteHoldingRegisterAsync(1, 16, 0);
        Vacum_Man.Background = Colors.LightGray;
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
        await _modbusService.WriteHoldingRegisterAsync(1, 40, 1); // Ghi giá trị 1 vào MW40
        Jog_Z__Down.Background = Colors.Green;
    }

    private async void Jog_Z__Down_System_Released(object sender, EventArgs e)
    {
        await _modbusService.WriteHoldingRegisterAsync(1, 40, 0); // Ghi giá trị 1 vào MW40
        Jog_Z__Down.Background = Colors.LightGray;
    }

    private async void Jog_Z_Up_System_Pressed(object sender, EventArgs e)
    {
        await _modbusService.WriteHoldingRegisterAsync(1, 45, 1); // Ghi giá trị 1 vào MW45
        Jog_Z_Up.Background = Colors.Green;
    }

    private async void Jog_Z_Up_System_Released(object sender, EventArgs e)
    {
        await _modbusService.WriteHoldingRegisterAsync(1, 45, 0); // Ghi giá trị 1 vào MW45
        Jog_Z_Up.Background = Colors.LightGray;
    }

    // Xử lý coil
    private async Task ReadCoilModbusAsync()
    {
        System.Diagnostics.Debug.WriteLine($"IsConnected_1: {_modbusService.IsConnected}");
        if (_modbusService.IsConnected) // Kiểm tra kết nối trực tiếp
        {
            await Task.WhenAll(
                _modbusService.StartReadingCoilAsync(Bang_tai, 1, 1),
                _modbusService.StartReadingCoilAsync(Van_cum, 1, 0)

            );
        }
        else
        {
            Bang_tai.Color = Colors.LightGray;
            Van_cum.Color = Colors.LightGray;
            //Home_Z.Color = Colors.LightGray;
        }
    }


    private async Task ReadInputsModbusAsync()
    {
        System.Diagnostics.Debug.WriteLine($"IsConnected_1: {_modbusService.IsConnected}");
        if (_modbusService.IsConnected) // Kiểm tra kết nối trực tiếp
        {
            await Task.WhenAll(
                _modbusService.StartReadingInputAsync(S1, 1, 0),
                _modbusService.StartReadingInputAsync(S2, 1, 1)

            );
        }
        else
        {
            S1.Color = Colors.LightGray;
            S2.Color = Colors.LightGray;
            //Home_Z.Color = Colors.LightGray;
        }
    }
}