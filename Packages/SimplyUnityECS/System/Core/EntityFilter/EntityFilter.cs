using RasofiaGames.SimpleUnityECS.Core;
using System.Collections.Generic;

namespace RasofiaGames.SimpleUnityECS
{
	public class EntityFilter : EntitiesHolder
	{
		public FilterRules FilterRules
		{
			get; private set;
		}

		#region Static Construction

		private static List<EntityFilter> _cachedFilters = new List<EntityFilter>();
		private static Dictionary<EntityFilter, int> _cachedFiltersReferenceCounter = new Dictionary<EntityFilter, int>();

		public static EntityFilter Create(TrackHandler trackedCallback, TrackHandler untrackedCallback, bool callForCurrentEntries = true)
		{
			return Create(FilterRules.CreateNoTagsFilter(), trackedCallback, untrackedCallback, callForCurrentEntries);
		}

		public static EntityFilter Create(FilterRules filterRules, TrackHandler trackedCallback, TrackHandler untrackedCallback, bool callForCurrentEntries = true)
		{
			void SetupFilter(EntityFilter filterToSetup)
			{
				filterToSetup.ListenToTrack(trackedCallback, untrackedCallback);
				if(callForCurrentEntries && trackedCallback != null)
				{
					filterToSetup.ForEach(x => trackedCallback(x));
				}
			}

			for(int i = _cachedFilters.Count - 1; i >= 0; i--)
			{
				EntityFilter filter = _cachedFilters[i];
				if(filter.FilterRules.Equals(filterRules))
				{
					AddReference(filter);
					SetupFilter(filter);
					return filter;
				}
			}

			EntityFilter self = new EntityFilter(filterRules);
			AddReference(self);
			_cachedFilters.Add(self);
			SetupFilter(self);
			return self;
		}

		private static void AddReference(EntityFilter instance)
		{
			if(HasReferences(instance))
			{
				_cachedFiltersReferenceCounter[instance]++;
			}
			else
			{
				_cachedFiltersReferenceCounter.Add(instance, 1);
			}
		}

		private static bool HasReferences(EntityFilter instance)
		{
			return _cachedFiltersReferenceCounter.ContainsKey(instance);
		}

		private static void RemoveReference(EntityFilter instance)
		{
			bool remove = false;
			if(HasReferences(instance))
			{
				_cachedFiltersReferenceCounter[instance]--;
				if(_cachedFiltersReferenceCounter[instance] == 0)
				{
					_cachedFiltersReferenceCounter.Remove(instance);
					remove = true;
				}
			}
			else
			{
				remove = true;
			}

			if(remove)
			{
				_cachedFilters.Remove(instance);
			}
		}

		#endregion

		private EntityFilter()
		{

		}

		private EntityFilter(FilterRules filter)
		{
			FilterRules = filter;
			EntityTracker.Instance.TagAddedEvent += OnEntityAddedTagEvent;
			EntityTracker.Instance.TagRemovedEvent += OnEntityRemovedTagEvent;
			EntityTracker.Instance.AddedComponentEvent += OnEntityAddedComponentEvent;
			EntityTracker.Instance.RemovedComponentEvent += OnEntityRemovedComponentEvent;
			EntityTracker.Instance.EnabledComponentEvent += OnEntityChangedEnabledStateOfComponentEvent;
			EntityTracker.Instance.DisabledComponentEvent += OnEntityChangedEnabledStateOfComponentEvent;
			EntityTracker.Instance.TrackedEvent += OnEntityTrackedEvent;
			EntityTracker.Instance.UntrackedEvent += OnEntityUntrackedEvent;
			FillWithAlreadyExistingMatches();
		}

		public void Clean(TrackHandler trackedCallback, TrackHandler untrackedCallback, bool callForCurrentEntries = true)
		{
			UnlistenFromTrack(trackedCallback, untrackedCallback);
			if(callForCurrentEntries && untrackedCallback != null)
			{
				ForEach(x => untrackedCallback(x));
			}
			RemoveReference(this);
			if(!HasReferences(this))
			{
				EntityTracker.Instance.TagAddedEvent -= OnEntityAddedTagEvent;
				EntityTracker.Instance.TagRemovedEvent -= OnEntityRemovedTagEvent;
				EntityTracker.Instance.AddedComponentEvent -= OnEntityAddedComponentEvent;
				EntityTracker.Instance.RemovedComponentEvent -= OnEntityRemovedComponentEvent;
				EntityTracker.Instance.EnabledComponentEvent -= OnEntityChangedEnabledStateOfComponentEvent;
				EntityTracker.Instance.DisabledComponentEvent -= OnEntityChangedEnabledStateOfComponentEvent;
				EntityTracker.Instance.TrackedEvent -= OnEntityTrackedEvent;
				EntityTracker.Instance.UntrackedEvent -= OnEntityUntrackedEvent;
				base.Clean();
			}
		}

		public bool Equals(EntityFilter filter)
		{
			return Equals(filter.FilterRules);
		}

		private void OnEntityAddedComponentEvent(EntityComponent component)
		{
			TrackLogics(component.Parent);
		}

		private void OnEntityRemovedComponentEvent(EntityComponent component)
		{
			TrackLogics(component.Parent);
		}

		private void OnEntityChangedEnabledStateOfComponentEvent(EntityComponent component)
		{
			TrackLogics(component.Parent);
		}

		private void OnEntityAddedTagEvent(Entity entity, string tag)
		{
			TrackLogics(entity);
		}

		private void OnEntityRemovedTagEvent(Entity entity, string tag)
		{
			TrackLogics(entity);
		}

		private void OnEntityTrackedEvent(Entity entity)
		{
			if(IsCleaned)
			{
				return;
			}

			if(entity != null && FilterRules.HasFilterPermission(entity))
			{
				Track(entity);
			}
		}

		private void OnEntityUntrackedEvent(Entity entity)
		{
			if(IsCleaned)
			{
				return;
			}

			if(entity != null)
			{
				Untrack(entity);
			}
		}

		private void FillWithAlreadyExistingMatches()
		{
			Entity[] t = EntityTracker.Instance.GetAll(FilterRules.HasFilterPermission);
			for(int i = 0; i < t.Length; i++)
			{
				Track(t[i]);
			}
		}

		private void TrackLogics(Entity entity)
		{
			if(IsCleaned)
			{
				return;
			}

			if(entity != null)
			{
				if(FilterRules.HasFilterPermission(entity))
				{
					Track(entity);
				}
				else
				{
					Untrack(entity);
				}
			}
		}
	}
}