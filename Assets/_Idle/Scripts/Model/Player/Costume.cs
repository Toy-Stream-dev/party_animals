using System;
using System.Collections.Generic;
using System.Linq;
using GeneralTools;

namespace _Idle.Scripts.Model.Player
{
	public class Costume : BaseBehaviour
	{
		public int CostumeId;

		private List<CostumePart> _parts;
		
		private void Awake()
		{
			_parts = GetComponentsInChildren<CostumePart>(true).ToList();
		}

		public void EnablePart(CostumePartType type)
		{
			var part = _parts.FirstOrDefault(x => x.PartType.Equals(type));
			
			if (part == null)
				return;
			
			part.gameObject.SetActive(true);
		}

		public void DisablePart(CostumePartType type)
		{
			var part = _parts.FirstOrDefault(x => x.PartType.Equals(type));
			
			if (part == null)
				return;
			
			part.gameObject.SetActive(false);
		}

		public void DisableCostume()
		{
			foreach (var part in _parts)
			{
				part.gameObject.SetActive(part.IsActive);
			}
		}
	}
}