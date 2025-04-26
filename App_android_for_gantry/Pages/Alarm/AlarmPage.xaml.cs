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
        System.Diagnostics.Debug.WriteLine($"BindingContext sau khi thiết lập: {BindingContext}");

        // Thêm dữ liệu giả và tải dữ liệu
        Device.BeginInvokeOnMainThread(async () =>
        {
            await Task.Delay(100); // Đợi giao diện tải xong
            await AddSampleData(); // Thêm dữ liệu giả vào SQLite
            await LoadEvents();    // Tải dữ liệu từ SQLite
        });
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // LoadEvents(); // Không cần gọi lại vì đã gọi trong constructor
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

    private async Task AddSampleData()
    {
        var sampleEvent = new WarehouseEvent
        {
            Date = "25/04/26",
            Time = "10:19:42",
            EventType = "Xuất kho",
            ValueA = 10,
            ValueB = 20,
            ValueC = 1,
            ValueD = 1,
            ValueE = 1,
            ValueF = 1,
            ValueG = 00
        };
        await _databaseService.SaveEventAsync(sampleEvent);
        System.Diagnostics.Debug.WriteLine("Đã thêm dữ liệu giả vào cơ sở dữ liệu.");
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}