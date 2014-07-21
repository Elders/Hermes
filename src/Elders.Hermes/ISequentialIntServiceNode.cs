using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Elders.Hermes
{
    public interface ISequentialIntServiceNode
    {
        Guid Id { get; }
        Guid ParentNodeId { get; }
        ConcurrentBag<Interval> Intervals { get; }
        void AddInterval(Interval interval);
        int GetNextRevision(Guid gd, int revision);
    }
}