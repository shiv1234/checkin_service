using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace UnlockOKR.OKRSupoort.Domain.Common
{
    [ExcludeFromCodeCoverage]
    public static class AuthResources
    {
        public static ResourceSet GetResources(string cultureName)
        {
            ResourceManager resourceManager = new ResourceManager("UnlockOKR.OKRSupoort.Domain.Resources.AuthResource", Assembly.GetExecutingAssembly());
            ResourceSet resourceSet = resourceManager.GetResourceSet(CultureInfo.CreateSpecificCulture(cultureName), true, true);
            return resourceSet;
        }

        public static string GetResourceKeyValue(string keyName, string culture = "en-US")
        {
            var resultSet = GetResources(culture);
            return (resultSet != null && !string.IsNullOrEmpty(resultSet.GetString(keyName, true))) ? resultSet.GetString(keyName, true) : string.Empty;
        }
    }
}
