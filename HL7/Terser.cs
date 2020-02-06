using System;

namespace HL7
{
    public class Terser
    {
        public Terser(string text)
        {
            var tokens = text.Split('-');
            if (tokens.Length == 2 || tokens.Length == 3)
            {
                SegmentName = tokens[0];
                FieldIndex = int.Parse(tokens[1]);
                if (tokens.Length == 3)
                {
                    ComponentIndex = int.Parse(tokens[2]);
                }
            }
            else
            {
                throw new ArgumentException("Must be 2 or 3 tokens", nameof(text));
            }
        }

        public string SegmentName { get; }

        public int FieldIndex { get; }

        public int ComponentIndex { get; }
    }
}
