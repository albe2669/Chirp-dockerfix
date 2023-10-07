namespace Chirp.Razor.Tests;
using AngleSharp;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class IntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _fixture;
    private readonly HttpClient _client;

    public IntegrationTest(WebApplicationFactory<Program> fixture)
    {
        _fixture = fixture;
        _client = _fixture.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true, HandleCookies = true });
    }

    [Fact]
    public async void CanSeePublicTimeline()
    {
        var response = await _client.GetAsync("/public");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content);
        Assert.Contains("public's Timeline", content);
    }

    [Theory]
    [InlineData("Helge")]
    [InlineData("Rasmus")]
    [InlineData("Jacqualine Gilcoine")]
    [InlineData("Roger Histand")]
    [InlineData("Luanna Muro")]
    public async void CanSeePrivateTimeline(string author)
    {
        var response = await _client.GetAsync($"/{author}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content);
        Assert.Contains($"{author}'s Timeline", content);
    }

    [Theory]
    [InlineData("Vobiscum")]
    [InlineData("Ad Astra")]
    public async void CheckIfAuthorHasNoCheeps(string author)
    {
        // if cheep doesn't occur in potential auther name
        var response = await _client.GetAsync($"/{author}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("There are no cheeps so far.", content);
    }

    // is the root page the same as page 1?
    [Fact]
    public async void CheckIfTheRootPageTheSameAsPageOne()
    {
        var response0 = await _client.GetAsync(@"/{?page=0}");
        response0.EnsureSuccessStatusCode();
        var response1 = await _client.GetAsync(@"/{?page=1}");
        response1.EnsureSuccessStatusCode();

        var contentPage0 = await response0.Content.ReadAsStringAsync();
        var contentPage1 = await response1.Content.ReadAsStringAsync();

        Assert.Equal(contentPage0, contentPage1);
    }

    [Theory]
    [InlineData("?page=1")]
    [InlineData("?page=2")]
    [InlineData("?page=3")]
    [InlineData("?page=4")]
    [InlineData("?page=5")]
    [InlineData("?page=6")]
    [InlineData("?page=7")]
    [InlineData("?page=8")]
    [InlineData("?page=9")]
    [InlineData("?page=10")]
    [InlineData("?page=20")]
    public async void CheckIfThereThirtyTwoCheepsPerPage(string page)
    {
        var response = await _client.GetAsync($"/{page}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        var context = BrowsingContext.New(Configuration.Default);
        var document = await context.OpenAsync(req => req.Content(content));
        var listItems = document.QuerySelectorAll("ul#messagelist li");

        //note: if the dump.sql isnt set up it'll be equal to only 2 cheeps on the first page
        Assert.Equal(32, listItems.Length);
    }

}