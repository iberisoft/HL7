using System.Collections.Generic;

namespace HL7
{
    class Field
    {
        Message m_Message;

        public Field(Message message)
        {
            m_Message = message;
        }

        public Field(Message message, string value)
        {
            m_Message = message;
            Components.Add(value);
        }

        public List<string> Components { get; } = new List<string>();

        public void FromString(string text)
        {
            Components.Clear();
            Components.AddRange(text.Split(m_Message.ComponentSeparator));
        }

        public override string ToString() => string.Join(new string(m_Message.ComponentSeparator, 1), Components);
    }
}
