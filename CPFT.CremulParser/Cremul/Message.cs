using System.Collections.Generic;

namespace CPFT.CremulParser.Cremul
{
    public class Message
    {
        public int MessageIndex { get; set; }
        public CremulHeader Header { get; set; }
        public List<Line> Lines { get; set; }
        public int NumberOfLines { get; set; }

        public Message(int messageNumber, string[] segments, ParserHelper helper)
        {
            MessageIndex = messageNumber;
            Header = new CremulHeader(segments, helper);
            Lines = new List<Line>();
            NumberOfLines = helper.number_of_lines_in_message(segments);
            var lineSegmentPos = helper.next_line_segment_index(segments, 0);
            for (int i = 0; i < NumberOfLines; i++)
            {
                Lines.Add(new Line(i + 1, segments, lineSegmentPos, helper));
                lineSegmentPos = helper.next_line_segment_index(segments, lineSegmentPos + 1);
            }
        }
    }
}