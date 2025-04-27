using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DbConfig
{
    internal class EmployeeEntityConfig : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");

            builder.Property(e => e.Name)
                   .HasMaxLength(20)
                   .IsRequired();

            builder.HasIndex(e => e.Name)
                   .HasDatabaseName("IX_Employees_Name");

            builder.HasIndex(e => e.Phone)
                   .HasDatabaseName("IX_Employees_Phone");
        }
    }
}
