using FlaggyApi;
using LaunchDarkly.Sdk;
using LaunchDarkly.Sdk.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flaggy.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly ApplicationContext _context;
    private readonly LdClient _ldClient;

    public PostsController(ApplicationContext context, LdClient ldClient)
    {
        _context = context;
        _ldClient = ldClient;
    }

    [HttpGet]
    public async Task<IActionResult> Get(int userId)
    {
        var users = new[]
        {
            new { Name = "User1" }, new { Name = "User2" }, new { Name = "User3" }, new { Name = "User4" },
        };

        var context = Context.Builder(FeatureFlags.CreateContextKey(userId.ToString()))
            .Set("user-id", userId)
            .Set("name", users[userId - 1].Name)
            .Build();

        var posts = await _context.Posts.AsNoTracking().ToListAsync();

        if (!_ldClient.BoolVariation(FeatureFlags.ShowTags, context))
            foreach (var post in posts)
                post.Tags = new List<Tag>();

        return Ok(posts);
    }
    
    [FeatureGate(FeatureFlags.AddPost)]
    [HttpPost("{blogId}:int")]
    public async Task<IActionResult> AddPost([FromRoute] int blogId, PostRequest newPost)
    {
        var post = new Post
        {
            Title = newPost.Title,
            Content = newPost.Content,
            Tags = newPost.Tags.Select(x => new Tag { Value = x }).ToList(),
            BlogId = blogId
        };
        _context.Add(post);
        await _context.SaveChangesAsync();
        return Ok();
    }

    public record PostRequest(string Title, string Content, string[] Tags);
}
