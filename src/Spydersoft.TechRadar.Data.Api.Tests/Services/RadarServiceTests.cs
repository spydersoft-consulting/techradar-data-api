using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Spydersoft.Platform.Telemetry;
using Spydersoft.TechRadar.Data.Api.Data;
using Spydersoft.TechRadar.Data.Api.Services;
using Spydersoft.TechRadar.Data.Api.Telemetry;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Spydersoft.TechRadar.Data.Api.Tests.Services;

public class RadarServiceTests
{
    private SqliteConnection _connection = null!;
    private DbContextOptions<TechRadarContext> _contextOptions = null!;
    private TechRadarTelemetryMetrics _telemetry = null!;
    private Mock<ITelemetryClient> _mockTelemetryClient = null!;

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
            new Radar { Title = "Radar1", Description = "First Radar" },
            new Radar { Title = "Radar2", Description = "Second Radar"});

        context.SaveChanges();
        
        // Initialize telemetry with mocked ITelemetryClient
        _mockTelemetryClient = new Mock<ITelemetryClient>();
        
        _telemetry = new TechRadarTelemetryMetrics(_mockTelemetryClient.Object);
    }

    private TechRadarContext CreateContext() => new(_contextOptions);

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        // Close the connection to the in-memory database.
        _connection.Close();
        _connection.Dispose();
    }

    [Test]
    public async Task GetRadarList_Succeeds()
    {
        using var context = CreateContext();
        var radarService = new RadarService(context, _telemetry);

        var radars = await radarService.GetRadarList();
        Assert.That(radars, Is.Not.Null);
        Assert.That(radars, Has.Count.EqualTo(2));
    }


}