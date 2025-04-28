using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using App_android_for_gantry.Models;

namespace App_android_for_gantry.Services
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseService()// Contructor tự động gọi
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "warehouse.db3"); //Tạo đường dẫn đến fiel
            _database = new SQLiteAsyncConnection(dbPath); // Kết nối tới file đó
            _database.CreateTableAsync<WarehouseEvent>().Wait();// Tạo bảng
        }

        public Task<int> SaveEventAsync(WarehouseEvent eventItem)
        {
            return _database.InsertAsync(eventItem);// Lưu lại bảng ghi
        }

        public Task<List<WarehouseEvent>> GetEventsAsync()
        {
            return _database.Table<WarehouseEvent>().OrderByDescending(e => e.Id).ToListAsync();// Lấy bảng ghi ra
        }

        // Phương thức xóa sự kiện
        public Task<int> DeleteEventAsync(WarehouseEvent eventItem)
        {
            return _database.DeleteAsync(eventItem); // Xóa sự kiện khỏi cơ sở dữ liệu
        }

    }
}
