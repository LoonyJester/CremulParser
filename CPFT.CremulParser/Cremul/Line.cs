using System;
using System.Collections.Generic;

namespace CPFT.CremulParser.Cremul
{
//# Represents a line item in a CREMUL payment transaction message. A line item may have
//# 1-* child-elements with the individual payments. A line item will have an amount field
//# which will be the sum of the amounts of the child payment items.
    public class Line
    {
        public int line_index { get; set; }
        public DateTime posting_date { get; set; }
        public Reference reference { get; set; }
        public List<PaymentTx> transactions { get; set; }

        public string bf_account_number { get; set; }

        public Money money { get; set; }

        public Line(int lineIndex, string[] segments, int lineSegmentPos, ParserHelper helper)
        {

            line_index = lineIndex;
            var d = segments[helper.next_date_segment_index(segments, lineSegmentPos, -1)].Split(':');
            posting_date = helper.ParseCremulDate(d[1]);

            reference = new Reference(segments[helper.next_ref_segment_index(segments, lineSegmentPos, -1)]);
            money = new Money(segments[helper.next_amount_segment_index(segments, lineSegmentPos, -1)]);
            var bf = segments[helper.next_fii_bf_segment_index(segments, lineSegmentPos, -1)].Split('+');
            bf_account_number = bf[2];

            transactions = new List<PaymentTx>();
            var n = helper.number_of_transactions_in_line(segments, lineSegmentPos);
            var txIndex = helper.next_tx_sequence_segment_index(segments, lineSegmentPos);
            for (int i = 0; i < n; i++)
            {
//      CremulParser.logger.info "CremulParser: file=#{CremulParser.filename}, parsing tx #{i+1}"
                transactions.Add(new PaymentTx(i + 1, segments, txIndex, helper));
                txIndex = helper.next_tx_sequence_segment_index(segments, txIndex + 1);
            }
        }
    }
}