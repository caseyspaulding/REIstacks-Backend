using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.Properties
{
    [Table("property_images")]
    public class PropertyImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PropertyId { get; set; }

        [Required, MaxLength(2048)]
        public string Url { get; set; }

        // Allows ordering in the gallery
        public int SortOrder { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(PropertyId))]
        public virtual Property Property { get; set; }
    }
}
