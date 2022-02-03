using System.Collections.Generic;
using System.Linq;

namespace GeneralTools.PathFinding
{
	public class GraphNode
	{
		public object Target { get; }
		public string Name { get; }
		public List<Edge> Connections { get; } = new List<Edge>();

		public int? MinCostToStart { get; set; }
		public GraphNode NearestToStart { get; set; }
		public bool Visited { get; set; }
		public bool ClearingNow { get; set; }

		public GraphNode(object target)
		{
			Target = target;
			Name = target.ToString();
		}
		
		public GraphNode(string name = "node")
		{
			Name = name;
		}

		public void UpdateAllConnectionsState(bool skip)
		{
			foreach (var connection in Connections)
			{
				connection.Skip = skip;
				connection.ConnectedNode.UpdateConnectionsState(skip, this);
			}
		}

		public void UpdateConnectionsState(bool skip, GraphNode toNode)
		{
			foreach (var connection in Connections)
			{
				if (connection.ConnectedNode == toNode) connection.Skip = skip;
			}
		}

		public void ClearPrevSearchFlags()
		{
			ClearingNow = true;
			
			MinCostToStart = null;
			NearestToStart = null;
			Visited = false;

			foreach (var connection in Connections)
			{
				if (connection.ConnectedNode.ClearingNow) continue;
				connection.ConnectedNode.ClearPrevSearchFlags();
			}

			ClearingNow = false;
		}

//		public void AddConnection(GraphNode node) => AddConnection(node, 0);
//
//		public void AddConnections(params GraphNode[] connections)
//		{
//			foreach (var node in connections) AddConnection(node, 1);
//		}

		public void AddConnection(GraphNode node, int cost)
		{
			if (!HasConnectionTo(node))
			{
				Connections.Add(new Edge(node, cost));
			}

			if (!node.HasConnectionTo(this)) node.AddConnection(this, cost);
		}

		public void AddConnections(params (GraphNode, int)[] connections)
		{
			foreach (var c in connections) AddConnection(c.Item1, c.Item2);
		}

		private bool HasConnectionTo(GraphNode node) => Connections.Any(edge => edge.ConnectedNode == node);

		public override string ToString() => Name;
	}

	public class Edge
	{
		public int Cost { get; }
		public int Length { get; }
		public GraphNode ConnectedNode { get; }
		public bool Skip { get; set; }

		public Edge(GraphNode connectedNode, int cost = 0, int length = 0)
		{
			ConnectedNode = connectedNode;
			Cost = cost;
			Length = length;
		}

		public override string ToString() => $"-> {ConnectedNode}";
	}
}