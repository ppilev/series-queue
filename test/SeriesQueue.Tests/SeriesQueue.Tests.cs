using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ContinuousQueueTests
{
    public class Tests
    {
        private SeriesQueue<MessageItem> queue;

        [SetUp]
        public void Setup()
        {
            queue = new SeriesQueue<MessageItem>();
        }

        [Test]
        public void SingleItemAtQueueHeadTest()
        {
            queue = new SeriesQueue<MessageItem>(nextOrdinal: 4);
            var message = new MessageItem(new Message { Sequence = 4, Timestamp = DateTime.UtcNow, Data = "Message4" });
            var items = queue.AddAndDequeueAll(message);

            items.Should().BeEquivalentTo(new[] { message });
            queue.Count.Should().Be(0);
        }

        [Test]
        public void SingleItemAheadOfQueueHeadTest()
        {
            queue = new SeriesQueue<MessageItem>(nextOrdinal: 4);
            var message = new MessageItem(new Message { Sequence = 10, Timestamp = DateTime.UtcNow, Data = "Message1" });
            var items = queue.AddAndDequeueAll(message);

            items.Should().BeEmpty();
            queue.Count.Should().Be(1);
        }

        [Test]
        public void ItemsInOrderTest()
        {
            var messages = new List<MessageItem>
            {
                new MessageItem(new Message { Sequence = 0, Timestamp = DateTime.UtcNow, Data = "Message1" }),
                new MessageItem(new Message { Sequence = 1, Timestamp = DateTime.UtcNow, Data = "Message2" }),
                new MessageItem(new Message { Sequence = 2, Timestamp = DateTime.UtcNow, Data = "Message3" }),
            };

            queue.Enqueue(messages[0]);
            queue.Enqueue(messages[1]);

            var items = queue.AddAndDequeueAll(messages[2]);

            items.Should().BeEquivalentTo(messages);
        }

        [Test]
        public void ItemsInReverseOrderTest()
        {
            var messages = new List<MessageItem>
            {
                new MessageItem(new Message { Sequence = 2, Timestamp = DateTime.UtcNow, Data = "Message3" }),
                new MessageItem(new Message { Sequence = 1, Timestamp = DateTime.UtcNow, Data = "Message2" }),
                new MessageItem(new Message { Sequence = 0, Timestamp = DateTime.UtcNow, Data = "Message1" }),
            };

            queue.Enqueue(messages[0]);
            queue.Enqueue(messages[1]);

            var items = queue.AddAndDequeueAll(messages[2]);
            items.Should().BeEquivalentTo(messages.OrderBy(m => m.Ordinal));
        }

        [Test]
        public void ItemsInOrderAheadOfQueueHeadTest()
        {
            var messages = new List<MessageItem>
            {
                new MessageItem(new Message { Sequence = 1, Timestamp = DateTime.UtcNow, Data = "Message2" }),
                new MessageItem(new Message { Sequence = 2, Timestamp = DateTime.UtcNow, Data = "Message3" }),
                new MessageItem(new Message { Sequence = 0, Timestamp = DateTime.UtcNow, Data = "Message1" }),
            };

            queue.Enqueue(messages[0]);
            queue.Enqueue(messages[1]);

            var items = queue.AddAndDequeueAll(messages[2]);

            items.Should().BeEquivalentTo(messages.OrderBy(m => m.Ordinal));
        }

        [Test]
        public void ItemsInScatteredSeriesAheadOfQueueHeadTest()
        {
            var messages = new List<MessageItem>
            {
                new MessageItem(new Message { Sequence = 1, Timestamp = DateTime.UtcNow, Data = "Message2" }),
                new MessageItem(new Message { Sequence = 2, Timestamp = DateTime.UtcNow, Data = "Message3" }),

                new MessageItem(new Message { Sequence = 4, Timestamp = DateTime.UtcNow, Data = "Message4" }),
                new MessageItem(new Message { Sequence = 5, Timestamp = DateTime.UtcNow, Data = "Message5" }),

                new MessageItem(new Message { Sequence = 0, Timestamp = DateTime.UtcNow, Data = "Message1" }),
                new MessageItem(new Message { Sequence = 3, Timestamp = DateTime.UtcNow, Data = "Message3" }),
            };

            // scattered series
            queue.Enqueue(messages[0]);
            queue.Enqueue(messages[1]);
            queue.Enqueue(messages[2]);
            queue.Enqueue(messages[3]);

            // complete the first series
            var items = queue.AddAndDequeueAll(messages[4]);

            items.Should().HaveCount(3);
            items.Should().BeEquivalentTo(messages.Where(m => m.Ordinal < 3).OrderBy(m => m.Ordinal));
            queue.Count.Should().Be(2);

            // complete the second series
            items = queue.AddAndDequeueAll(messages[5]);

            items.Should().HaveCount(3);
            items.Should().BeEquivalentTo(messages.Where(m => m.Ordinal >= 3).OrderBy(m => m.Ordinal));
            queue.Count.Should().Be(0);
        }

        [Test]
        public void CreateQueueFromEmptyList()
        {
            queue = new SeriesQueue<MessageItem>(new List<MessageItem>());

            queue.Count.Should().Be(0);
            queue.NextOrdinal.Should().Be(0);
        }

        [Test]
        public void CreateQueueFromEmptyListAndNextSequence()
        {
            queue = new SeriesQueue<MessageItem>(new List<MessageItem>(), 10);

            queue.Count.Should().Be(0);
            queue.NextOrdinal.Should().Be(10);
        }

        [Test]
        public void CreateQueueFromBinaryTreeList()
        {
            var messages = new List<MessageItem>
            {
                new MessageItem(new Message { Sequence = 10, Timestamp = DateTime.UtcNow, Data = "Message3" }),
                new MessageItem(new Message { Sequence = 11, Timestamp = DateTime.UtcNow, Data = "Message2" }),
                new MessageItem(new Message { Sequence = 12, Timestamp = DateTime.UtcNow, Data = "Message1" }),
            };

            queue = new SeriesQueue<MessageItem>(messages);
            queue.NextOrdinal.Should().Be(10);
        }

        [Test]
        public void CreateQueueFromBinaryTreeListAndNextSequence()
        {
            var messages = new List<MessageItem>
            {
                new MessageItem(new Message { Sequence = 10, Timestamp = DateTime.UtcNow, Data = "Message3" }),
                new MessageItem(new Message { Sequence = 11, Timestamp = DateTime.UtcNow, Data = "Message2" }),
                new MessageItem(new Message { Sequence = 12, Timestamp = DateTime.UtcNow, Data = "Message1" }),
            };

            queue = new SeriesQueue<MessageItem>(messages, 15);
            queue.NextOrdinal.Should().Be(15);
            queue.Count.Should().Be(3);
        }


        [Test]
        public void CreateQueueAheadOfNextSequence()
        {
            var messages = new List<MessageItem>
            {
                new MessageItem(new Message { Sequence = 10, Timestamp = DateTime.UtcNow, Data = "Message3" }),
                new MessageItem(new Message { Sequence = 11, Timestamp = DateTime.UtcNow, Data = "Message2" }),
                new MessageItem(new Message { Sequence = 12, Timestamp = DateTime.UtcNow, Data = "Message1" }),
            };

            Assert.Throws<InvalidOperationException>(() =>
            {
                queue = new SeriesQueue<MessageItem>(messages, 5);
            });
        }
    }

}