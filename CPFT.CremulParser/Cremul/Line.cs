using System;
using System.Collections.Generic;

namespace CPFT.CremulParser.Cremul
{
// Represents a line item in a CREMUL payment transaction message. A line item may have
// 1-* child-elements with the individual payments. A line item will have an amount field
// which will be the sum of the amounts of the child payment items.
    public class Line
    {
        public int LineIndex { get; set; }
        public DateTime PostingDate { get; set; }
        public Reference Reference { get; set; }
        public List<PaymentTx> Transactions { get; set; }

        public string BfAccountNumber { get; set; }

        public Money Money { get; set; }

        public Line(int lineIndex, string[] segments, int lineSegmentPos, ParserHelper helper)
        {
            LineIndex = lineIndex;
            var d = segments[helper.next_date_segment_index(segments, lineSegmentPos, -1)].Split(':');
            PostingDate = helper.ParseCremulDate(d[1]);

            Reference = new Reference(segments[helper.next_ref_segment_index(segments, lineSegmentPos, -1)]);
            Money = new Money(segments[helper.next_amount_segment_index(segments, lineSegmentPos, -1)]);
            var bf = segments[helper.next_fii_bf_segment_index(segments, lineSegmentPos, -1)].Split('+');
            BfAccountNumber = bf[2];

            Transactions = new List<PaymentTx>();
            var n = helper.number_of_transactions_in_line(segments, lineSegmentPos);
            var txIndex = helper.next_tx_sequence_segment_index(segments, lineSegmentPos);
            for (int i = 0; i < n; i++)
            {
                Transactions.Add(new PaymentTx(i + 1, segments, txIndex, helper));
                txIndex = helper.next_tx_sequence_segment_index(segments, txIndex + 1);
            }
        }
    }
}