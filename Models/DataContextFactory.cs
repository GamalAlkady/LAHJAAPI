using LAHJAAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();

        // ضع هنا الاتصال الصحيح بقاعدة البيانات
        optionsBuilder.UseSqlServer("Server=db19094.public.databaseasp.net; Database=db19094; User Id=db19094; Password=3Cr#i7-E6Kp!; Encrypt=True; TrustServerCertificate=True; MultipleActiveResultSets=True; ");

        return new DataContext(optionsBuilder.Options);
    }
}
