namespace NetBuild.App.Core.ApiModel.Requests.Blockchain
{
    public class PublishBidRequest
    {
        public string User { get; set; }
        public double BuyAmount { get; set; }
        public double BuyPrice { get; set; }
    }
}
