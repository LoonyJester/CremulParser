using System.Collections.Generic;

namespace CPFT.CremulParser.Cremul
{
    public class NameAndAddress
    {
//   NAD segment in unstructured form:
//   NAD+party-id+code+code+nad-line1+nad-line2+nad-line3+nad-line4+nad-line5
//  
//   NAD segment in structured form (3 variants):
//   NAD+party-id+code+code+nad-line OR
//   NAD+party-id+code+nad-line OR
//   NAD+party-id+nad-line
//  
//   In the structured form the nad-line will have colon (:) as separator between the address parts
//  
//   Party-IDs:
//   MR - Beneficiary bank
//   BE - Beneficiary, the ultimate recipient of the funds
//   PL - Payor

        public List<string> NadLines { get; set; }
        //TODO: need refactor to enum
        public string Type { get; set; }

        public NameAndAddress(string nadSegment)
        {
            var s = nadSegment.Split('+');
            Type = s[1];

            NadLines = new List<string>();
            if (s.Length <= 5)
            {
                //  structured form
                var addr = s[s.Length - 1].Split(':');
                foreach (string t in addr)
                    NadLines.Add(t);
            }
            else
            {
                for (var i = 4; i < s.Length; i++)
                    NadLines.Add(s[i]);
            }
        }
    }
}