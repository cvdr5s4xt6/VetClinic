﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class VetClinicaEntities2 : DbContext
    {
        public VetClinicaEntities2()
            : base("name=VetClinicaEntities2")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Animal> Animal { get; set; }
        public virtual DbSet<AnimalType> AnimalType { get; set; }
        public virtual DbSet<Appointment> Appointment { get; set; }
        public virtual DbSet<LabTest> LabTest { get; set; }
        public virtual DbSet<MedicalRecord> MedicalRecord { get; set; }
        public virtual DbSet<Owner> Owner { get; set; }
        public virtual DbSet<Specialty> Specialty { get; set; }
        public virtual DbSet<TestTypes> TestTypes { get; set; }
        public virtual DbSet<Veterenarian> Veterenarian { get; set; }
    }
}
