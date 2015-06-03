namespace CPFT.CremulParser.Cremul
{
//# Represents a reference (RFF) segment in a CREMUL message.
//# Type may be:
//# - :ACK - Bank reference - KID in Norwegian
//# - :AII - Bank’s reference number allocated by the bank to different underlying individual transactions
//# - :CT - AutoGiro agreement ID
//# - :ABO - Originator´s reference – for example SWIFT reference senders bank
//# - :ACD - Bank reference = Archive reference
//# - :AEK - Payment order number
//# - :AFO - Beneficiary’s reference
//# - :AGN - Payer’s reference, aka 'egenref' (NO)
//# - :AHK - Payer’s reference number, aka 'debetref' (NO)
//# - :RA - Remittance advice number
//# - :TBR - Reference number pre-advice
    public class Reference
    {
        public string type { get; set; }
        public string number { get; set; }

        ///  def initialize(ref_segment)
        ///    s = ref_segment.split(':')
        ///    @number = s[1]
        ///    s = s[0].split('+')
        ///    @type = s[1].to_sym
        ///  end
        public Reference(string refSegment)
        {
            var s = refSegment.Split(':');
            if (s.Length > 1)
                number = s[1];
            s = s[0].Split('+');
            type = s[1];
        }

    }
}