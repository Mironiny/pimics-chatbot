using System.Collections.Generic;
using System.Threading.Tasks;
using PimBot.Dto;

namespace PimBot.Repositories.Impl
{
    public class FeatureRepository : IFeaturesRepository
    {
        public async Task<IEnumerable<PimFeature>> GetAll()
        {
            var client = ODataClientSingleton.Get();
            var features = await client
                .For(Constants.Company).Key(Constants.CompanyName)
                .NavigateTo(Constants.FeaturesServiceEndpointName)
                .FindEntriesAsync();

            return MapFeatures(features);
        }

        private IEnumerable<PimFeature> MapFeatures(IEnumerable<IDictionary<string, object>> features)
        {
            List<PimFeature> pimFeatures = new List<PimFeature>();
            foreach (var feature in features)
            {
                var pimFeature = MapPimFeature(feature);
                pimFeatures.Add(pimFeature);
            }

            return pimFeatures;
        }

        private PimFeature MapPimFeature(IDictionary<string, object> keyword)
        {
            var pimFeature = new PimFeature();
            pimFeature.Source = (string)keyword["Source"];
            pimFeature.Code = (string)keyword["Code"];
            pimFeature.Group_System_Number = (string)keyword["Group_System_Number"];
            pimFeature.Source_Type = (string)keyword["Source_Type"];
            pimFeature.Source_Code = (string)keyword["Source_Code"];
            pimFeature.Line_No = (int)keyword["Line_No"];
            pimFeature.Line_Type = (string)keyword["Line_Type"];
            pimFeature.Number = (string)keyword["Number"];
            pimFeature.Description = (string)keyword["Description"];
            pimFeature.Value = (string)keyword["Value"];
            pimFeature.Unit_ID = (string)keyword["Unit_ID"];
            pimFeature.Values = (int)keyword["Values"];
            pimFeature.Possible_values = (int)keyword["Possible_values"];
            pimFeature.Field_Type = (string)keyword["Field_Type"];
            pimFeature.Usage_Type_Code = (string)keyword["Usage_Type_Code"];
            pimFeature.Feature_Type = (string)keyword["Feature_Type"];
            pimFeature.Not_inheritance_value = (bool)keyword["Not_inheritance_value"];
            pimFeature.Composed_value = (string)keyword["Composed_value"];
            pimFeature.Connection_Characteristics = (string)keyword["Connection_Characteristics"];
            pimFeature.Formatting = (string)keyword["Formatting"];
            pimFeature.Required = (bool)keyword["Required"];
            pimFeature.Search_Feature_ID = (string)keyword["Search_Feature_ID"];
            pimFeature.Feature_ID_Reference = (string)keyword["Feature_ID_Reference"];
            pimFeature.Document_ID = (string)keyword["Document_ID"];
            pimFeature.Field_Format = (string)keyword["Field_Format"];
            pimFeature.Alignment = (string)keyword["Alignment"];
            pimFeature.Output_Format_Features = (string)keyword["Output_Format_Features"];
            pimFeature.Print = (string)keyword["Print"];
            pimFeature.Web = (string)keyword["Web"];
            pimFeature.Description_Texts = (bool)keyword["Description_Texts"];
            pimFeature.Unit_Shorthand_Description = (string)keyword["Unit_Shorthand_Description"];
            pimFeature.Value_ID = (string)keyword["Value_ID"];
            pimFeature.Value_ID_Reference = (string)keyword["Value_ID_Reference"];
            pimFeature.Order = (int)keyword["Order"];
            pimFeature.ETag = (string)keyword["ETag"];
            return pimFeature;
        }
    }
}
