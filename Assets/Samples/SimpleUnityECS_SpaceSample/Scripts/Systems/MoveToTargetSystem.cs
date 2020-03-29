using UnityEngine;

namespace RasofiaGames.SimpleUnityECS.Sample1
{
	public class MoveToTargetSystem : EntitySystem
	{
		public const string FEATURE_TAG = "System_Movement";

		protected override FilterRules InitializeFilterRules(FilterRulesBuilder builder)
		{
			return builder.AddTagRule(FEATURE_TAG, TagFilterType.HasAnyTag)
							.AddHasComponentRule<MovementComp>(true)
							.AddHasComponentRule<TargetingComp>(true)
							.AddHasComponentRule<TimelinesComp>(true)
							.Result();
		}

		protected void Update()
		{
			Entities.ForEach(entity => 
			{
				TargetingComp targetingComp = entity.GetEntityComponent<TargetingComp>();

				if(targetingComp.TryGetTarget(TargetingGlobals.MOVEMENT_TARGETING_ID, out TargetingComp.TargetData targetData))
				{
					TimelinesComp timelinesComp = entity.GetEntityComponent<TimelinesComp>();

					if(timelinesComp.TryEvaluateTime(TimelineGlobals.MOVEMENT_TIMELINE_ID, Time.deltaTime, out TimelinesComp.TimelineData timelineData))
					{
						MovementComp movementComp = entity.GetEntityComponent<MovementComp>();

						entity.transform.position = targetData.StartPos + movementComp.MovementCurve.Evaluate(timelineData.CurrentTime / timelineData.TotalTime) * targetData.Delta;
						entity.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.LerpAngle(entity.transform.rotation.eulerAngles.z, Mathf.Atan2(targetData.Delta.y, targetData.Delta.x) * Mathf.Rad2Deg + 90f, Time.deltaTime * movementComp.RotationSpeed));

						if(timelineData.HasReachedEnd)
						{
							timelinesComp.RemoveTimeline(timelineData.ID);
						}
					}
				}
			});
		}

		protected override void TrackedEntity(Entity entity)
		{
			TargetingComp targetingComp = entity.GetEntityComponent<TargetingComp>();
			targetingComp.TargetAddedEvent += OnTargetAddedEvent;
			targetingComp.TargetRemovedEvent += OnTargetRemovedEvent;
		}

		protected override void UntrackedEntity(Entity entity)
		{
			TargetingComp targetingComp = entity.GetEntityComponent<TargetingComp>();
			targetingComp.TargetAddedEvent -= OnTargetAddedEvent;
			targetingComp.TargetRemovedEvent -= OnTargetRemovedEvent;
			OnTargetRemovedEvent(targetingComp, targetingComp.GetTarget(TargetingGlobals.MOVEMENT_TARGETING_ID));
		}

		private void OnTargetAddedEvent(TargetingComp comp, TargetingComp.TargetData targetData)
		{
			if(targetData.ID == TargetingGlobals.MOVEMENT_TARGETING_ID)
			{
				comp.Parent.GetEntityComponent<TimelinesComp>().SetTimeline(TimelineGlobals.MOVEMENT_TIMELINE_ID, targetData.Delta.magnitude / comp.Parent.GetEntityComponent<MovementComp>().MovementSpeed);
			}
		}

		private void OnTargetRemovedEvent(TargetingComp comp, TargetingComp.TargetData targetData)
		{
			if(targetData.ID == TargetingGlobals.MOVEMENT_TARGETING_ID)
			{
				comp.Parent.GetEntityComponent<TimelinesComp>().RemoveTimeline(TimelineGlobals.MOVEMENT_TIMELINE_ID);
			}
		}
	}
}