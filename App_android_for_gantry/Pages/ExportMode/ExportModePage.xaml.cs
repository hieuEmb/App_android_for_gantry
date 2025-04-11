using App_android_for_gantry.Services;
using App_android_for_gantry.ViewModels;
using Microsoft.Maui.Networking;
using System.ComponentModel;
using System.Diagnostics;
namespace App_android_for_gantry.Pages.ExportMode;

public partial class ExportModePage : ContentPage
{
	public ExportModePage()
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

    // Xac nhan
    private async void Write_System(object sender, EventArgs e)
    {
        // Kiem tra xem Entry có du lieu không
        if (ushort.TryParse(entryValue_A.Text, out ushort value))
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"value: {value}");
                // Ghi giá tr? vào thanh ghi Modbus
                await _modbusService.WriteHoldingRegisterAsync(1, 191, value);
                await DisplayAlert("Thành công", $" Xu?t {value} s?n ph?m", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("L?i", $"Xu?t s?n ph?m th?t b?i: {ex.Message}", "OK");
            }
        }
        else
        {
            await DisplayAlert("L?i", "Vui lòng s? l??ng s?n ph?m h?p l?!", "OK");
        }
    }
}