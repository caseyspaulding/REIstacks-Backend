namespace REIstacks.Application.Contracts.Responses;
public class BlogPostResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
    public string Slug { get; set; }
    public string ImageUrl { get; set; }
    public string OrganizationId { get; set; }
    public bool IsMainSiteBlog { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string Author { get; set; }
}
