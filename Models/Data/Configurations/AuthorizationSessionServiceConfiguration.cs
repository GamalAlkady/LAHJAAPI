using LAHJAAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Models.Data.Configurations
{
    internal sealed class AuthorizationSessionServiceConfiguration : IEntityTypeConfiguration<AuthorizationSessionService>
    {
        public void Configure(EntityTypeBuilder<AuthorizationSessionService> builder)
        {
            builder.HasKey(sc => new { sc.AuthorizationSessionId, sc.ServiceId });

            builder
                .HasOne(s => s.AuthorizationSession)
                .WithMany(c => c.AuthorizationSessionServices)
                .HasForeignKey(c => c.AuthorizationSessionId);

            builder
                .HasOne(s => s.Service)
                .WithMany(c => c.AuthorizationSessionServices)
                .HasForeignKey(c => c.ServiceId);

            builder.Navigation(e => e.Service).AutoInclude();
        }
    }
}
