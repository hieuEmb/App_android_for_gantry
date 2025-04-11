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

    // Write
    private async void Write_System(object sender, EventArgs e)
    {
        // Ki?m tra xem Entry có d? li?u không
        if (ushort.TryParse(entryValue_A.Text, out ushort value))
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"value: {value}");
                // Ghi giá tr? vào thanh ghi Modbus
                await _modbusService.WriteHoldingRegisterAsync(1, 191, value);
                await DisplayAlert("Thành công", $"?ã ghi {value} vào thanh ghi MW0", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("L?i", $"Không th? ghi d? li?u: {ex.Message}", "OK");
            }
        }
        else
        {
            await DisplayAlert("L?i", "Vui lòng nh?p s? h?p l?!", "OK");
        }
    }
}