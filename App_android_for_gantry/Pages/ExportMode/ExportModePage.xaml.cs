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
        try
        {
            var entries = new List<(Entry entry, ushort address)>
        {
            (entryValue_A, 112),
            (entryValue_B, 114),
            (entryValue_C, 116),
            (entryValue_D, 118),
            (entryValue_E, 120),
            (entryValue_F, 122),
            (entryValue_G, 124),
        };

            bool hasWritten = false;

            foreach (var (entry, address) in entries)
            {
                if (!string.IsNullOrWhiteSpace(entry.Text) && ushort.TryParse(entry.Text, out ushort value))
                {
                    await _modbusService.WriteHoldingRegisterAsync(1, address, value);
                    
                    System.Diagnostics.Debug.WriteLine($"Ghi {value} vào địa chỉ {address}");
                    hasWritten = true;
                }
            }

            if (hasWritten)
            {
                await _modbusService.WriteHoldingRegisterAsync(1, 126, 1); // Trigger chỉ khi có Entry hợp lệ
                await DisplayAlert("Thành công", "Đã ghi các giá trị đã nhập thành công!", "OK");
            }
            else
                await DisplayAlert("Thông báo", "Không có ô nào được nhập giá trị!", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Lỗi", $"Ghi dữ liệu thất bại: {ex.Message}", "OK");
        }
    }

}