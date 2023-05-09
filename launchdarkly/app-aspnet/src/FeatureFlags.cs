namespace Flaggy;

public class FeatureFlags
{
    public static string CreateContextKey(string key) => $"flaggy-api:{key}";
    public const string ButtonSchemeValue = "BUTTON_SCHEME_VALUE";
    public const string AddPost = "ADD_POST";
    public const string ShowTags = "SHOW_TAGS";
}
