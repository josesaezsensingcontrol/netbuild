namespace NetBuild.App.Core.ApiModel.Requests.Blockchain
{
    public class PublishAskRequest
    {
        public string User { get; set; }
        public double SellAmount { get; set; }
        public double SellPrice { get; set; }
    }
}
