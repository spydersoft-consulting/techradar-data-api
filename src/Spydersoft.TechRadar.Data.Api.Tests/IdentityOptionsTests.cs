using Spydersoft.TechRadar.Data.Api.Options;

namespace Spydersoft.TechRadar.Data.Api.Tests
{
    public class Tests
    {
        public class OptionsTests
        {
            [SetUp]
            public void Setup()
            {
            }

            [Test]
            public void IdentityOptionsDefaults()
            {
                var options = new IdentityOptions();
                Assert.Multiple(() =>
                {
                    Assert.That(options.Authority, Is.Null);
                    Assert.That(options.ApplicationName, Is.Null);
                });
            }

            [Test]
            public void IdentityOptionsScheme()
            {
                Assert.That(IdentityOptions.SectionName, Is.EqualTo("Identity"));
            }

            [Test]
            public void IdentityOptionsPropertyTest()
            {
                var options = new IdentityOptions
                {
                    Authority = "https://localhost:1234",
                    ApplicationName = "test-client-id"
                };
                Assert.Multiple(() =>
                {
                    Assert.That(options, Is.Not.Null);
                    Assert.That(options, Has.Property("Authority").TypeOf<string>());
                    Assert.That(options, Has.Property("ApplicationName").TypeOf<string>());
                });
            }
        }
    }
}