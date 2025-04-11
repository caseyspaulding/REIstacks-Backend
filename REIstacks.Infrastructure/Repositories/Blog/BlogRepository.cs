using Microsoft.EntityFrameworkCore;
using REIstacks.Application.Repositories.Interfaces;
using REIstacks.Domain.Entities.Blog;
using REIstacks.Infrastructure.Data;


namespace REIstacks.Infrastructure.Repositories.Marketing
{
    public class BlogRepository : IBlogRepository
    {
        private readonly AppDbContext _context;

        public BlogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<BlogPost>> GetAllPostsAsync(bool includeUnpublished = false)
        {
            return await _context.BlogPosts
                .Where(p => includeUnpublished || p.IsPublished)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        // Get posts for main site
        public async Task<List<BlogPost>> GetMainSiteBlogPostsAsync(bool includeUnpublished = false)
        {
            return await _context.BlogPosts
                .Where(p => p.IsMainSiteBlog && (includeUnpublished || p.IsPublished))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        // Get posts for a specific organization
        public async Task<List<BlogPost>> GetOrganizationBlogPostsAsync(string organizationId, bool includeUnpublished = false)
        {
            return await _context.BlogPosts
                .Where(p => p.OrganizationId == organizationId && !p.IsMainSiteBlog && (includeUnpublished || p.IsPublished))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<BlogPost> GetPostByIdAsync(int id)
        {
            return await _context.BlogPosts
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<BlogPost> GetPostBySlugAsync(string slug)
        {
            return await _context.BlogPosts
                .FirstOrDefaultAsync(p => p.Slug == slug && p.IsPublished);
        }

        // Get post by slug for main site
        public async Task<BlogPost> GetMainSitePostBySlugAsync(string slug)
        {
            return await _context.BlogPosts
                .FirstOrDefaultAsync(p => p.Slug == slug && p.IsMainSiteBlog && p.IsPublished);
        }

        // Get post by slug for an organization
        public async Task<BlogPost> GetOrganizationPostBySlugAsync(string organizationId, string slug)
        {
            return await _context.BlogPosts
                .FirstOrDefaultAsync(p => p.Slug == slug && p.OrganizationId == organizationId && !p.IsMainSiteBlog && p.IsPublished);
        }

        public async Task<int> AddPostAsync(BlogPost post)
        {
            post.CreatedAt = DateTime.UtcNow;

            // Ensure slug is unique
            if (string.IsNullOrEmpty(post.Slug))
            {
                post.Slug = GenerateSlug(post.Title);
            }

            // Check if slug already exists within same scope (main site or org)
            var slugExists = post.IsMainSiteBlog
                ? await _context.BlogPosts.AnyAsync(p => p.Slug == post.Slug && p.IsMainSiteBlog)
                : await _context.BlogPosts.AnyAsync(p => p.Slug == post.Slug && p.OrganizationId == post.OrganizationId && !p.IsMainSiteBlog);

            if (slugExists)
            {
                post.Slug = $"{post.Slug}-{Guid.NewGuid().ToString().Substring(0, 8)}";
            }

            _context.BlogPosts.Add(post);
            await _context.SaveChangesAsync();
            return post.Id;
        }

        public async Task UpdatePostAsync(BlogPost post)
        {
            var existingPost = await _context.BlogPosts.FindAsync(post.Id);

            if (existingPost == null)
                throw new KeyNotFoundException($"Blog post with ID {post.Id} not found");

            // Update fields
            existingPost.Title = post.Title;
            existingPost.Description = post.Description;
            existingPost.Content = post.Content;
            existingPost.ImageUrl = post.ImageUrl;
            existingPost.IsPublished = post.IsPublished;
            existingPost.UpdatedAt = DateTime.UtcNow;
            existingPost.Author = post.Author;

            // Handle slug update if title changed
            if (post.Slug != existingPost.Slug)
            {
                // Check if new slug already exists within same scope (main site or org)
                var slugExists = existingPost.IsMainSiteBlog
                    ? await _context.BlogPosts.AnyAsync(p => p.Id != post.Id && p.Slug == post.Slug && p.IsMainSiteBlog)
                    : await _context.BlogPosts.AnyAsync(p => p.Id != post.Id && p.Slug == post.Slug && p.OrganizationId == existingPost.OrganizationId && !p.IsMainSiteBlog);

                if (slugExists)
                {
                    existingPost.Slug = $"{post.Slug}-{Guid.NewGuid().ToString().Substring(0, 8)}";
                }
                else
                {
                    existingPost.Slug = post.Slug;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeletePostAsync(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);

            if (post == null)
                throw new KeyNotFoundException($"Blog post with ID {id} not found");

            _context.BlogPosts.Remove(post);
            await _context.SaveChangesAsync();
        }

        // Helper method to generate a slug from a title
        private string GenerateSlug(string title)
        {
            if (string.IsNullOrEmpty(title))
                return Guid.NewGuid().ToString();

            // Convert to lowercase
            var slug = title.ToLowerInvariant();

            // Replace spaces with hyphens
            slug = slug.Replace(" ", "-");

            // Remove special characters
            slug = new string(slug
                .Where(c => char.IsLetterOrDigit(c) || c == '-')
                .ToArray());

            // Remove multiple hyphens
            while (slug.Contains("--"))
            {
                slug = slug.Replace("--", "-");
            }

            // Trim hyphens from beginning and end
            slug = slug.Trim('-');

            return slug;
        }
    }
}