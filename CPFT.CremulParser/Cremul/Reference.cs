namespace CPFT.CremulParser.Cremul
{
// Represents a reference (RFF) segment in a CREMUL message.
// Type may be:
// - :ACK - Bank reference - KID in Norwegian
// - :AII - Bank’s reference number allocated by the bank to different underlying individual transactions
// - :CT - AutoGiro agreement ID
// - :ABO - Originator´s reference – for example SWIFT reference senders bank
// - :ACD - Bank reference = Archive reference
// - :AEK - Payment order number
// - :AFO - Beneficiary’s reference
// - :AGN - Payer’s reference, aka 'egenref' (NO)
// - :AHK - Payer’s reference number, aka 'debetref' (NO)
// - :RA - Remittance advice number
// - :TBR - Reference number pre-advice
    public class Reference
    {
        public string Type { get; set; }
        public string Number { get; set; }

        public Reference(string refSegment)
        {
            var s = refSegment.Split(':');
            if (s.Length > 1)
                Number = s[1];
            s = s[0].Split('+');
            Type = s[1];
        }

    }
}