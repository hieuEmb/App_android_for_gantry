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
        public int ValueA { get; set; }
        public int ValueB { get; set; }
        public int ValueC { get; set; }
        public int ValueD { get; set; }
        public int ValueE { get; set; }
        public int ValueF { get; set; }
        public int ValueG { get; set; }
    }
}
