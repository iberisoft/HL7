using System;
using System.Collections.Generic;
using System.Linq;

namespace HL7
{
    public class Segment
    {
        Message m_Message;

        internal Segment(Message message)
        {
            m_Message = message;
        }

        public Segment(Message message, string name)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            if (name.Length != 3)
            {
                throw new ArgumentException("Must have 3 characters", nameof(name));
            }

            m_Message = message;
            Fields.Add(new Field(m_Message, name));
            if (name == "MSH")
            {
                Fields.Add(new Field(m_Message, ""));
                Fields.Add(new Field(m_Message, $@"{m_Message.ComponentSeparator}~\&"));
            }
        }

        public Segment(Message message, Segment src)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            m_Message = message;
            FromString(src.ToString());
        }

        internal List<Field> Fields { get; } = new List<Field>();

        public string Name => Fields[0].ToString();

        public void FromString(string text)
        {
            Fields.Clear();
            Fields.AddRange(text.Split(m_Message.FieldSeparator).Select(text2 =>
            {
                var field = new Field(m_Message);
                field.FromString(text2);
                return field;
            }));
            if (text.StartsWith("MSH"))
            {
                Fields.Insert(1, new Field(m_Message, ""));
            }
        }

        public override string ToString()
        {
            var fields = Fields.AsEnumerable();
            if (Name == "MSH")
            {
                fields = fields.Where((field, i) => i != 1);
            }
            return string.Join(new string(m_Message.FieldSeparator, 1), fields.Select(field => field.ToString()));
        }

        public int GetComponentCount(int fieldIndex)
        {
            if (fieldIndex >= Fields.Count)
            {
                return 0;
            }
            var field = Fields[fieldIndex];
            return field.Components.Count;
        }

        public string this[int fieldIndex]
        {
            get
            {
                if (fieldIndex >= Fields.Count)
                {
                    return null;
                }
                var field = Fields[fieldIndex];
                var value = field.ToString();
                return value != "" ? value : null;
            }
            set
            {
                if (Fields.Count <= fieldIndex)
                {
                    for (var i = Fields.Count; i <= fieldIndex; ++i)
                    {
                        Fields.Add(new Field(m_Message, ""));
                    }
                }

                value = value ?? "";
                var field = new Field(m_Message);
                field.FromString(value);
                Fields[fieldIndex] = field;
            }
        }

        public string this[int fieldIndex, int componentIndex]
        {
            get
            {
                if (fieldIndex >= Fields.Count)
                {
                    return null;
                }
                var field = Fields[fieldIndex];
                if (componentIndex - 1 >= field.Components.Count)
                {
                    return null;
                }
                var value = field.Components[componentIndex - 1];
                return value != "" ? value : null;
            }
            set
            {
                if (Fields.Count <= fieldIndex)
                {
                    for (var i = Fields.Count; i <= fieldIndex; ++i)
                    {
                        Fields.Add(new Field(m_Message, ""));
                    }
                }

                value = value ?? "";
                var field = Fields[fieldIndex];
                if (field.Components.Count < componentIndex)
                {
                    for (var i = field.Components.Count; i < componentIndex; ++i)
                    {
                        field.Components.Add("");
                    }
                }
                field.Components[componentIndex - 1] = value;
            }
        }

        public IEnumerable<string> this[int fieldIndex, int componentIndex, int componentCount]
        {
            get
            {
                for (var i = 0; i < componentCount; ++i)
                {
                    yield return this[fieldIndex, componentIndex + i];
                }
            }
        }
    }
}
