using LAHJAAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Models.Data.Configurations
{
    internal sealed class PlanFeaturesConfiguration : IEntityTypeConfiguration<PlanFeature>
    {
        public void Configure(EntityTypeBuilder<PlanFeature> builder)
        {
            builder.HasKey(sc => new { sc.Key, sc.PlanId });

            builder
                .HasOne(s => s.Plan)
                .WithMany(c => c.PlanFeatures)
                .HasForeignKey(c => c.PlanId);


        }
    }
}
