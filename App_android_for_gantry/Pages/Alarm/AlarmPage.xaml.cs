using App_android_for_gantry.Services;
using App_android_for_gantry.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace App_android_for_gantry.Pages.Alarm;

public partial class AlarmPage : ContentPage, INotifyPropertyChanged
{
    private DatabaseService _databaseService = new DatabaseService();
    private ObservableCollection<WarehouseEvent> _events = new ObservableCollection<WarehouseEvent>();

    public ObservableCollection<WarehouseEvent> Events
    {
        get => _events;
        set
        {
            _events = value;
            OnPropertyChanged();
        }
    }
    public AlarmPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await AddSampleData();
        await LoadEvents();
    }
/// <summary>
/// ////////////////////////////////////////////////// Xử lý việc lưu lưu trữ, hiển thị dữ liệu
/// </summary>
/// <returns></returns>
    private async Task AddSampleData()
    {
        var sampleEvent = new WarehouseEvent
        {
            Date = DateTime.Now.ToString("dd/MM/yy"),
            Time = DateTime.Now.ToString("HH:mm:ss"),
            EventType = "Xuất kho",
            ItemType = "Loại C",
            Quantity = 5
        };
        await _databaseService.SaveEventAsync(sampleEvent);
        System.Diagnostics.Debug.WriteLine("Đã thêm dữ liệu giả vào cơ sở dữ liệu.");
    }

    private async Task LoadEvents()
    {
        Events.Clear(); // Xóa danh sách cũ
        var events = await _databaseService.GetEventsAsync();
        foreach (var evt in events)
        {
            Events.Add(evt); // Thêm các sự kiện mới vào danh sách
        }
        System.Diagnostics.Debug.WriteLine($"Số lượng sự kiện từ cơ sở dữ liệu: {Events.Count}");
    }
/// <summary>
/// ///////////////////////////////////////////////////////////////////////////////////////////////
/// </summary>

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Phương thức xóa sự kiện
    private async Task DeleteEvent(WarehouseEvent eventToDelete)
    {
        try
        {
            // Xóa sự kiện khỏi cơ sở dữ liệu
            await _databaseService.DeleteEventAsync(eventToDelete);

            // Cập nhật lại danh sách sự kiện
            await LoadEvents();
            System.Diagnostics.Debug.WriteLine("Đã xóa sự kiện thành công.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Lỗi khi xóa sự kiện: {ex.Message}");
        }
    }

    private async void OnDeleteEventClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var eventToDelete = button?.BindingContext as WarehouseEvent;

        if (eventToDelete != null)
        {
            // Xóa sự kiện
            await DeleteEvent(eventToDelete);
        }
    }
}
