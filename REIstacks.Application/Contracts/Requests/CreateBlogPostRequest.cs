namespace REIstacks.Application.Contracts.Requests;
public class CreateBlogPostRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
    public string Slug { get; set; }
    public string ImageUrl { get; set; }
    public string OrganizationId { get; set; }
    public bool IsMainSiteBlog { get; set; }
    public bool IsPublished { get; set; }
    public string Author { get; set; }
}
