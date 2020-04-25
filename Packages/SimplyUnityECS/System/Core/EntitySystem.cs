using UnityEngine;

namespace RasofiaGames.SimpleUnityECS
{
	public abstract class EntitySystem : MonoBehaviour
	{
		public EntityFilter Entities
		{
			get; private set;
		}

		protected void Awake()
		{
			Entities = EntityFilter.Create(InitializeFilterRules(FilterRulesBuilder.SetupNoTagsBuilder()), TrackedEntity, UntrackedEntity);
		}

		protected void OnDestroy()
		{
			Entities.Clean(TrackedEntity, UntrackedEntity);
			Entities = null;
		}

		protected virtual void TrackedEntity(Entity entity)
		{

		}

		protected virtual void UntrackedEntity(Entity entity)
		{

		}

		protected abstract FilterRules InitializeFilterRules(FilterRulesBuilder builder);
	}
}