namespace NetBuild.App.Core.Blockchain
{
    public class BlockchainTransaction
    {
        public string Buyer { get; set; }
        public string Seller { get; set; }
        public double Amount { get; set; }
        public double Price { get; set; }
        public long Timestamp { get; set; }
    }
}
