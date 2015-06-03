using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CPFT.CremulParser.Cremul
{
    public class ParserHelper
    {

        private static readonly Dictionary<string, RefTypes> RefTypes = new Dictionary<string, RefTypes>
        {
            {"380", Cremul.RefTypes.InvoiceNumber},
            {"381", Cremul.RefTypes.CreditNote},
            {"999", Cremul.RefTypes.Kid},
            {"998", Cremul.RefTypes.InvoiceNumber}
        };

        public RefTypes ref_type(string typeNumber)
        {
            return RefTypes[typeNumber];
        }

        public Dictionary<string, Dictionary<string, string>> SegmentCodes { get; set; }

        public int msg_id_segment_index(string[] segments)
        {
            var reg = new Regex("^UNH.*");
            for (var i = 0; i < segments.Length; i++)
            {
                if (reg.IsMatch(segments[i]))
                    return i;
            }
            return -1;
        }

        public int find_index_by_regex(string[] segments, int startPos, int endPos, string regexPattern)
        {
            if (endPos < 0)
            {
                endPos = next_tx_sequence_segment_index(segments, startPos + 1);
                if (endPos < 0)
                    endPos = segments.Length;
            }
            var reg = new Regex(regexPattern);
            for (int i = startPos; i < endPos; i++)
            {
                if (reg.IsMatch(segments[i]))
                    return i;
            }
            return -1;
        }

        public int next_date_segment_index(string[] segments, int startPos, int endPos)
        {
            return find_index_by_regex(segments, startPos, endPos, "^DTM.*");
        }

        public int next_amount_segment_index(string[] segments, int startPos, int endPos)
        {
            return find_index_by_regex(segments, startPos, endPos, "^MOA.*");
        }


        public int next_line_segment_index(string[] segments, int startPos)
        {
            return find_index_by_regex(segments, startPos, segments.Length, @"^LIN\+\d");
        }

        public int next_tx_sequence_segment_index(string[] segments, int startPos)
        {
            return find_index_by_regex(segments, startPos, segments.Length, @"^SEQ\+\+\d");
        }

        public int doc_segment_index(string[] segments, int startPos, int endPos)
        {
            return find_index_by_regex(segments, startPos, endPos, "^DOC.*");
        }

        public int payment_details_segment_index(string[] segments, int startPos, int endPos)
        {
            return find_index_by_regex(segments, startPos, endPos, @"^FTX\+PMD.*");
        }

        public int payment_advice_segment_index(string[] segments, int startPos, int endPos)
        {
            return find_index_by_regex(segments, startPos, endPos, @"^FTX\+AAG.*");
        }

        public int next_ref_segment_index(string[] segments, int startPos, int endPos)
        {
            return find_index_by_regex(segments, startPos, endPos, "^RFF.*");
        }

        public int next_fii_bf_segment_index(string[] segments, int startPos, int endPos)
        {
            return find_index_by_regex(segments, startPos, endPos, @"^FII\+BF.*");
        }

        public int next_fii_or_segment_index(string[] segments, int startPos, int endPos)
        {
            return find_index_by_regex(segments, startPos, endPos, @"^FII\+OR.*");
        }

        public int next_nad_segment_index(string[] segments, int startPos, int endPos)
        {
            return find_index_by_regex(segments, startPos, endPos, "^NAD.*");
        }

        public int line_count_segment_index(string[] segments)
        {
            var reg = new Regex(@"^CNT\+LIN?:\d");
            for (var i = 0; i < segments.Length; i++)
            {
                if (reg.IsMatch(segments[i]))
                    return i;
            }
            return -1;
        }

        public int number_of_lines_in_message(string[] segments)
        {
            var s = segments[line_count_segment_index(segments)];
            return int.Parse(s.Substring(s.IndexOf(":", StringComparison.InvariantCulture) + 1));
        }

        public int number_of_transactions_in_line(string[] segments, int lineSegmentPos)
        {
            var n = 1; // there must be at least 1 payment tx
            var txPos = next_tx_sequence_segment_index(segments, lineSegmentPos + 1);
            //      # search for payment tx items until next line item or end of message
            var nextLineIndex = next_line_segment_index(segments, lineSegmentPos + 1);
            while (true)
            {
                txPos = next_tx_sequence_segment_index(segments, txPos + 1);
                if (txPos < 0)
                    break;
                if (nextLineIndex >= 0 && txPos > nextLineIndex)
                    break;
                n += 1;
            }
            return n;
        }

        public int number_of_references_in_tx(string[] segments, int txSegmentPos)
        {
            int n = 0;
            var refIndex = next_ref_segment_index(segments, txSegmentPos, -1);
            var nextTxIndex = next_tx_sequence_segment_index(segments, txSegmentPos + 1);
            var nextLineIndex = next_line_segment_index(segments, txSegmentPos);
            while (true)
            {
                if (refIndex < 0)
                    break;
                if (nextTxIndex >= 0 && refIndex > nextTxIndex)
                    break;
                if (nextLineIndex > 0 && refIndex > nextLineIndex)
                    break;
                n += 1;
                refIndex = next_ref_segment_index(segments, refIndex + 1, -1);
            }
            return n;
        }

        public List<int> get_messages_in_file(string[] segments)
        {
            var m = new List<int>();
            var reg = new Regex("^UNA.*");
            for (int i = 0; i < segments.Length; i++)
            {
                if (reg.IsMatch(segments[i]))
                    m.Add(i);
            }
            return m;
        }

        public int last_segment_index_in_tx(string[] segments, int txSegmentPos)
        {
            int nextTxIndex = next_tx_sequence_segment_index(segments, txSegmentPos + 1);
            int nextLineIndex = next_line_segment_index(segments, txSegmentPos);
            int cntIndex = line_count_segment_index(segments);

            if (nextTxIndex > 0)
                return nextTxIndex - 1;
            if (nextLineIndex > 0)
                return nextLineIndex - 1;
            return cntIndex - 1;
        }

        public DateTime ParseCremulDate(string date)
        {
            return DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
        }
    }
}
