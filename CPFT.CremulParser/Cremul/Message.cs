using System.Collections.Generic;

namespace CPFT.CremulParser.Cremul
{
    public class Message
    {
        public int message_index { get; set; }
        public CremulHeader header { get; set; }
        public List<Line> lines { get; set; }
        public int number_of_lines { get; set; }

//  # The message_index is the index number of the Cremul message in the file.
//  def initialize(message_number, segments)
//    @message_index = message_number
//    @header = CremulHeader.new(segments)
//    @lines = []
//    @number_of_lines = number_of_lines_in_message(segments)
//
//    # instantiate the line items
//    line_segment_pos = next_line_segment_index(segments, 0)
//    @number_of_lines.times do |n|
//      CremulParser.logger.info "CremulParser: file=#{CremulParser.filename}, parsing line #{n+1}"
//      @lines << CremulLine.new(n+1, segments, line_segment_pos)
//      line_segment_pos = next_line_segment_index(segments, line_segment_pos+1)
//    end
        public Message(int messageNumber, string[] segments, ParserHelper helper)
        {
            message_index = messageNumber;
            header = new CremulHeader(segments, helper);
            lines = new List<Line>();
            number_of_lines = helper.number_of_lines_in_message(segments);
//     instantiate the line items
            var lineSegmentPos = helper.next_line_segment_index(segments, 0);
            for (int i = 0; i < number_of_lines; i++)
            {
                lines.Add(new Line(i + 1, segments, lineSegmentPos, helper));
                lineSegmentPos = helper.next_line_segment_index(segments, lineSegmentPos + 1);
            }
        }
    }
}