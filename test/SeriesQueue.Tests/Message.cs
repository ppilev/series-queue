using System;

namespace ContinuousQueueTests
{
    public class Message
    {
        public string Data { get; set; }
        public DateTime Timestamp { get; set; }
        public int Sequence { get; set; }
    }
}