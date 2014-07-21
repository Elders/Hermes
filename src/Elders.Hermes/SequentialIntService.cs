using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Elders.Hermes
{
    public class SequentialIntService : ISequentialIntService
    {
        public SequentialIntService(ISequentialIntService service = null)
        {
            this.id = Guid.NewGuid();

            this.MakeAware(service);

            CreateServiceNodes();
        }

        public void CreateServiceNodes()
        {
            var numberOfNodesPerService = 5;

            if (services.Count > 0)
            {
                foreach (var svc in services)
                {
                    var serviceNodes = svc.GetServiceNodes();
                    
                    foreach (var node in serviceNodes)
                    {
                        var newNode = new SequentialIntServiceNode(this.Id);

                        foreach (var interval in node.Intervals)
                        {
                            var newInterval = interval.Split(services.Count + 1);

                            newNode.AddInterval(interval);
                        }

                        this.AddServiceNode(newNode);
                    }
                }

                var allNodes = GetAllNodesFromAllServices();

                foreach (var node in allNodes)
                {
                    DistributeTheNode(node);
                }
            }
            else
            {
                int minValue = int.MinValue;
                int nodeInterval = (int.MaxValue / numberOfNodesPerService) * 2;

                for (int i = 0; i < numberOfNodesPerService; i++)
                {
                    int nodeMinValue = minValue + (i * nodeInterval);
                    int nodeMaxValue = minValue + ((i + 1) * nodeInterval) - 1;

                    if (i + 1 == numberOfNodesPerService)
                        nodeMaxValue = int.MaxValue;

                    var newInterval = new Interval(nodeMinValue, nodeMaxValue);

                    var newNode = new SequentialIntServiceNode(this.Id);

                    newNode.AddInterval(newInterval);

                    this.nodes.Add(newNode);
                }
            }
        }

        private ConcurrentBag<ISequentialIntServiceNode> nodes = new ConcurrentBag<ISequentialIntServiceNode>();

        private ConcurrentBag<ISequentialIntService> services = new ConcurrentBag<ISequentialIntService>();

        private Guid id;

        public Guid Id
        {
            get
            {
                return id;
            }
        }

        public ISequentialIntServiceNode GetServiceNode(int hash)
        {
            var requestedNode = nodes.FirstOrDefault(x => x.Intervals.Where(y => y.Start <= hash && y.End >= hash).ToList().Count == 1);

            if (requestedNode == null)
                Console.WriteLine("Can't find node for this hash: " + hash);

            return requestedNode;
        }

        public ConcurrentBag<ISequentialIntServiceNode> GetServiceNodes()
        {
            return new ConcurrentBag<ISequentialIntServiceNode>(this.nodes.Where(x => x.ParentNodeId == this.Id));
        }

        public int GetNextRevision(Guid gd, int revision)
        {
            var hash = gd.GetHashCode();

            ISequentialIntServiceNode node;

            node = nodes.FirstOrDefault(x => x.Intervals.Where(y => y.Start <= hash && y.End >= hash).ToList().Count == 1);

            if (node == null)
            {
                foreach (var service in services)
                {
                    var requestedNode = service.GetServiceNode(hash);

                    if (requestedNode != null)
                    {
                        node = requestedNode;

                        break;
                    }
                }
            }

            if (node == null)
            {
                Console.WriteLine("Can't find node for this hash: " + hash);

                return -1;
            }

            return node.GetNextRevision(gd, revision);
        }

        public void MakeAware(ISequentialIntService newService)
        {
            if (services.Contains(newService) || newService == null)
                return;

            if (newService != this)
            {
                services.Add(newService);

                newService.MakeAware(this);
            }

            foreach (var service in services)
            {
                newService.MakeAware(service);
                service.MakeAware(newService);
            }
        }

        public void AddServiceNode(ISequentialIntServiceNode node)
        {
            this.nodes.Add(node);
        }

        private ConcurrentBag<ISequentialIntServiceNode> GetAllNodesFromAllServices()
        {
            var allNodes = new ConcurrentBag<ISequentialIntServiceNode>();

            foreach (var node in this.nodes)
	        {
                allNodes.Add(node);
	        }

            foreach (var service in this.services)
            {
                foreach (var node in service.GetServiceNodes())
                {
                    allNodes.Add(node);
                }
            }

            return new ConcurrentBag<ISequentialIntServiceNode>(allNodes.Distinct());
        }

        private void DistributeTheNode(ISequentialIntServiceNode node)
        {
            var counter = 0;

            if (this.nodes.FirstOrDefault(x => x.Id == node.Id) != null)
                counter++;
            else
            {
                this.AddServiceNode(node);
                counter++;
            }

            foreach (var service in this.services)
            {
                var serviceNodes = service.GetServiceNodes();

                if (serviceNodes.FirstOrDefault(x => x.Id == node.Id) != null)
                    counter++;
            }

            var minimumServicesForNode = services.Count == 1 ? 2 : (services.Count / 2) + 1;

            while (counter < minimumServicesForNode)
            {
                this.services.FirstOrDefault(x => x.GetServiceNodes().FirstOrDefault(y => y.Id == node.Id) == null).AddServiceNode(node);

                counter++;
            }
        }
    }
}