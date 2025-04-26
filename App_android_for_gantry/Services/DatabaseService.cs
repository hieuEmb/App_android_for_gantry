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

        public DatabaseService()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "warehouse.db3");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<WarehouseEvent>().Wait();
        }

        public Task<int> SaveEventAsync(WarehouseEvent eventItem)
        {
            return _database.InsertAsync(eventItem);
        }

        public Task<List<WarehouseEvent>> GetEventsAsync()
        {
            return _database.Table<WarehouseEvent>().OrderByDescending(e => e.Id).ToListAsync();
        }
    }
}
