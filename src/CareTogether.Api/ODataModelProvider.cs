using CareTogether.Managers;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace CareTogether.Api
{
    public class ODataModelProvider
    {
        public static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<CombinedFamilyInfo>(nameof(CombinedFamilyInfo));
            return builder.GetEdmModel();
        }
    }
}
