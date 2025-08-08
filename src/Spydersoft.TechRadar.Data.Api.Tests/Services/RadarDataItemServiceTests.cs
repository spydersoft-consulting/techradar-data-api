using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Spydersoft.TechRadar.Data.Api.Data;
using Spydersoft.TechRadar.Data.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Spydersoft.TechRadar.Data.Api.Tests.Services;
public class RadarDataItemServiceTests
{
    private const string ExistingRadarName = "Existing Radar";
    private SqliteConnection _connection = null!;
    private DbContextOptions<TechRadarContext> _contextOptions = null!;
    private ClaimsPrincipal _claimsPrincipal = null!;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        // Create and open a connection. This creates the SQLite in-memory database, which will persist until the connection is closed
        // at the end of the test (see Dispose below).
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        // These options will be used by the context instances in this test suite, including the connection opened above.
        _contextOptions = new DbContextOptionsBuilder<TechRadarContext>()
            .UseSqlite(_connection)
            .Options;

        // Create the schema and seed some data
        using var context = new TechRadarContext(_contextOptions);

        if (context.Database.EnsureCreated())
        {
        }

        context.AddRange(
            new Radar { Title = ExistingRadarName, Description = "First Radar" });


        context.SaveChanges();

        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, "username"),
            new Claim(ClaimTypes.NameIdentifier, "userId"),
            new Claim("name", "John Doe"),
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        _claimsPrincipal = new ClaimsPrincipal(identity);
    }

    TechRadarContext CreateContext() => new TechRadarContext(_contextOptions);

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        // Close the connection to the in-memory database.
        _connection.Close();
        _connection.Dispose();
    }

    [Test]
    public async Task NewRadarItem_GetsQuadrantsAndArcs()
    {
        var newTitle = "Quadrant Create Test";
        using var context = CreateContext();
        var radarService = new RadarDataItemService(context);

        // Act
        radarService.SaveRadarDataItem(new Radar
        {
            Title = newTitle,
            Description = "This is a new radar",
        }, _claimsPrincipal);

        // Assert
        var radar = await context.Radars.FirstOrDefaultAsync(r => r.Title == newTitle);
        Assert.That(radar, Is.Not.Null);
        var quadrants = await context.Quadrants
            .Where(q => q.RadarId == radar.Id)
            .ToListAsync();
        
        Assert.That(quadrants, Is.Not.Null);
        Assert.That(quadrants.Count, Is.EqualTo(4), "Expected 4 quadrants to be created for a new radar item.");

        var arcs = await context.RadarArcs
            .Where(ra => ra.RadarId == radar.Id)
            .ToListAsync();

        Assert.That(arcs, Is.Not.Null);
        Assert.That(arcs.Count, Is.EqualTo(4), "Expected 4 arcs to be created for a new radar item.");
    }

    [Test]
    public async Task ExistingRadarItem_NoQuadrantsOrArcs()
    {
        using var context = CreateContext();
        var radarService = new RadarDataItemService(context);

        Radar radar = await context.Radars.FirstAsync(r => r.Title == ExistingRadarName);

        // Act
        radarService.SaveRadarDataItem(radar, _claimsPrincipal);

        // Assert
        var quadrants = await context.Quadrants
            .Where(q => q.RadarId == radar.Id)
            .ToListAsync();

        Assert.That(quadrants, Is.Not.Null);
        Assert.That(quadrants.Count, Is.EqualTo(0), "Expected 0 quadrants to be created for an existing radar item in the DB.");

        var arcs = await context.RadarArcs
            .Where(ra => ra.RadarId == radar.Id)
            .ToListAsync();

        Assert.That(arcs, Is.Not.Null);
        Assert.That(arcs.Count, Is.EqualTo(0), "Expected 0 arcs to be created for an existing radar item in the DB.");
    }

}
