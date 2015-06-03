using System;
using System.Collections.Generic;

namespace CPFT.CremulParser.Cremul
{
    // Represents an individual payment transaction (SEQ segment) i a CREMUL message
    public class PaymentTx
    {
        //DTM+203:20130410:102
        //   Normal format:
        //   FII+<party-id>+<account identification>
        //  
        //   Emtpy format (no bank account info given):
        //   FII+<party-id>
        //  
        //   Party-id:
        //   OR - Payor's bank account
        //   I2 - Beneficiary's bank account


        public DateTime PostingDate { get; set; }
        public int TxIndex { get; set; }
        public string InvoiceRef { get; set; }
        public string PayerAccountNumber { get; set; }
        public RefTypes InvoiceRefType { get; set; }
        public string FreeText { get; set; }
        public Money Money { get; set; }
        public NameAndAddress BeneficiaryNad { get; set; }
        public NameAndAddress PayerNad { get; set; }
        public List<Reference> References { get; set; }




        public PaymentTx(int txIndex, string[] segments, int txSegmentPos, ParserHelper helper)
        {
            TxIndex = txIndex; // the index number of this tx within it's parent line
            var s = segments[helper.next_date_segment_index(segments, txSegmentPos, -1)].Split(':');
            PostingDate = helper.ParseCremulDate(s[1]);

            s = segments[helper.next_fii_or_segment_index(segments, txSegmentPos, -1)].Split('+');
            if (s.Length >= 3) // bank account number is provided)
            {
                if (s[2].Contains(":"))
                    PayerAccountNumber = s[2].Split(':')[0];
                        //  the part after the : is the payer account holder name
                else
                    PayerAccountNumber = s[2];
            }
            init_invoice_ref(segments, txSegmentPos, helper);
            init_free_text(segments, txSegmentPos, helper);

            Money = new Money(segments[helper.next_amount_segment_index(segments, txSegmentPos, -1)]);

            init_refs(segments, txSegmentPos, helper);
            init_name_and_addresses(segments, txSegmentPos, helper);


        }

        private void init_name_and_addresses(string[] segments, int txSegmentPos, ParserHelper helper)
        {
            var n = helper.last_segment_index_in_tx(segments, txSegmentPos);
            var nadIndex = helper.next_nad_segment_index(segments, txSegmentPos, -1);
            while (nadIndex >= 0 && nadIndex <= n)
            {
                var nad = new NameAndAddress(segments[nadIndex]);
                assign_nad(nad);
                nadIndex = helper.next_nad_segment_index(segments, nadIndex + 1, -1);
            }
        }

        //TODO: need refactor to enum
        private void assign_nad(NameAndAddress nad)
        {
            switch (nad.Type)
            {
                case "PL":
                    PayerNad = nad;
                    break;
                case "BE":
                    BeneficiaryNad = nad;
                    break;
            }
        }

        private void init_refs(string[] segments, int txSegmentPos, ParserHelper helper)
        {
            References = new List<Reference>();
            var n = helper.number_of_references_in_tx(segments, txSegmentPos);
            var refSegmentIndex = helper.next_ref_segment_index(segments, txSegmentPos, -1);
            for (int i = 0; i < n; i++)
            {
                References.Add(new Reference(segments[refSegmentIndex]));
                refSegmentIndex = helper.next_ref_segment_index(segments, refSegmentIndex + 1, -1);
            }
        }

        private void init_free_text(string[] segments, int txSegmentPos, ParserHelper helper)
        {
            var i = helper.payment_details_segment_index(segments, txSegmentPos, -1);
            if (i < 0) return;
            var s = segments[i].Split('+');
            FreeText = s[s.Length - 1];
        }

        private void init_invoice_ref(string[] segments, int txSegmentPos, ParserHelper helper)
        {
            var i = helper.doc_segment_index(segments, txSegmentPos, -1);
            if (i >= 0)
            {
                var s = segments[helper.doc_segment_index(segments, txSegmentPos, -1)].Split('+');
                InvoiceRefType = helper.ref_type(s[1]);
                if (s.Length > 2)
                    InvoiceRef = s[2];
            }
        }
    }
}