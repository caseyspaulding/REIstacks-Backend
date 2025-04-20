using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REIstacks.Application.Contracts.Requests;
using REIstacks.Application.Contracts.Responses;
using REIstacks.Application.Repositories.Interfaces;
using REIstacks.Domain.Entities.Blog;

namespace REIstacks.Api.Controllers.Blog;

[ApiController]
[Route("api/blog")]
public class BlogController : ControllerBase
{
    private readonly IBlogRepository _blogRepository;

    public BlogController(IBlogRepository blogRepository)
    {
        _blogRepository = blogRepository;
    }


    // CREATE an organization-scoped blog post
    // POST: api/blog/organization/{orgId}
    [HttpPost("organization/{orgId}")]
    [Authorize(Roles = "Owner,Admin")]
    public async Task<ActionResult<BlogPost>> CreateOrganizationPost(string orgId, [FromBody] BlogPost post)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            post.OrganizationId = orgId; // Tie this post to the specified org
            int id = await _blogRepository.AddPostAsync(post);

            // Return canonical route to retrieve this by slug
            return CreatedAtAction(
                nameof(GetOrganizationPostBySlug),
                new { orgId, slug = post.Slug },
                post
            );
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // UPDATE an organization-scoped blog post
    // PUT: api/blog/organization/{orgId}/{id}
    [HttpPut("organization/{orgId}/{id}")]
    [Authorize(Roles = "Owner,Admin")]
    public async Task<IActionResult> UpdateOrganizationPost(string orgId, int id, [FromBody] BlogPost post)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (id != post.Id)
            return BadRequest(new { error = "ID mismatch" });
        if (orgId != post.OrganizationId)
            return BadRequest(new { error = "Organization mismatch" });

        try
        {
            await _blogRepository.UpdatePostAsync(post);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // DELETE an organization-scoped blog post
    // DELETE: api/blog/organization/{orgId}/{id}
    [HttpDelete("organization/{orgId}/{id}")]
    [Authorize(Roles = "Owner,Admin")]
    public async Task<IActionResult> DeleteOrganizationPost(string orgId, int id)
    {
        try
        {
            var existing = await _blogRepository.GetPostByIdAsync(id);
            if (existing == null || existing.OrganizationId != orgId)
                return NotFound();

            await _blogRepository.DeletePostAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // (Optional) GET a specific post by slug for an organization
    // GET: api/blog/organization/{orgId}/{slug}
    // If you need to restrict read-access to Owner/Admin, add the attribute:
    [Authorize(Roles = "Owner,Admin")]
    [HttpGet("organization/{orgId}/{slug}")]
    public async Task<ActionResult<BlogPost>> GetOrganizationPostBySlug(string orgId, string slug)
    {
        var post = await _blogRepository.GetOrganizationPostBySlugAsync(orgId, slug);
        return post == null ? NotFound() : post;
    }

    // GET: api/blog
    [HttpGet]
    public async Task<ActionResult<List<BlogPost>>> GetAllPosts([FromQuery] bool includeUnpublished = false)
    {
        // Only allow administrators to see unpublished posts
        bool canSeeUnpublished = User.IsInRole("SuperAdmin");
        return await _blogRepository.GetAllPostsAsync(includeUnpublished && canSeeUnpublished);
    }

    // GET: api/blog/main
    [HttpGet("main")]
    public async Task<ActionResult<List<BlogPost>>> GetMainSitePosts([FromQuery] bool includeUnpublished = false)
    {
        bool canSeeUnpublished = User.IsInRole("SuperAdmin");
        return await _blogRepository.GetMainSiteBlogPostsAsync(includeUnpublished && canSeeUnpublished);
    }

    // GET: api/blog/organization/{orgId}
    [HttpGet("organization/{orgId}")]
    public async Task<ActionResult<List<BlogPost>>> GetOrganizationPosts(string orgId, [FromQuery] bool includeUnpublished = false)
    {
        bool canSeeUnpublished = User.IsInRole("SuperAdmin");
        return await _blogRepository.GetOrganizationBlogPostsAsync(orgId, includeUnpublished && canSeeUnpublished);
    }

    // GET: api/blog/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BlogPost>> GetPostById(int id)
    {
        var post = await _blogRepository.GetPostByIdAsync(id);
        if (post == null)
            return NotFound();

        // Only return unpublished posts to administrators
        if (!post.IsPublished && !User.IsInRole("SuperAdmin"))
            return NotFound();

        return post;
    }

    // GET: api/blog/main/my-post-slug
    [HttpGet("main/{slug}")]
    public async Task<ActionResult<BlogPost>> GetMainSitePostBySlug(string slug)
    {
        var post = await _blogRepository.GetMainSitePostBySlugAsync(slug);
        if (post == null)
            return NotFound();
        return post;
    }



    // GET: api/blog/my-post-slug (legacy support)
    [HttpGet("{slug}")]
    public async Task<ActionResult<BlogPost>> GetPostBySlug(string slug)
    {
        var post = await _blogRepository.GetPostBySlugAsync(slug);
        if (post == null)
            return NotFound();
        return post;
    }

    // POST: api/blog
    [HttpPost]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<BlogPostResponse>> CreatePost([FromBody] CreateBlogPostRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Create domain entity from request
        var post = new BlogPost
        {
            Title = request.Title,
            Description = request.Description,
            Content = request.Content,
            Slug = request.Slug,
            ImageUrl = request.ImageUrl ?? "",
            OrganizationId = request.OrganizationId,
            IsMainSiteBlog = request.IsMainSiteBlog,
            IsPublished = request.IsPublished,
            Author = request.Author,
            CreatedAt = DateTime.UtcNow
        };

        // For main site blogs, use the organization ID from claims if not provided
        if (post.IsMainSiteBlog && string.IsNullOrEmpty(post.OrganizationId))
        {
            var orgId = User.Claims.FirstOrDefault(c => c.Type == "organization_id")?.Value;
            post.OrganizationId = orgId;
        }

        try
        {
            int id = await _blogRepository.AddPostAsync(post);

            // Map domain entity to response
            var response = new BlogPostResponse
            {
                Id = post.Id,
                Title = post.Title,
                Description = post.Description,
                Content = post.Content,
                Slug = post.Slug,
                ImageUrl = post.ImageUrl,
                OrganizationId = post.OrganizationId,
                IsMainSiteBlog = post.IsMainSiteBlog,
                IsPublished = post.IsPublished,
                Author = post.Author,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt
            };

            return CreatedAtAction(nameof(GetPostById), new { id }, response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // PUT: api/blog/5
    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> UpdatePost(int id, [FromBody] BlogPost post)
    {
        if (id != post.Id)
            return BadRequest(new { error = "ID mismatch" });
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            await _blogRepository.UpdatePostAsync(post);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // DELETE: api/blog/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> DeletePost(int id)
    {
        try
        {
            await _blogRepository.DeletePostAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}