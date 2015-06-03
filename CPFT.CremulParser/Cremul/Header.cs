using System;

namespace CPFT.CremulParser.Cremul
{
   public class CremulHeader
    {
//  include Cremul::ParserHelper

//  # bf_id : Beneficiary ID. When the beneficiary is an organization this will be the organzation-number,
//  #         and when the beneficiary is a person this will be the SSN, aka personnummer (NO).

        public string msg_id { get; set; }
        public DateTime created_date { get; set; }
        public int bf_id { get; set; }


        ///  # Expects an array with all segments in the CREMUL file
        ///  def initialize(segments)
        ///    @msg_id = segments[msg_id_segment_index(segments)]
        ///
        ///    d = segments[next_date_segment_index(segments, 0)].split(':')
        ///    @created_date = Date.parse(d[1])
        ///
        ///    bf_nad_index = next_nad_segment_index(segments, 0) # may not be present in the header
        ///    unless bf_nad_index.nil?
        ///      nad = segments[bf_nad_index]
        ///      @bf_id = nad.split('+')[2].to_i
        ///    end
        ///  end
        public CremulHeader(string[] segments, ParserHelper helper)
        {
            //  # Expects an array with all segments in the CREMUL file
            msg_id = segments[helper.msg_id_segment_index(segments)];

            var d = segments[helper.next_date_segment_index(segments, 0, -1)].Split(':');
            created_date = DateTime.Parse(d[1]);

            var bfNadIndex = helper.next_nad_segment_index(segments, 0, -1); // may not be present in the header
            if (bfNadIndex >= 0)
            {
                var nad = segments[bfNadIndex];
                @bf_id = int.Parse(nad.Split('+')[2]);
            }

        }


    }
}
