using System;

namespace CPFT.CremulParser.Cremul
{
   public class CremulHeader
    {

//   bf_id : Beneficiary ID. When the beneficiary is an organization this will be the organzation-number,
//           and when the beneficiary is a person this will be the SSN, aka personnummer (NO).

        public string MsgId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int BfId { get; set; }


        public CremulHeader(string[] segments, ParserHelper helper)
        {
            MsgId = segments[helper.msg_id_segment_index(segments)];

            var d = segments[helper.next_date_segment_index(segments, 0, -1)].Split(':');
            CreatedDate = helper.ParseCremulDate(d[1]);

            var bfNadIndex = helper.next_nad_segment_index(segments, 0, -1); // may not be present in the header
            if (bfNadIndex >= 0)
            {
                var nad = segments[bfNadIndex];
                BfId = int.Parse(nad.Split('+')[2]);
            }

        }


    }
}
