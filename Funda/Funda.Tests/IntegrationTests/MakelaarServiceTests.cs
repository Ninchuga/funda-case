using FluentAssertions;
using Funda.Application.Services;
using Funda.DAL.Clients;
using Funda.DAL.Models;
using Funda.Domain.Models;
using Funda.Tests.Builders;
using Funda.Tests.Fixtures;
using Microsoft.Extensions.Logging;
using Moq;

namespace Funda.Tests.IntegrationTests
{
    public class MakelaarServiceTests(MakelaarServiceTestFixture fixture) : IClassFixture<MakelaarServiceTestFixture>
    {
        private readonly MakelaarServiceTestFixture _fixture = fixture;
        private readonly Mock<ILogger<MakelaarService>> _logger = new();
        private readonly Mock<IFundaObjectsClient> _mockedFundaObjectsClient = new();

        [Fact]
        public async Task GivenAmsterdamProperties_WhenErrorOccurs_ThenFailedResultShouldBeReturned()
        {
            string errorMessage = "An error occurred while fetching data from the Funda API.";

            _mockedFundaObjectsClient.Setup(x => x.GetMakellarsDataFromObjects(
                    It.IsAny<GetMakelaarsDataFromObjectsRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<GetMakelaarsDataFromObjectsResponse>.Failure("An error occurred while fetching data from the Funda API."));

            var makelarService = new MakelaarService(_mockedFundaObjectsClient.Object, _fixture.SetupFundaConfigMock(), _fixture.Cache, _logger.Object);

            var result = await makelarService.GetTopTenSellingMakelaarsFor(MakelaarServiceTestFixture.DefaultCity, propertiesWithGarden: false, new CancellationToken());

            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNullOrEmpty();
            result.Errors.Should().Contain(errorMessage);
        }

        [Fact]
        public async Task GivenAmsterdamProperties_WhenSuccessfulResponseIsReturned_ThenMakelaarsDataShouldBeReturned()
        {
            var makelaar1 = new MakelaarBuilder().WithId(1).WithName("Makelaar 1").Build();
            var makelaar2 = new MakelaarBuilder().WithId(2).WithName("Makelaar 2").Build();
            var makelaar3 = new MakelaarBuilder().WithId(3).WithName("Makelaar 3").Build();
            var makelaar4 = new MakelaarBuilder().WithId(4).WithName("Makelaar 4").Build();

            _mockedFundaObjectsClient.SetupSequence(client => client.GetMakellarsDataFromObjects(It.IsAny<GetMakelaarsDataFromObjectsRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<GetMakelaarsDataFromObjectsResponse>.Success(new GetMakelaarsDataFromObjectsResponse([makelaar1, makelaar2, makelaar2, makelaar3, makelaar4], 2, 10)))
                .ReturnsAsync(Result<GetMakelaarsDataFromObjectsResponse>.Success(new GetMakelaarsDataFromObjectsResponse([makelaar3, makelaar3, makelaar4, makelaar4, makelaar4], 2, 10)));

            var makelarService = new MakelaarService(_mockedFundaObjectsClient.Object, _fixture.SetupFundaConfigMock(), _fixture.Cache, _logger.Object);
            
            var result = await makelarService.GetTopTenSellingMakelaarsFor(MakelaarServiceTestFixture.DefaultCity, propertiesWithGarden: false, new CancellationToken());

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().HaveCount(4);
            var topSellingMakelaar = result.Data.First();
            topSellingMakelaar.Name.Should().Be(makelaar4.Name);
            topSellingMakelaar.NumberOfPropertiesForSale.Should().Be(4);
            var lastSellingMakelaar = result.Data.Last();
            lastSellingMakelaar.Name.Should().Be(makelaar1.Name);
            lastSellingMakelaar.NumberOfPropertiesForSale.Should().Be(1);
        }

        [Fact]
        public async Task GivenAmsterdamProperties_WhenSuccessfulResponseIsReturnedAndCached_ThenMakelaarsDataShouldBeReturnedFromCache()
        {
            var makelaar1 = new MakelaarBuilder().WithId(1).WithName("Makelaar 1").Build();
            var makelaar2 = new MakelaarBuilder().WithId(2).WithName("Makelaar 2").Build();
            string cacheKey = $"TopTenSellingMakelaars_{MakelaarServiceTestFixture.DefaultCity}_garden:{false}";
            await _fixture.SetCacheKey<List<Makelaar>>(cacheKey, [makelaar1, makelaar2]);

            var makelarService = new MakelaarService(_mockedFundaObjectsClient.Object, _fixture.SetupFundaConfigMock(), _fixture.Cache, _logger.Object);

            var result = await makelarService.GetTopTenSellingMakelaarsFor(MakelaarServiceTestFixture.DefaultCity, propertiesWithGarden: false, new CancellationToken());

            await _fixture.RemoveCacheKey(cacheKey);

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().HaveCount(2);
        }
    }
}
