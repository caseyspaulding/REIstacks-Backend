using REIstacks.Domain.Models;

namespace REIstacks.Application.Repositories.Interfaces;
public interface IBlogRepository
{
    Task<int> AddPostAsync(BlogPost post);
    Task DeletePostAsync(int id);
    Task<List<BlogPost>> GetAllPostsAsync(bool includeUnpublished = false);
    Task<List<BlogPost>> GetMainSiteBlogPostsAsync(bool includeUnpublished = false);
    Task<BlogPost> GetMainSitePostBySlugAsync(string slug);
    Task<List<BlogPost>> GetOrganizationBlogPostsAsync(string organizationId, bool includeUnpublished = false);
    Task<BlogPost> GetOrganizationPostBySlugAsync(string organizationId, string slug);
    Task<BlogPost> GetPostByIdAsync(int id);
    Task<BlogPost> GetPostBySlugAsync(string slug);
    Task UpdatePostAsync(BlogPost post);
}