namespace App_android_for_gantry.Pages.ImportMode;
using App_android_for_gantry.ViewModels;
using Microsoft.Maui.Networking;
using System.ComponentModel;
using System.Diagnostics;
using App_android_for_gantry.Services;
public partial class ImportModePage : ContentPage
{
	public ImportModePage()
	{
		InitializeComponent();
        BindingContext = ViewModel;
    }
    private ModbusService _modbusService = new ModbusService();
    public MainViewModel ViewModel { get; set; } = new MainViewModel();

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _modbusService.ConnectPLCAsync();
        ViewModel.StartReadingMedicine();
    }

    //// Import_Mode
    //private async void Import_System(object sender, EventArgs e)
    //{
    //    await _modbusService.WriteHoldingRegisterAsync(1, 18, 1); // Ghi giá trị 1 vào MW10
    //    await Task.Delay(100);
    //    await _modbusService.WriteHoldingRegisterAsync(1, 18, 0);
    //}

    // Dieu huong trang Home
    private async void Back_Home_System(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }

    // Stop
    private async void Stop_System(object sender, EventArgs e)
    {
        Stop.Background = Colors.Green;
        await _modbusService.WriteHoldingRegisterAsync(1, 1, 1); // Ghi giá trị 1 vào MW1
        await _modbusService.WriteHoldingRegisterAsync(1, 0, 0);
        await Task.Delay(100);
        await _modbusService.WriteHoldingRegisterAsync(1, 1, 0); // Reset về 0
        Stop.Background = Colors.WhiteSmoke;
    }

}