using SubastaYa.Domain.Enums;

namespace SubastaYa.Domain.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public AuditEntityType EntityType { get; set; }
        public string EntityId { get; set; } = default!;
        public string Action { get; set; } = default!;
        public Guid? UserId { get; set; }
        public string DetailsJson { get; set; } = default!;
        public DateTime OccurredAt { get; set; }
    }
}
