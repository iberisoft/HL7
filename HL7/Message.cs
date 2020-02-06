using System;
using System.Collections.Generic;
using System.Linq;

namespace HL7
{
    public class Message
    {
        public static char[] SegmentSeparator => new[] { '\r', '\n' };

        public char FieldSeparator { get; private set; } = '|';

        public char ComponentSeparator { get; private set; } = '^';

        public List<Segment> Segments { get; } = new List<Segment>();

        public void FromString(string text)
        {
            if (!text.StartsWith("MSH"))
            {
                throw new ArgumentException("Must start from MSH", nameof(text));
            }
            FieldSeparator = text[3];
            ComponentSeparator = text[4];

            Segments.Clear();
            Segments.AddRange(text.Split(SegmentSeparator, StringSplitOptions.RemoveEmptyEntries).Select(text2 =>
            {
                var segment = new Segment(this);
                segment.FromString(text2);
                return segment;
            }));
        }

        public override string ToString() => string.Join(new string(SegmentSeparator[0], 1), Segments.Select(segment => segment.ToString()));

        public Segment this[string segmentName] => Segments.FirstOrDefault(segment => segment.Name == segmentName);

        public string this[Terser terser]
        {
            get
            {
                var segment = this[terser.SegmentName];
                if (terser.ComponentIndex != 0)
                {
                    return segment[terser.FieldIndex, terser.ComponentIndex];
                }
                else
                {
                    return segment[terser.FieldIndex];
                }
            }
            set
            {
                var segment = this[terser.SegmentName];
                if (terser.ComponentIndex != 0)
                {
                    segment[terser.FieldIndex, terser.ComponentIndex] = value;
                }
                else
                {
                    segment[terser.FieldIndex] = value;
                }
            }
        }

        public Message CreateAck(bool success)
        {
            var ack = new Message();
            ack.Segments.Add(new Segment(ack, this["MSH"]));
            ack["MSH"][3] = this["MSH"][5];
            ack["MSH"][4] = this["MSH"][6];
            ack["MSH"][5] = this["MSH"][3];
            ack["MSH"][6] = this["MSH"][4];
            ack["MSH"][7] = StringConverter.FromDateTime(DateTime.Now);
            ack["MSH"][9] = "ACK";
            ack["MSH"][10] = "A" + this["MSH"][10];
            ack.Segments.Add(new Segment(ack, "MSA"));
            ack["MSA"][1] = success ? "AA" : "AE";
            ack["MSA"][2] = this["MSH"][10];
            return ack;
        }
    }
}
