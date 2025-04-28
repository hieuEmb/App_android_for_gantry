using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace App_android_for_gantry.Models
{
    public class WarehouseEvent
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string EventType { get; set; }
        public string ItemType { get; set; }  // "Loại A", "Loại B", ...
        public int Quantity { get; set; }     // số lượng người dùng nhập
    }
}
