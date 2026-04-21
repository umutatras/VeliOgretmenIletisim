using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VeliOgretmenIletisim.Domain.Entities;

namespace VeliOgretmenIletisim.Infrastructure.Persistence.Configurations;

public class MeetingNoteConfiguration : IEntityTypeConfiguration<MeetingNote>
{
    public void Configure(EntityTypeBuilder<MeetingNote> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Teacher)
            .WithMany()
            .HasForeignKey(x => x.TeacherId)
            .OnDelete(DeleteBehavior.Restrict); // Break cycle

        builder.HasOne(x => x.Parent)
            .WithMany()
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict); // Break cycle
    }
}
