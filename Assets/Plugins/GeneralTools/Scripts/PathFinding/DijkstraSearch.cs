using System.Collections.Generic;
using System.Linq;

//Dijkstra algorithm
//https://www.codeproject.com/Articles/1221034/Pathfinding-Algorithms-in-Csharp
namespace GeneralTools.PathFinding
{
	public static class DijkstraSearch
	{
		private static GraphNode _start,
		                         _end;
		
		private static int _nodeVisits,
		                   _shortestPathLength,
		                   _shortestPathCost;

		public static List<GraphNode> GetPath(GraphNode start, GraphNode end)
		{
			return GetPath(start, end, out _);
		}

		public static List<GraphNode> GetPath(GraphNode start, GraphNode end, out int cost)
		{
			_start = start;
			_end = end;

			_nodeVisits = _shortestPathLength = _shortestPathCost = 0;
			
			_start.ClearPrevSearchFlags();
			
			DoDijkstraSearch();
			
			var shortestPath = new List<GraphNode> {_end};
			
			BuildShortestPath(shortestPath, _end);

			if (shortestPath.Count == 1)
			{
				shortestPath.Clear();
			}
			else
			{
				shortestPath.Reverse();
			}

			cost = _shortestPathCost;
			
			return shortestPath;
		}

		private static void BuildShortestPath(List<GraphNode> list, GraphNode node)
		{
			if (node.NearestToStart == null) return;
			
			list.Add(node.NearestToStart);
			
			_shortestPathLength += node.Connections.Single(x => x.ConnectedNode == node.NearestToStart).Length;
			_shortestPathCost += node.Connections.Single(x => x.ConnectedNode == node.NearestToStart).Cost;
			
			BuildShortestPath(list, node.NearestToStart);
		}

		private static void DoDijkstraSearch()
		{
			_nodeVisits = 0;
			_start.MinCostToStart = 0;
			var priorQueue = new List<GraphNode> {_start};
			do
			{
				_nodeVisits++;
				priorQueue = priorQueue.OrderBy(x => x.MinCostToStart.Value).ToList();
				var node = priorQueue.First();
				priorQueue.Remove(node);
				foreach (var cnn in node.Connections.OrderBy(x => x.Cost))
				{
					if (cnn.Skip) continue;
					
					var childNode = cnn.ConnectedNode;
					
					if (childNode.Visited) continue;
					
					if (childNode.MinCostToStart == null ||
					    node.MinCostToStart + cnn.Cost < childNode.MinCostToStart)
					{
						childNode.MinCostToStart = node.MinCostToStart + cnn.Cost;
						childNode.NearestToStart = node;
						if (!priorQueue.Contains(childNode)) priorQueue.Add(childNode);
					}
				}

				node.Visited = true;
				if (node == _end) return;
			} while (priorQueue.Any());
		}
	}
}
