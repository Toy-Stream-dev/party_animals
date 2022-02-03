using System.Collections.Generic;
using System.Text;

namespace GeneralTools.PathFinding
{
	public interface INavigationNode<T>
	{
		GraphNode GraphNode { get; }
		void CreateGraphNode(bool onlyIfNeeded = true);
	}

	public static class NavigationLogic
	{
		public static List<GraphNode> GetPath(this GraphNode start, GraphNode end)
		{
			return GetPath(start, end, out _);
		}
		
		public static List<GraphNode> GetPath(this GraphNode start, GraphNode end, out int cost)
		{
			return DijkstraSearch.GetPath(start, end, out cost);
		}

		private const string PATH_CHAR = ">";

		public static string ToStr(this List<GraphNode> path)
		{
			var pathStr = new StringBuilder();

			for (var i = 0; i < path.Count; i++)
			{
				pathStr.Append(path[i].Name);
				if (i + 1 < path.Count) pathStr.Append(PATH_CHAR);
			}

			return pathStr.ToString();
		}
	}
}