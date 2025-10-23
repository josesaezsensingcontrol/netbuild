using NetBuild.App.Core.ApiModel.SignalR;
using NetBuild.App.Core.Electricity;

namespace NetBuild.App.Core.ApiModel.Responses.Data
{
    public class GetElectricityPricesResponse : Response<IEnumerable<TimestampedValue>>
    {
        public GetElectricityPricesResponse(ReeResponse reeResponse)
        {
            try
            {
                Data = reeResponse.Included[0]
                    .Attributes
                    .Values
                    .Select(attr =>
                        new TimestampedValue
                        {
                            Date = attr.Datetime.ToUnixTimeMilliseconds(),
                            Value = attr.Value / 1000
                        }
                    );
            }
            catch (Exception)
            {
                Data = new List<TimestampedValue>();
            }
        }
    }
}
