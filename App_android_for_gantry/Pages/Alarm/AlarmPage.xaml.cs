using App_android_for_gantry.Services;
using App_android_for_gantry.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace App_android_for_gantry.Pages.Alarm;

public partial class AlarmPage : ContentPage, INotifyPropertyChanged
{
    private DatabaseService _databaseService = new DatabaseService();
    private ObservableCollection<WarehouseEvent> _events = new ObservableCollection<WarehouseEvent>();
    private WarehouseEvent _selectedEvent;

    public ObservableCollection<WarehouseEvent> Events
    {
        get => _events;
        set
        {
            _events = value;
            OnPropertyChanged();
        }
    }

    public WarehouseEvent SelectedEvent  // Property for the selected event
    {
        get => _selectedEvent;
        set
        {
            _selectedEvent = value;
            OnPropertyChanged();  // Notify property changed to update UI binding
        }
    }


    // When an item is selected from the ListView
    private void OnEventSelected(object sender, SelectedItemChangedEventArgs e)
    {
        // Save the selected event in the SelectedEvent property
        var selectedEvent = e.SelectedItem as WarehouseEvent;
        if (selectedEvent != null)
        {
            SelectedEvent = selectedEvent;  // Assign the selected event to the property
        }
    }

    public ICommand DeleteCommand { get; }

    public AlarmPage()
    {
        InitializeComponent();
        BindingContext = this;           
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        //await AddSampleData(); // 1
        await LoadEvents();// 2
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
            ItemType = "Loại A",
            Quantity = 2
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

    // Khi nhấn nút "Xóa sự kiện đã chọn"
    // When the "Delete Selected Event" button is clicked
    private async void OnDeleteEventClicked(object sender, EventArgs e)
    {
        if (SelectedEvent != null)
        {
            // Hiển thị hộp thoại xác nhận
            bool isConfirmed = await DisplayAlert("Confirm", "Are you sure you want to delete this event?", "Yes", "No");

            if (isConfirmed)
            {
                var rowsDeleted = await _databaseService.DeleteEventAsync(SelectedEvent);
                if (rowsDeleted > 0)
                {
                    await LoadEvents();
                    await DisplayAlert("Notification", "Event deleted successfully!", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "Unable to delete event.", "OK");
                }
            }
            // Nếu người dùng bấm "No" thì không làm gì cả
        }
        else
        {
            await DisplayAlert("Notification", "Please select an event to delete.", "OK");
        }
    }

    // Hàm xóa toàn bộ sự kiện
    private async void OnDeleteAllClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Xác nhận", "Bạn có chắc chắn muốn xóa TẤT CẢ sự kiện không?", "Xóa hết", "Hủy");

        if (confirm)
        {
            await _databaseService.DeleteAllEventsAsync();
            await LoadEvents(); // Làm mới lại danh sách hiển thị sau khi xóa
            await DisplayAlert("Thông báo", "Đã xóa toàn bộ sự kiện!", "OK");
        }
    }


}
