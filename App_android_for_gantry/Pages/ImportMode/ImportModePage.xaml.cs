namespace App_android_for_gantry.Pages.ImportMode;
using App_android_for_gantry.Services;
public partial class ImportModePage : ContentPage
{
	public ImportModePage()
	{
		InitializeComponent();
	}
    private ModbusService _modbusService = new ModbusService();

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _modbusService.ConnectPLCAsync();     
    }

    // Write
    private async void Write_System(object sender, EventArgs e)
    {
        // Ki?m tra xem Entry c� d? li?u kh�ng
        if (ushort.TryParse(entryValue.Text, out ushort value))
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"value: {value}");
                // Ghi gi� tr? v�o thanh ghi Modbus
                await _modbusService.WriteHoldingRegisterAsync(1, 166, value);
                await DisplayAlert("Th�nh c�ng", $"?� ghi {value} v�o thanh ghi MW0", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("L?i", $"Kh�ng th? ghi d? li?u: {ex.Message}", "OK");
            }
        }
        else
        {
            await DisplayAlert("L?i", "Vui l�ng nh?p s? h?p l?!", "OK");
        }
    }
}