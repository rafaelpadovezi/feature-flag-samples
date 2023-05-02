using Flaggy;
using LaunchDarkly.Sdk.Server;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRazorPages();

builder.Services.AddDbContext<ApplicationContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("ApplicationContext")));

var ldClient = AddLaunchDarklyClient(builder);

var app = builder.Build();

if (!ldClient.Initialized)
    app.Logger.LogError("LauchDarkly sdk client was not initialized! The app will run with default values");

app.Lifetime.ApplicationStopping.Register(() =>
{
    app.Logger.LogInformation("Disposing LaunchDarkly Client");
    ldClient.Dispose();
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();

LdClient AddLaunchDarklyClient(WebApplicationBuilder webApplicationBuilder)
{
    var launchDarklySdkKey = webApplicationBuilder.Configuration.GetValue<string>("LaunchDarkly:SdkKey");
    // The initial payload contains all the targeting rules for the environment associated with the provided SDK key.
    // If you target a large number of individual contexts, have a very large number of flags, or have large numbers
    // of extremely complicated rules, then this can increase the initial payload size. Configure any relevant timeout
    // options to anticipate a large initial load. This helps prevent the SDK from timing out.
    // https://docs.launchdarkly.com/sdk/concepts/getting-started#plan-for-a-large-initial-payload-from-the-streaming-endpoint
    var ldConfig = Configuration.Default(launchDarklySdkKey);
    var ldClient = new LdClient(ldConfig);
    // Your SDK should only initialize once statically, and that instance should be the exclusive instance you use
    // for a given LaunchDarkly project. When you create multiple SDK instances, previous instances may leak into
    // memory and stay active beyond their useful lifespan.
    // https://docs.launchdarkly.com/sdk/concepts/getting-started#implement-sdks-in-a-singleton-pattern
    webApplicationBuilder.Services.AddSingleton(_ => ldClient);
    return ldClient;
}
