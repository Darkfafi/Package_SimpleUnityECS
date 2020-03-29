using System;

namespace RasofiaGames.SimpleUnityECS.Core
{
	public sealed class EntityTracker : EntitiesHolder, IComponentLifecycle, IEntityLifecycle
	{
		public event TrackHandler TrackedEvent;
		public event TrackHandler UntrackedEvent;

		// Entity
		public event Action<Entity, string> TagAddedEvent;
		public event Action<Entity, string> TagRemovedEvent;
		public event Action<Entity> EntityCreatedEvent;
		public event Action<Entity> EntityDestroyEvent;

		// Components
		public event Action<EntityComponent> AddedComponentEvent;
		public event Action<EntityComponent> RemovedComponentEvent;
		public event Action<EntityComponent> EnabledComponentEvent;
		public event Action<EntityComponent> DisabledComponentEvent;

		public static EntityTracker Instance
		{
			get
			{
				if(_instance == null)
				{
					_instance = new EntityTracker();
				}

				return _instance;
			}
		}

		private static EntityTracker _instance = null;

		private EntityTracker()
		{
			ListenToTrack(FireTrackedEvent, FireUntrackedEvent);
		}

		public void RegisterEntity(Entity entity)
		{
			if(IsCleaned)
			{
				return;
			}

			if(Track(entity))
			{
				entity.AddedComponentEvent += OnAddedComponentEvent;
				entity.RemovedComponentEvent += OnRemovedComponentEvent;
				entity.EnabledComponentEvent += OnEnabledComponentEvent;
				entity.DisabledComponentEvent += OnDisabledComponentEvent;

				entity.TagAddedEvent += OnTagAddedEvent;
				entity.TagRemovedEvent += OnTagRemovedEvent;
				entity.EntityCreatedEvent += OnEntityCreatedEvent;
				entity.EntityDestroyEvent += OnEntityDestroyEvent;
			}
		}

		public void UnregisterEntity(Entity entity)
		{
			if(IsCleaned)
			{
				return;
			}

			if(Untrack(entity))
			{
				entity.AddedComponentEvent -= OnAddedComponentEvent;
				entity.RemovedComponentEvent -= OnRemovedComponentEvent;
				entity.EnabledComponentEvent -= OnEnabledComponentEvent;
				entity.DisabledComponentEvent -= OnDisabledComponentEvent;

				entity.TagAddedEvent -= OnTagAddedEvent;
				entity.TagRemovedEvent -= OnTagRemovedEvent;
				entity.EntityCreatedEvent -= OnEntityCreatedEvent;
				entity.EntityDestroyEvent -= OnEntityDestroyEvent;
				UnityEngine.Object.Destroy(entity.gameObject);
			}
		}

		protected override void Clean()
		{
			base.Clean();
			UnlistenFromTrack(FireTrackedEvent, FireUntrackedEvent);
			TrackedEvent = null;
			UntrackedEvent = null;
			_instance = null;
		}

		private void FireTrackedEvent(Entity entity)
		{
			if(TrackedEvent != null)
			{
				TrackedEvent(entity);
			}
		}

		private void FireUntrackedEvent(Entity entity)
		{
			if(UntrackedEvent != null)
			{
				UntrackedEvent(entity);
			}
		}

		private void OnAddedComponentEvent(EntityComponent entityComponent)
		{
			AddedComponentEvent?.Invoke(entityComponent);
		}

		private void OnRemovedComponentEvent(EntityComponent entityComponent)
		{
			RemovedComponentEvent?.Invoke(entityComponent);
		}

		private void OnEnabledComponentEvent(EntityComponent entityComponent)
		{
			EnabledComponentEvent?.Invoke(entityComponent);
		}

		private void OnDisabledComponentEvent(EntityComponent entityComponent)
		{
			DisabledComponentEvent?.Invoke(entityComponent);
		}

		private void OnTagAddedEvent(Entity entity, string tag)
		{
			TagAddedEvent?.Invoke(entity, tag);
		}

		private void OnTagRemovedEvent(Entity entity, string tag)
		{
			TagRemovedEvent?.Invoke(entity, tag);
		}

		private void OnEntityCreatedEvent(Entity entity)
		{
			EntityCreatedEvent?.Invoke(entity);
		}

		private void OnEntityDestroyEvent(Entity entity)
		{
			EntityDestroyEvent?.Invoke(entity);
		}
	}
}