
using UnlockOKR.OKRSupoort.Domain.ResponseModels;
using Xunit;

namespace UnlockOKR.OKRSupoort.Domain.Tests.ResponseModels
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
