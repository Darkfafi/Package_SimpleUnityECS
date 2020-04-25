using System;
using System.Collections.Generic;

namespace RasofiaGames.SimpleUnityECS.Core
{
	public abstract class EntitiesHolder
	{
		public delegate void TrackHandler(Entity entity);
		private TrackHandler _trackedCallbacks;
		private TrackHandler _untrackedCallbacks;

		public bool IsCleaned
		{
			get
			{
				return _entities == null;
			}
		}

		private List<Entity> _entities = new List<Entity>();

		// -- Entity Query Methods -- \\

		// - Single Entity - \\

		public Entity GetRandom(Func<Entity, bool> filterCondition)
		{
			Random r = new Random();
			Entity[] e = GetAll(filterCondition);
			if(e.Length > 0)
				return e[r.Next(0, e.Length)];

			return null;
		}

		public Entity GetRandom()
		{
			return GetRandom(null);
		}

		public Entity GetFirst(Comparison<Entity> sort = null)
		{
			return GetFirst(null, sort);
		}

		public Entity GetFirst(Func<Entity, bool> filterCondition, Comparison<Entity> sort = null)
		{
			Entity[] e = GetAll(filterCondition, sort);
			if(e.Length > 0)
				return e[0];

			return null;
		}

		// - Multiple Entities - \\

		public void ForEach(Action<Entity> method, Comparison<Entity> sort = null)
		{
			ForEach(method, null, sort);
		}

		public void ForEach(Action<Entity> method, Func<Entity, bool> filterCondition, Comparison<Entity> sort = null)
		{
			Entity[] all = GetAll(filterCondition, sort);
			for(int i = 0, c = all.Length; i < c; i++)
			{
				method(all[i]);
			}
		}

		public void ForEachReverse(Action<Entity> method, Comparison<Entity> sort = null)
		{
			ForEachReverse(method, null, sort);
		}

		public void ForEachReverse(Action<Entity> method, Func<Entity, bool> filterCondition, Comparison<Entity> sort = null)
		{
			Entity[] all = GetAll(filterCondition, sort);
			for(int i = all.Length - 1; i >= 0; i--)
			{
				method(all[i]);
			}
		}

		public Entity[] GetAll(Comparison<Entity> sort = null)
		{
			if(sort == null)
				return _entities.ToArray();
			else
				return GetAll(null, sort);
		}

		public Entity[] GetAll(Func<Entity, bool> filterCondition, Comparison<Entity> sort = null)
		{
			List<Entity> result = new List<Entity>();
			if(_entities != null)
			{
				for(int i = 0, count = _entities.Count; i < count; i++)
				{
					Entity e = _entities[i];
					if(e != null && (filterCondition == null || filterCondition(e)))
					{
						result.Add(e);
					}
				}
			}

			if(sort != null)
				result.Sort(sort);

			return result.ToArray();
		}

		public bool Has(Entity model)
		{
			return _entities.Contains(model);
		}

		protected void ListenToTrack(TrackHandler trackedCallback, TrackHandler untrackedCallback)
		{
			if(trackedCallback != null)
			{
				_trackedCallbacks += trackedCallback;
			}

			if(untrackedCallback != null)
			{
				_untrackedCallbacks += untrackedCallback;
			}
		}

		protected void UnlistenFromTrack(TrackHandler trackedCallback, TrackHandler untrackedCallback)
		{
			if(trackedCallback != null)
			{
				_trackedCallbacks -= trackedCallback;
			}

			if(untrackedCallback != null)
			{
				_untrackedCallbacks -= untrackedCallback;
			}
		}

		protected virtual void Clean()
		{
			for(int i = _entities.Count - 1; i >= 0; i--)
			{
				Untrack(_entities[i]);
			}

			_trackedCallbacks = null;
			_untrackedCallbacks = null;

			_entities.Clear();
			_entities = null;
		}

		// Internal tracking

		protected bool Track(Entity model)
		{
			if(_entities.Contains(model))
				return false;

			_entities.Add(model);

			if(_trackedCallbacks != null)
			{
				_trackedCallbacks(model);
			}

			return true;
		}

		protected bool Untrack(Entity model)
		{
			if(!_entities.Contains(model))
				return false;

			_entities.Remove(model);

			if(_untrackedCallbacks != null)
			{
				_untrackedCallbacks(model);
			}

			return true;
		}
	}
}