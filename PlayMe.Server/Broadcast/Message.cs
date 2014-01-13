using System;

namespace PlayMe.Server.Broadcast
{
    public class Message : IMessage
    {
        private readonly string template;
        private readonly string data;

        public Message(string template, string data)
        {
            this.data = data;
            this.template = template;
        }

        public string GetMessage(int truncateAt)
        {

            int lengthRemaining = truncateAt - String.Format(template, string.Empty).Length;

            return data.Length >= lengthRemaining ? FormatMessage(data.Substring(0, (lengthRemaining - 3)) + "...") : FormatMessage(data);
        }

        public string GetMessage()
        {
            return FormatMessage(data);
        }

        private string FormatMessage(string messageData)
        {
            return string.Format(template,messageData);
        }
    }
}
