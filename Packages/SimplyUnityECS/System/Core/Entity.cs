using RasofiaGames.SimpleUnityECS.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RasofiaGames.SimpleUnityECS
{
	public sealed class Entity : MonoBehaviour, IComponentLifecycle, IEntityLifecycle
	{
		// Entity
		public event Action<Entity, string> TagAddedEvent;
		public event Action<Entity, string> TagRemovedEvent;
		public event Action<Entity> EntityCreatedEvent;
		public event Action<Entity> EntityDestroyEvent;

		// Component
		public event Action<EntityComponent> AddedComponentEvent;
		public event Action<EntityComponent> RemovedComponentEvent;
		public event Action<EntityComponent> EnabledComponentEvent;
		public event Action<EntityComponent> DisabledComponentEvent;

		[SerializeField, HideInInspector]
		private List<string> _tags = new List<string>();

		private HashSet<EntityComponent> _components = new HashSet<EntityComponent>();

		public void AddTag(string tag)
		{
			if(!HasTag(tag))
			{
				_tags.Add(tag);
				if(TagAddedEvent != null)
				{
					TagAddedEvent(this, tag);
				}
			}
		}

		public string[] GetTags()
		{
			return _tags.ToArray();
		}

		public bool HasTag(string tag)
		{
			return _tags.Contains(tag);
		}

		public void RemoveTag(string tag)
		{
			if(HasTag(tag))
			{
				_tags.Remove(tag);
				if(TagRemovedEvent != null)
				{
					TagRemovedEvent(this, tag);
				}
			}
		}

		public bool HasAnyTag(params string[] tags)
		{
			if(tags == null || tags.Length == 0)
				return _tags.Count == 0;

			for(int i = 0, c = tags.Length; i < c; i++)
			{
				if(HasTag(tags[i]))
				{
					return true;
				}
			}

			return false;
		}

		public bool HasAllTags(params string[] tags)
		{
			if(tags == null || tags.Length == 0)
				return _tags.Count == 0;

			for(int i = 0, c = tags.Length; i < c; i++)
			{
				if(!HasTag(tags[i]))
				{
					return false;
				}
			}

			return true;
		}

		public bool HasEntityComponent<T>(bool incDisabledComponents = true) where T : EntityComponent
		{
			return HasEntityComponent(typeof(T), incDisabledComponents);
		}

		public bool HasEntityComponent(Type componentType, bool incDisabledComponents)
		{
			EntityComponent comp = GetEntityComponent(componentType);
			if(comp != null)
			{
				if(!incDisabledComponents && !comp.enabled)
				{
					return false;
				}

				return true;
			}
			return false;
		}

		public T GetEntityComponent<T>() where T : EntityComponent
		{
			return GetEntityComponent(typeof(T)) as T;
		}

		public EntityComponent GetEntityComponent(Type componentType)
		{
			foreach(EntityComponent component in _components)
			{
				if(componentType.IsAssignableFrom(component.GetType()))
				{
					return component;
				}
			}

			return null;
		}

		public void RemoveEntityComponent<T>() where T : EntityComponent
		{
			T comp = GetEntityComponent<T>();
			if(comp != null)
			{
				Destroy(comp);
			}
		}

		public void RegisterComponent(EntityComponent component)
		{
			if(!_components.Contains(component))
			{
				if(Array.IndexOf(gameObject.GetComponents(component.GetType()), component) >= 0)
				{
					_components.Add(component);
					component.AddedComponentEvent += OnAddedComponentEvent;
					component.RemovedComponentEvent += OnRemovedComponentEvent;
					component.EnabledComponentEvent += OnEnabledComponentEvent;
					component.DisabledComponentEvent += OnDisabledComponentEvent;
				}
				else
				{
					Debug.LogError($"Can't register component {component.GetType()} because it is not on the GameObject {gameObject.name}", gameObject);
				}
			}
		}

		public void UnregisterComponent(EntityComponent component)
		{
			if(_components.Contains(component))
			{
				_components.Remove(component);
				component.AddedComponentEvent -= OnAddedComponentEvent;
				component.RemovedComponentEvent -= OnRemovedComponentEvent;
				component.EnabledComponentEvent -= OnEnabledComponentEvent;
				component.DisabledComponentEvent -= OnDisabledComponentEvent;
				Destroy(component);
			}
		}

		private void Awake()
		{
			EntityTracker.Instance.RegisterEntity(this);
		}

		private void Start()
		{
			if(EntityCreatedEvent != null)
			{
				EntityCreatedEvent(this);
			}
		}

		private void OnDestroy()
		{
			if(EntityDestroyEvent != null)
			{
				EntityDestroyEvent(this);
			}

			EntityTracker.Instance.UnregisterEntity(this);
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
	}


	public interface IEntityLifecycle
	{
		event Action<Entity, string> TagAddedEvent;
		event Action<Entity, string> TagRemovedEvent;
		event Action<Entity> EntityCreatedEvent;
		event Action<Entity> EntityDestroyEvent;
	}
}