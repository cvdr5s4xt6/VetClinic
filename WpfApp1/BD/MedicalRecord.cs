//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WpfApp1.BD
{
    using System;
    using System.Collections.Generic;
    
    public partial class MedicalRecord
    {
        public int record_id { get; set; }
        public int animal_id { get; set; }
        public int veterenarian_id { get; set; }
        public string diagnosis { get; set; }
        public string treatment { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
    
        public virtual Animal Animal { get; set; }
        public virtual Veterenarian Veterenarian { get; set; }
    }
}
