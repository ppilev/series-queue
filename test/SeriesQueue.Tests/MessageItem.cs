using System.Collections;

namespace ContinuousQueueTests
{
    public class MessageItem : SeriesItem<Message>
    {
        public MessageItem(Message message) : base(message, message.Sequence)
        {
        }
    }
}