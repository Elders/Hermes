using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Elders.Hermes
{
    public interface ISequentialIntService
    {
        Guid Id { get; }
        ISequentialIntServiceNode GetServiceNode(int hash);
        ConcurrentBag<ISequentialIntServiceNode> GetServiceNodes();
        int GetNextRevision(Guid gd, int revision);
        void MakeAware(ISequentialIntService newService);
        void CreateServiceNodes();
        void AddServiceNode(ISequentialIntServiceNode node);
    }
}