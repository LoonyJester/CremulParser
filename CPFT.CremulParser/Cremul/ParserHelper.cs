﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CPFT.CremulParser.Cremul
{
    public class ParserHelper
    {
        private const string ELEMENT_SEPARATOR = "+";
        private const string DATA_SEPARATOR = ":";
        private const string REPETITION_SEPARATOR = "*";
        private const string SEGMENT_DELIMITER = "'";

        private const string MESSAGE_HEADER = "UNH";
        private const string MESSAGE_BEGIN = "BGM";

        private const string LINE_ITEM_SEGMENT = "LIN";
        private const string DATE_SEGMENT = "DTM";
        private const string BUSINESS_FUNCTION_SEGMENT = "BUS";

        private const string REFERENCE_SEGMENT = "RFF";
        private const string NAME_ADDRESS_SEGMENT = "NAD";
        private const string SEQUENCE_SEGMENT = "SEQ";

        private const string FINANCIAL_INSTITUTION_SEGMENT = "FII";
        private const string BENEFICIARY = "BF";
        // Example: FII+BF+12345678901" (Beneficiary bank account number)

        private const string MONETARY_AMOUNT_SEGMENT = "MOA";
        private const string CURRENCIES_SEGMENT = "CUX";

        private const string PROCESS_TYPE_SEGMENT = "PRC";
        private const string FREETEXT_SEGMENT = "FTX";


        private static Dictionary<string, Dictionary<string, string>> CODES =
            new Dictionary<string, Dictionary<string, string>>
            {
                {
                    BUSINESS_FUNCTION_SEGMENT, new Dictionary<string, string>
                    {
                        {"c230", "Total amount valid KID"},
                        {"c231", "Total amount invalid KID"},
                        {"c232", "Total amount AutoGiro"},
                        {"c233", "Total amount electronic payments"},
                        {"c234", "Total amount Giro notes"},
                        {"c240", "Total amount structured information"}
                    }
                },
                {
                    MONETARY_AMOUNT_SEGMENT, new Dictionary<string, string>
                    {
                        {"c60", "Final posted amount"},
                        {"c346", "Total credit. Sum of final posted amounts on level C"},
                        {"c349", "Amount that will be posted/amount not confirmed by bank"},
                        {"c362", "Amount for information – can be changed"}
                    }
                }
                ,
                {
                    REFERENCE_SEGMENT, new Dictionary<string, string>
                    {
                        {"ACK", "Bank reference - KID in Norwegian"},
                        {
                            "AII",
                            "Bank’s reference number allocated by the bank to different underlaying individual transactions"
                        },
                        {"CT", "AutoGiro agreement ID"}
                    }
                }
            };

        public enum RefTypes
        {
            INVOICE_NUMBER,
            CREDIT_NOTE,
            KID
        }


        private static Dictionary<string, RefTypes> REF_TYPES = new Dictionary<string, RefTypes>
        {
            {"c380", RefTypes.INVOICE_NUMBER},
            {"c381", RefTypes.CREDIT_NOTE},
            {"c999", RefTypes.KID},
            {"c998", RefTypes.INVOICE_NUMBER}
        };

/*    # Assumes that the parameter is a string with the code
            def ref_type(type_number)
              type_symbol = ('c' << type_number).to_sym
              REF_TYPES[type_symbol]
            end  */

        public RefTypes ref_type(string type_number)
        {
            return REF_TYPES[type_number];
        }

        public Dictionary<string, Dictionary<string, string>> segment_codes { get; set; }

        ///     def msg_id_segment_index(segments)
        ///         segments.index { |x| /^UNH.*/.match(x) }
        ///     end
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


        ///   def find_index_by_regex(segments, start_pos, end_pos=nil, regex)
        ///      if end_pos.nil?
        ///        end_pos = next_tx_sequence_segment_index(segments, start_pos+1)
        ///        if end_pos.nil? # no more segments
        ///          end_pos = segments.size
        ///        end
        ///      end
        ///      index = nil
        ///      unless end_pos.nil?
        ///        index = segments[start_pos, end_pos-start_pos].index { |x| regex.match(x) }
        ///        index += start_pos unless index.nil?
        ///      end
        ///      index
        ///    end
        public int find_index_by_regex(string[] segments, int start_pos, int end_pos, string regexPattern)
        {
            if (end_pos < 0)
            {
                end_pos = next_tx_sequence_segment_index(segments, start_pos + 1);
                if (end_pos < 0)
                    end_pos = segments.Length;
            }
            var reg = new Regex(regexPattern);
            for (int i = start_pos; i <= end_pos; i++)
            {
                if (reg.IsMatch(segments[i]))
                    return i;
            }
            return -1;
        }

        ///    def next_date_segment_index(segments, start_pos, end_pos=nil)
        ///      find_index_by_regex(segments, start_pos, end_pos, /^DTM.*/)
        ///    end
        public int next_date_segment_index(string[] segments, int start_pos, int end_pos)
        {
            return find_index_by_regex(segments, start_pos, end_pos, "^DTM.*");
        }

        ///    def next_amount_segment_index(segments, start_pos, end_pos=nil)
        ///      find_index_by_regex(segments, start_pos, end_pos, /^MOA.*/)
        ///    end
        public int next_amount_segment_index(string[] segments, int start_pos, int end_pos)
        {
            return find_index_by_regex(segments, start_pos, end_pos, "^MOA.*");
        }


        ///    def next_line_segment_index(segments, start_pos)
        ///      find_index_by_regex(segments, start_pos, segments.size, /^LIN\+\d/)
        ///    end
        public int next_line_segment_index(string[] segments, int start_pos)
        {
            return find_index_by_regex(segments, start_pos, segments.Length, @"^LIN\+\d");
        }


        ///    def next_tx_sequence_segment_index(segments, start_pos)
        ///      find_index_by_regex(segments, start_pos, segments.size, /^SEQ\+\+\d/)
        ///    end
        public int next_tx_sequence_segment_index(string[] segments, int start_pos)
        {
            return find_index_by_regex(segments, start_pos, segments.Length, @"^SEQ\+\+\d");
        }


        ///    def doc_segment_index(segments, start_pos, end_pos=nil)
        ///      find_index_by_regex(segments, start_pos, end_pos, /^DOC.*/)
        ///    end
        public int doc_segment_index(string[] segments, int start_pos, int end_pos)
        {
            return find_index_by_regex(segments, start_pos, end_pos, "^DOC.*");
        }

        ///    def payment_details_segment_index(segments, start_pos, end_pos=nil)
        ///      find_index_by_regex(segments, start_pos, end_pos, /^FTX\+PMD.*/)
        ///    end
        public int payment_details_segment_index(string[] segments, int start_pos, int end_pos)
        {
            return find_index_by_regex(segments, start_pos, end_pos, @"^FTX\+PMD.*");
        }

        ///    # Optional segment with free text info regarding the payment
        ///    def payment_advice_segment_index(segments, start_pos, end_pos=nil)
        ///      find_index_by_regex(segments, start_pos, end_pos, /^FTX\+AAG.*/)
        ///    end
        public int payment_advice_segment_index(string[] segments, int start_pos, int end_pos)
        {
            return find_index_by_regex(segments, start_pos, end_pos, @"^FTX\+AAG.*");
        }

        ///    def next_ref_segment_index(segments, start_pos, end_pos=nil)
        ///      find_index_by_regex(segments, start_pos, end_pos, /^RFF.*/)
        ///    end
        public int next_ref_segment_index(string[] segments, int start_pos, int end_pos)
        {
            return find_index_by_regex(segments, start_pos, end_pos, "^RFF.*");
        }

        ///    # Bank account of beneficiary
        ///    def next_fii_bf_segment_index(segments, start_pos, end_pos=nil)
        ///      find_index_by_regex(segments, start_pos, end_pos, /^FII\+BF.*/)
        ///    end
        public int next_fii_bf_segment_index(string[] segments, int start_pos, int end_pos)
        {
            return find_index_by_regex(segments, start_pos, end_pos, @"^FII\+BF.*");
        }


        ///    # Bank account of payer
        ///    def next_fii_or_segment_index(segments, start_pos, end_pos=nil)
        ///      find_index_by_regex(segments, start_pos, end_pos, /^FII\+OR.*/)
        ///    end
        public int next_fii_or_segment_index(string[] segments, int start_pos, int end_pos)
        {
            return find_index_by_regex(segments, start_pos, end_pos, @"^FII\+OR.*");
        }

        ///    def next_nad_segment_index(segments, start_pos, end_pos=nil)
        ///      find_index_by_regex(segments, start_pos, end_pos, /^NAD.*/)
        ///    end
        public int next_nad_segment_index(string[] segments, int start_pos, int end_pos)
        {
            return find_index_by_regex(segments, start_pos, end_pos, "^NAD.*");
        }


        ///    def line_count_segment_index(segments)
        ///      segments.index { |x| /^CNT\+LIN?:\d/.match(x) }
        ///    end
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

        ///    def number_of_lines_in_message(segments)
        ///      s = segments[line_count_segment_index(segments)]
        ///      s[s.index(':')+1, s.size].to_i
        ///    end
        public int number_of_lines_in_message(string[] segments)
        {
            var s = segments[line_count_segment_index(segments)];
            return int.Parse(s.Substring(s.IndexOf(":", StringComparison.InvariantCulture) + 1));
        }

        ///    # Return the number of individual payments transactions in the line item. Expects 'line_segment_pos'
        ///    # to be the index of the current line item.
        ///    def number_of_transactions_in_line(segments, line_segment_pos)
        ///      n = 1 # there must be at least 1 payment tx
        ///      tx_pos = next_tx_sequence_segment_index(segments, line_segment_pos+1)
        ///      # search for payment tx items until next line item or end of message
        ///      next_line_index = next_line_segment_index(segments, line_segment_pos+1)
        ///      loop do
        ///        tx_pos = next_tx_sequence_segment_index(segments, tx_pos+1)
        ///        if tx_pos.nil?
        ///          break
        ///        elsif !next_line_index.nil? && tx_pos > next_line_index
        ///          break
        ///       else
        ///          n += 1
        ///        end
        ///      end
        ///      n
        ///    end
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

        ///    def number_of_references_in_tx(segments, tx_segment_pos)
        ///      n = 0
        ///      ref_index = next_ref_segment_index(segments, tx_segment_pos)
        ///      next_tx_index = next_tx_sequence_segment_index(segments, tx_segment_pos+1)
        ///      next_line_index = next_line_segment_index(segments, tx_segment_pos)
        ///      loop do
        ///        if ref_index.nil?
        ///          break
        ///        elsif !next_tx_index.nil? && ref_index > next_tx_index
        ///          break
        ///        elsif !next_line_index.nil? && ref_index > next_line_index
        ///          break
        ///        else
        ///          n += 1
        ///        end
        ///        ref_index = next_ref_segment_index(segments, ref_index+1)
        ///      end
        ///      n
        ///    end
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


        ///    def last_segment_index_in_tx(segments, tx_segment_pos)
        ///      next_tx_index = next_tx_sequence_segment_index(segments, tx_segment_pos+1)
        ///      next_line_index = next_line_segment_index(segments, tx_segment_pos)
        ///      cnt_index = line_count_segment_index(segments)
        ///
        ///      if next_tx_index
        ///        next_tx_index - 1
        ///      elsif next_line_index
        ///        next_line_index - 1
        ///      else
        ///        cnt_index - 1
        ///      end
        ///    end

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
    }
}
