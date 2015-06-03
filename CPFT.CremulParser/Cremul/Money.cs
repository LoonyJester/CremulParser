namespace CPFT.CremulParser.Cremul
{
    public class Money
    {
        private const string DefaultCurrency = "NOK";
        public decimal amount { get; set; }
        public string currency { get; set; }

        public Money(string moneySegment)
        {
            var m = moneySegment.Split(':');
            var a = m[1].Replace(',', '.'); // # , is used as decimal mark
            amount = decimal.Parse(a);
            currency = m.Length >= 3 ? m[2] : DefaultCurrency;
        }
    }
}