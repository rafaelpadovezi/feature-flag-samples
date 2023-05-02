using System.Net.Http.Json;
using Flaggy.Tests.Support;
using LaunchDarkly.Sdk;
using LaunchDarkly.Sdk.Server.Integrations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Flaggy.Tests;

public class PostsControllerTests : IClassFixture<TestWebApplicationFactory>, IDisposable
{
    
    private readonly WebApplicationFactory<Program> _factory;
    private readonly ApplicationContext _context;
    private readonly IDbContextTransaction _transaction;
    private readonly TestData _ldTestData;

    public PostsControllerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _context = _factory.Services.GetRequiredService<ApplicationContext>();
        _ldTestData = _factory.Services.GetRequiredService<TestData>();
        
        _context.Database.Migrate();
        _transaction = _context.Database.BeginTransaction();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_get_posts(bool showTags)
    {
        // Arrange
        var client = _factory.CreateClient();
        _ldTestData.Update(
            _ldTestData.Flag(FeatureFlags.ShowTags).ValueForAll(LdValue.Of(showTags)));
        _context.Blogs.Add(new Blog
        {
            Url = "www.myblog.com",
            Posts = new List<Post>
            {
                new()
                {
                    Title = "Title",
                    Content = "Content",
                    Tags = new List<Tag> { new() { Value = "Tag1" }, new() { Value = "Tag2" }, }
                }
            }
        });
        await _context.SaveChangesAsync();

        // Act
        var response = await client.GetAsync("posts?userId=1");

        // Assert
        Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<Post[]>();
        Assert.Collection(result, post =>
        {
            Assert.Equal("Title", post.Title);
            Assert.Equal("Content", post.Content);
            if (showTags)
                Assert.Collection(post.Tags,
                    tag => Assert.Equal("Tag1", tag),
                    tag => Assert.Equal("Tag2", tag));
            else
                Assert.Empty(post.Tags);
        });
    }

    public void Dispose()
    {
        if (_transaction == null) return;
        _transaction.Rollback();
        _transaction.Dispose();
    }
}
