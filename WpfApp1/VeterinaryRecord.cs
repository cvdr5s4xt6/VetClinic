using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class VeterinaryRecord
    {
        public int RecordID { get; set; }        // ID Записи
        public DateTime Date { get; set; }       // Дата
        public string OwnerName { get; set; }    // Владелец
        public string PetName { get; set; }      // Питомец
        public string Status { get; set; }       // Статус
    }
}
