using App_android_for_gantry.Services;
using App_android_for_gantry.ViewModels;
using App_android_for_gantry.Pages.Alarm;
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
    private DatabaseService _databaseService = new DatabaseService(); // Thêm DatabaseService
    
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
            var entries = new List<(Entry entry, ushort address, string itemType)>
        {
            (entryValue_A, 112, "Loại A"),
            (entryValue_B, 114, "Loại B"),
            (entryValue_C, 116, "Loại C"),
            (entryValue_D, 118, "Loại D"),
            (entryValue_E, 120, "Loại E"),
            (entryValue_F, 122, "Loại F"),
            (entryValue_G, 124, "Loại G"),
        };

            bool hasWritten = false;
            List<Models.WarehouseEvent> eventList = new List<Models.WarehouseEvent>();

            foreach (var (entry, address, itemType) in entries)
            {
                if (!string.IsNullOrWhiteSpace(entry.Text) && ushort.TryParse(entry.Text, out ushort value))
                {
                    await _modbusService.WriteHoldingRegisterAsync(1, address, value);
                    
                    System.Diagnostics.Debug.WriteLine($"Ghi {value} vào địa chỉ {address}");
                    hasWritten = true;


                    // Lưu vào database mỗi loại
                    var eventItem = new Models.WarehouseEvent
                    {
                        Date = DateTime.Now.ToString("dd/MM/yy"),
                        Time = DateTime.Now.ToString("HH:mm:ss"),
                        EventType = "Xuất kho",
                        ItemType = itemType,
                        Quantity = value
                    };
                    await _databaseService.SaveEventAsync(eventItem);

                }
            }

            if (hasWritten)
            {
                await _modbusService.WriteHoldingRegisterAsync(1, 126, 1); // Trigger chỉ khi có Entry hợp lệ
                await Task.Delay(300); // Giữ trạng thái 1 trong 300ms
                await _modbusService.WriteHoldingRegisterAsync(1, 126, 0); // Reset trigger về 0


                // Xóa các Entry sau khi ghi
                foreach (var (entry, _, _) in entries)
                {
                    entry.Text = string.Empty;
                }


                // thong bao

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

    //// Export_mode
    //private async void Export_System(object sender, EventArgs e)
    //{       
    //    await _modbusService.WriteHoldingRegisterAsync(1, 22, 1); // Ghi giá trị 1 vào MW10
    //    await Task.Delay(100);
    //    await _modbusService.WriteHoldingRegisterAsync(1, 22, 0);

    //}

    // Dieu huong trang Home
    private async void Back_Home_System(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }

    // Stop
    private async void Stop_System_Ex(object sender, EventArgs e)
    {
        Stop.Background = Colors.Green;
        await _modbusService.WriteHoldingRegisterAsync(1, 1, 1); // Ghi giá trị 1 vào MW1
        await _modbusService.WriteHoldingRegisterAsync(1, 0, 0);
        await Task.Delay(100);
        await _modbusService.WriteHoldingRegisterAsync(1, 1, 0); // Reset về 0
        Stop.Background = Colors.WhiteSmoke;
    }

}