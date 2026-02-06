
using OneGuru.CFR.Domain.ResponseModels;
using Xunit;

namespace OneGuru.CFR.Domain.Tests.ResponseModels
{
    public class BlobVaultResponseTest : BaseTest
    {
        [Fact]
        public void BlobVaultResponse_Success()
        {
            var model = new BlobVaultResponse();
            var resultGet = GetModelTestData(model);
            var resultSet = SetModelTestData(model);
            Assert.NotNull(resultGet);
            Assert.NotNull(resultSet);
        }
    }
}
