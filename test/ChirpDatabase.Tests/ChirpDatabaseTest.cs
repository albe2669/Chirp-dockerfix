using Testcontainers.MsSql;
using Bogus;
using Chirp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.ChirpRepository;
using Chirp.Core;
using Chirp.Infrastructure.Models;

namespace ChirpIntegraiton.Tests;

public class ChirpDatabaseTest : IAsyncLifetime
{
    private readonly MsSqlContainer _sqlServer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    public Task InitializeAsync()
    {
        return _sqlServer.StartAsync();
    }
    public Task DisposeAsync()
    {
        return _sqlServer.DisposeAsync().AsTask();
    }

    private static ChirpDBContext SetupContext(string ConnectionString)
    {
        var contextOptions = new DbContextOptionsBuilder<ChirpDBContext>()
            .UseSqlServer(ConnectionString)
            .Options;
        var context = new ChirpDBContext(contextOptions);
        context.Database.Migrate();
        
        return context;
    }

    [Theory]
    [InlineData("Obi-Wan", "Hello there")]
    [InlineData("Grievous", "General Kenobi. A bold one i see")]
    public async void CreateValidCheepInDatabase_WhereAuthorExists(string authorName, string message)
    {   
        // Arrange
        var context = SetupContext(_sqlServer.GetConnectionString());

        var cheepService = new CheepRepository(context);
        var authorService = new AuthorRepository(context);
        var userService = new UserRepository(context);

        // Act
        await userService.CreateUser(authorName);
        await authorService.CreateAuthor( await userService.GetUserByName(authorName) );
        var cheep = new CheepCreateDTO(message, authorName);

        await cheepService.CreateCheep(cheep, await authorService.GetAuthorByName(authorName));
        
        var dbCheep = cheepService.GetAll();

        // Assert
        Assert.Equal(1, dbCheep.Item2);
        Assert.Equal(message, dbCheep.Item1.FirstOrDefault().Text);
    }

    [Fact]
    public async void Create100Cheeps_ReadMostResent32()
    {
        // Arrange
        var context = SetupContext(_sqlServer.GetConnectionString());

        var cheepService = new CheepRepository(context);
        var authorService = new AuthorRepository(context);
        var userService = new UserRepository(context);

        // Act
        for (int i = 0; i < 100; i++)
        {
            var authorName = "Test author " + i;
            await userService.CreateUser(authorName);
            await authorService.CreateAuthor( await userService.GetUserByName(authorName) );
            var cheep = new CheepCreateDTO("Test message for author " + i, authorName);
            await cheepService.CreateCheep(cheep, await authorService.GetAuthorByName(authorName));
        }

        // Assert
        var allCheeps = cheepService.GetAll();
        var cheeps = await cheepService.GetSome(0, 32);
        Assert.Equal(100, allCheeps.Item2); // All cheeps are created
        Assert.Equal(32, cheeps.Item1.Count); // Only getting 32 cheeps
        Assert.Equal("Test message for author 99", cheeps.Item1.FirstOrDefault().Message); // The cheeps gotten is the most resent 
    }

    [Theory]
    [InlineData("Obi-Wan", "obi-wan@jedi.com")]
    [InlineData("General Grievous", "xXjediSlayerXx@sith.co.uk")]
    public async void CreateUserWithEmail(string name, string email)
    {
        // Arrange
        var context = SetupContext(_sqlServer.GetConnectionString());
    
        var userService = new UserRepository(context);

        // Act
        await userService.CreateUser(name, email);
        
        // Assert
        var userByName = await userService.GetUserByName(name);
        Assert.Equal(name, userByName.Name);
        var userByEmail = await userService.GetUserByEmail(email);
        Assert.Equal(email, userByEmail.Email);
    }
public record Follows { 
    public required int FollowerId { get; set; }
    public required int FollowingId { get; set; }

    
}    
}
