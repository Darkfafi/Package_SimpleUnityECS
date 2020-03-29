using RasofiaGames.SimpleUnityECS.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RasofiaGames.SimpleUnityECS.Sample1
{
	public class TimelinesComp : EntityComponent
	{
		public event Action<TimelinesComp, TimelineData> TimelineAddedEvent;
		public event Action<TimelinesComp, TimelineData> TimelineRemovedEvent;

		private Dictionary<string, TimelineData> _timelines = new Dictionary<string, TimelineData>();

		public bool HasTimeline(string id)
		{
			return _timelines.ContainsKey(id);
		}

		public bool TryGetTimeline(string id, out TimelineData data)
		{
			return _timelines.TryGetValue(id, out data);
		}

		public TimelineData GetTimeline(string id)
		{
			if(TryGetTimeline(id, out TimelineData data))
			{
				return data;
			}
			return default;
		}

		public void SetTimeline(string id, float totalTime)
		{
			RemoveTimeline(id);
			TimelineData data = new TimelineData(id, totalTime);
			_timelines[id] = data;
			TimelineAddedEvent?.Invoke(this, data);
		}

		public void RemoveTimeline(string id)
		{
			if(_timelines.TryGetValue(id, out TimelineData data))
			{
				_timelines.Remove(id);
				TimelineRemovedEvent?.Invoke(this, data);
			}
		}

		public bool TryEvaluateTime(string id, float deltaTime, out TimelineData data)
		{
			if(_timelines.TryGetValue(id, out data))
			{
				data.EvaluateTime(deltaTime);
				_timelines[id] = data;
				return true;
			}
			return false;
		}

		public struct TimelineData
		{
			public string ID;
			public float TotalTime;
			public float CurrentTime;

			public bool HasReachedEnd => Mathf.Approximately(CurrentTime, TotalTime);

			public TimelineData(string id, float totalTime)
			{
				ID = id;
				TotalTime = totalTime;
				CurrentTime = 0f;
			}

			public void EvaluateTime(float deltaTime)
			{
				CurrentTime = Mathf.Clamp(CurrentTime + deltaTime, 0f, TotalTime);
			}
		}
	}
}