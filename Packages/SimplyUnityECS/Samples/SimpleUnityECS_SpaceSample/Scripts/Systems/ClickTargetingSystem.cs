using UnityEngine;

namespace RasofiaGames.SimpleUnityECS.Sample1
{
	public class ClickTargetingSystem : EntitySystem
	{
		public enum ClickID
		{
			GlobalClickID,
			MovementID
		}

		public enum MouseButton : int
		{
			Left = 0,
			Right = 1,
			Middle = 2
		}

		public const string FEATURE_TAG = "System_ClickTargeting";

		public const string TARGETING_ID = "ClickTargetingID";

		[SerializeField]
		private ClickID _targetingID = ClickID.GlobalClickID;

		[SerializeField]
		private MouseButton _mouseButton = MouseButton.Left;

		protected override FilterRules InitializeFilterRules(FilterRulesBuilder builder)
		{
			return builder.AddTagRule(FEATURE_TAG, TagFilterType.HasAnyTag)
							.AddHasComponentRule<TargetingComp>(true)
							.Result();
		}

		protected void Update()
		{
			if(Input.GetMouseButtonDown((int)_mouseButton))
			{
				Vector2 worldPosClicked = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Entities.ForEach(x => x.GetEntityComponent<TargetingComp>().SetTarget(ClickIDToTargetID(_targetingID), x.transform.position, worldPosClicked));
			}
		}

		private string ClickIDToTargetID(ClickID clickID)
		{
			switch(clickID)
			{
				case ClickID.MovementID:
					return TargetingGlobals.MOVEMENT_TARGETING_ID;
				default:
					return TARGETING_ID;
			}
		}
	}
}