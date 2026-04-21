using NUlid;

namespace VeliOgretmenIletisim.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Ulid.NewUlid().ToGuid();
}
