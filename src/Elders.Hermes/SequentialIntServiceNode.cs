using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Elders.Hermes
{
    public class SequentialIntServiceNode : ISequentialIntServiceNode
    {
        private ConcurrentDictionary<Guid, int> revisions = new ConcurrentDictionary<Guid, int>();

        private ConcurrentBag<Interval> intervals = new ConcurrentBag<Interval>();

        public ConcurrentBag<Interval> Intervals
        {
            get
            {
                return intervals;
            }
        }

        private Guid id;

        public Guid Id
        {
            get
            {
                return id;
            }
        }

        private Guid parentNodeId;

        public Guid ParentNodeId
        {
            get
            {
                return parentNodeId;
            }
        }

        public SequentialIntServiceNode(Guid parentNodeId)
        {
            this.id = Guid.NewGuid();
            this.parentNodeId = parentNodeId;
        }

        public void AddInterval(Interval interval)
        {
            this.intervals.Add(interval);
        }

        public int GetNextRevision(Guid gd, int revision)
        {
            return revisions.AddOrUpdate(gd, revision, (x, y) => y + 1);
        }
    }
}