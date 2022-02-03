using System;
using System.Collections.Generic;
using GeneralTools.Tools;
using UnityEngine;

namespace _Idle.Scripts.Tools
{
	[CreateAssetMenu(fileName = "Distances", menuName = "_Idle/Distances", order = 0)]
	public class DistancesContainer : SingletonScriptableObject<DistancesContainer>
	{
		[Serializable]
		public class Distance
		{
			public int Id;
			public float Dist;
		}

		public List<Distance> Distances;
	}
}