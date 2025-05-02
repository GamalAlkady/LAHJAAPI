using LAHJAAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();

        // ضع هنا الاتصال الصحيح بقاعدة البيانات
        optionsBuilder.UseSqlServer("Server=\tdb11439.public.databaseasp.net; Database=db11439; User Id=db11439; Password=Xf3?+Tk72_Bh; Encrypt=False; MultipleActiveResultSets=True;");

        return new DataContext(optionsBuilder.Options);
    }
}
