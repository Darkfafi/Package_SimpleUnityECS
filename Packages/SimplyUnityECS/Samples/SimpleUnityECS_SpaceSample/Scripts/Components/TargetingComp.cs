using RasofiaGames.SimpleUnityECS.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RasofiaGames.SimpleUnityECS.Sample1
{
	public class TargetingComp : EntityComponent
	{
		public event Action<TargetingComp, TargetData> TargetAddedEvent;
		public event Action<TargetingComp, TargetData> TargetRemovedEvent;

		private Dictionary<string, TargetData> _targets = new Dictionary<string, TargetData>();

		public bool HasTarget(string id)
		{
			return _targets.ContainsKey(id);
		}

		public bool TryGetTarget(string id, out TargetData targetingData)
		{
			return _targets.TryGetValue(id, out targetingData);
		}

		public TargetData GetTarget(string id)
		{
			if(TryGetTarget(id, out TargetData data))
			{
				return data;
			}
			return default;
		}

		public void SetTarget(string id, Vector2 startPos, Vector2 target)
		{
			RemoveTarget(id);
			TargetData targetData = new TargetData(id, startPos, target);
			_targets[id] = targetData;
			TargetAddedEvent?.Invoke(this, targetData);
		}

		public void RemoveTarget(string id)
		{
			if(_targets.TryGetValue(id, out TargetData targetData))
			{
				_targets.Remove(id);
				TargetRemovedEvent?.Invoke(this, targetData);
			}
		}

		public struct TargetData
		{
			public string ID;
			public Vector2 StartPos;
			public Vector2 Target;
			public Vector2 Delta;

			public bool IsValid
			{
				get;
			}

			public TargetData(string id, Vector2 startPos, Vector2 target)
			{
				ID = id;
				StartPos = startPos;
				Target = target;
				Delta = target - startPos;
				IsValid = true;
			}
		}
	}
}