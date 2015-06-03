// A file may contain one or more Cremul messages. A Cremul message consists of 'message segments'.
//Each segment starts with a Cremul keyword. A group of segments make up a logical element of the
//# Cremul message. The overall logical structure of a Cremul message is as follows:
//#
//# [ Cremul message ] 1 --- * [ Line ] 1 --- * [ Payment TX ]
//#

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CPFT.CremulParser.Cremul;

namespace CPFT.CremulParser
{
    public class CremulParser
    {
        private readonly ParserHelper _helper;

        
        public List<Message> messages { get; set; }

        public CremulParser()
        {
            _helper = new ParserHelper();
            messages = new List<Message>();
        }

        public bool parse(string filePath)
        {
            if(!File.Exists(filePath)) return false;
            var segments = GetSegments(filePath);

            List<int> m = _helper.get_messages_in_file(segments);
            if (m.Any())
            {
                for (int i = 0; i < m.Count; i++)
                {
                    string[] sub;
                    if ((i + 1) == m.Count)
                        sub = segments.Skip(m[i]).Take(segments.Length - m[i]).ToArray();
                    else
                        sub =segments.Skip(m[i]).Take(m[i+1] - m[i]).ToArray();
                   messages.Add(new Message(i+1, sub, _helper)); 
                }
                return true;
            }
            return false;            
        }

        private static string[] GetSegments(string filePath)
        {
            var fileAsAString = new StringBuilder();
            string line;
            // Read the file and display it line by line.
            var file = new StreamReader(filePath);
            while ((line = file.ReadLine()) != null)
                fileAsAString.Append(line);
            file.Close();
            return  fileAsAString.ToString().Split('\'').Select(s => s.Trim()).Where(x=>!string.IsNullOrEmpty(x)).ToArray();
        }
    }
}
