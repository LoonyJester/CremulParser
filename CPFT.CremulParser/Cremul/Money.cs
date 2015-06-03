using System.Globalization;

namespace CPFT.CremulParser.Cremul
{
    public class Money
    {
        private const string DefaultCurrency = "NOK";
        public decimal Amount { get; set; }
        public string Currency { get; set; }

        public Money(string moneySegment)
        {
            var m = moneySegment.Split(':');
            var a = m[1].Replace(',', '.'); // # , is used as decimal mark
            Amount = decimal.Parse(a, CultureInfo.InvariantCulture);
            Currency = m.Length >= 3 ? m[2] : DefaultCurrency;
        }
    }
}