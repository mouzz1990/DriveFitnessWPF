﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DriveFitnessLibrary
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DrivefitnessContext : DbContext
    {
        public DrivefitnessContext()
            : base("name=DrivefitnessContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Attendance> Attendance { get; set; }
        public virtual DbSet<Client> Client { get; set; }
        public virtual DbSet<Group> Group { get; set; }
        public virtual DbSet<Schedule> Schedule { get; set; }
        public virtual DbSet<Subscription> Subscription { get; set; }
    }
}
