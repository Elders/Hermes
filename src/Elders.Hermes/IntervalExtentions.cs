using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Elders.Hermes
{
    public static class IntervalExtentions
    {
        public static Interval Split(this Interval self, int count)
        {
            var oldValue = self.End;

            var margin = (self.End - self.Start) / count;

            self.End = self.End - margin;

            var newInterval = new Interval(self.End + 1, oldValue);

            return newInterval;
        }
    }
}