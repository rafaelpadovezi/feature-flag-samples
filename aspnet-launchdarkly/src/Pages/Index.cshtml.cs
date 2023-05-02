using LaunchDarkly.Sdk;
using LaunchDarkly.Sdk.Server;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Flaggy.Pages;

public class IndexModel : PageModel
{
    private readonly ApplicationContext _context;
    private static readonly Random GetRandom = new();

    public IndexModel(ApplicationContext context, LdClient client)
    {
        _context = context;

        var randomUserId = GetRandom.Next(1, 4);
        var ldContext = Context.Builder(FeatureFlags.CreateContextKey(randomUserId.ToString()))
            .Build();
        ButtonSchemeValue = client.StringVariation(
            FeatureFlags.ButtonSchemeValue,
            ldContext,
            "btn-dark");
    }

    public IList<Post> Post { get;set; } = default!;
    public string ButtonSchemeValue { get; }

    public async Task OnGetAsync()
    {
        Post = await _context.Posts
            .Include(p => p.Blog).ToListAsync();
    }
}
