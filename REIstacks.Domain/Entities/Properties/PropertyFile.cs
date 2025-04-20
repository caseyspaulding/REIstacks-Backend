using REIstacks.Domain.Entities.User;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.Properties;
[Table("property_files")]
public class PropertyFile
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int PropertyId { get; set; }

    [Required]
    [MaxLength(255)]
    public string FileName { get; set; }

    [Required]
    [MaxLength(2048)]
    public string FileUrl { get; set; }

    [MaxLength(50)]
    public string FileType { get; set; }

    public long FileSize { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;


    public Guid? UserId { get; set; }

    [ForeignKey("PropertyId")]
    public virtual Property Property { get; set; }

    [ForeignKey("UserId")]
    public virtual UserProfile UploadedBy { get; set; }



}
