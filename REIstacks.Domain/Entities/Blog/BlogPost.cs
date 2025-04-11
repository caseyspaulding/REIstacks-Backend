
using REIstacks.Domain.Entities.Organizations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace REIstacks.Domain.Entities.Blog;

[Table("blog_posts")]
public class BlogPost
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; }

    [MaxLength(255)]
    public string Description { get; set; }

    [MaxLength(255)]
    public string ImageUrl { get; set; }

    public string Content { get; set; }

    [Required]
    [MaxLength(100)]
    public string Slug { get; set; }

    // Can be null for main site blog posts
    [JsonPropertyName("organizationId")]
    public string OrganizationId { get; set; }

    public bool IsMainSiteBlog { get; set; } = false;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public bool IsPublished { get; set; } = true;

    [MaxLength(50)]
    public string Author { get; set; }

    // Navigation properties
    [ForeignKey("OrganizationId")]
    public virtual Organization Organization { get; set; }
}