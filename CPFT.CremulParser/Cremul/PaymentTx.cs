//require_relative 'parser_helper'
//require_relative 'cremul_reference'
//require_relative 'cremul_name_and_address'

using System;
using System.Collections.Generic;

namespace CPFT.CremulParser.Cremul
{
    //# Represents an individual payment transaction (SEQ segment) i a CREMUL message
    public class PaymentTx
    {
        //DTM+203:20130410:102
        //  # Normal format:
        //  # FII+<party-id>+<account identification>
        //  #
        //  # Emtpy format (no bank account info given):
        //  # FII+<party-id>
        //  #
        //  # Party-id:
        //  # OR - Payor's bank account
        //  # I2 - Beneficiary's bank account
        
        private const int REF_TYPE_KID = 999; // Norwegian customer invoice reference
        private const int REF_TYPE_INVOICE_NUMBER = 380;
        //
        //  attr_reader :posting_date, :money, :references, :invoice_ref_type, :invoice_ref, :free_text
        //  attr_reader :payer_account_number, :payer_nad, :beneficiary_nad, :tx_index
        public DateTime posting_date { get; set; }
        public int tx_index { get; set; }
        public string invoice_ref { get; set; }
        public string payer_account_number { get; set; }
        public ParserHelper.RefTypes invoice_ref_type { get; set; }
        public string free_text { get; set; }
        public Money money { get; set; }
        public NameAndAddress beneficiary_nad { get; set; }
        public NameAndAddress payer_nad { get; set; }



        public PaymentTx(int txIndex, string[] segments, int txSegmentPos, ParserHelper helper)
        {
            tx_index = txIndex; // the index number of this tx within it's parent line
            var s = segments[helper.next_date_segment_index(segments, txSegmentPos, -1)].Split(':');
            posting_date = DateTime.Parse(s[1]);

            s = segments[helper.next_fii_or_segment_index(segments, txSegmentPos, -1)].Split('+');
            if (s.Length >= 3) //# bank account number is provided)
            {
                if (s[2].Contains(":"))
                    payer_account_number = s[2].Split(':')[0]; // # the part after the : is the payer account holder name
                else
                    payer_account_number = s[2];
            }
            init_invoice_ref(segments, txSegmentPos, helper);
            init_free_text(segments, txSegmentPos, helper);

            money = new Money(segments[helper.next_amount_segment_index(segments, txSegmentPos, -1)]);

            init_refs(segments, txSegmentPos, helper);
            init_name_and_addresses(segments, txSegmentPos, helper);


        }

        private void init_name_and_addresses(string[] segments, int txSegmentPos, ParserHelper helper)
        {
            var n = helper.last_segment_index_in_tx(segments, txSegmentPos);
            var nadIndex = helper.next_nad_segment_index(segments, txSegmentPos, -1);
            while (nadIndex >= 0 && nadIndex <= n)
            {
              var  nad = new NameAndAddress(segments[nadIndex]);
                assign_nad(nad);
                nadIndex = helper.next_nad_segment_index(segments, nadIndex + 1, -1);
            }
        }
        //TODO: need refactor to enum
        private void assign_nad(NameAndAddress nad)
        {
            switch (nad.type)
            {
                case "PL":
                    payer_nad = nad;
                    break;
                case "BE":
                    beneficiary_nad = nad;
                    break;
            }
        }

        private void init_refs(string[] segments, int txSegmentPos, ParserHelper helper)
        {
            var references = new List<Reference>();
            var n = helper.number_of_references_in_tx(segments, txSegmentPos);
            var refSegmentIndex = helper.next_ref_segment_index(segments, txSegmentPos, -1);
            for (int i = 0; i < n; i++)
            {
                references.Add(new Reference(segments[refSegmentIndex]));
                refSegmentIndex = helper.next_ref_segment_index(segments, refSegmentIndex + 1, -1);
            }
        }

        private void init_free_text(string[] segments, int txSegmentPos, ParserHelper helper)
        {
            var i = helper.payment_details_segment_index(segments, txSegmentPos, -1);
            if (i < 0) return;
            var s = segments[i].Split('+');
            free_text = s[s.Length - 1];
        }

        private void init_invoice_ref(string[] segments, int txSegmentPos, ParserHelper helper)
        {
            var i = helper.doc_segment_index(segments, txSegmentPos, -1);
            if (i >= 0)
            {
                var s = segments[helper.doc_segment_index(segments, txSegmentPos, -1)].Split('+');
                invoice_ref_type = helper.ref_type(s[1]);
                invoice_ref = s[2];
            }
        }


    }
}