using RasofiaGames.SimpleUnityECS.Core;
using System;
using System.Collections.Generic;

namespace RasofiaGames.SimpleUnityECS
{
	public enum TagFilterType
	{
		HasAnyTag,
		HasAllTags,
		HasNoneOfTags,
	}

	public struct FilterRules
	{
		private List<TagRule> _filterTags;
		private List<IncComponentRule> _componentsToFilterOn;

		/// <summary>
		/// Creates a FilterRules with no tags to filter on (TagFilterType.None)
		/// This FilterRules is not open to any static construction methods. 
		/// </summary>
		/// <returns>Default FilterRules of (TagFilterType.None)</returns>
		public static FilterRules CreateNoTagsFilter()
		{
			return new FilterRules(new string[] { }, TagFilterType.HasAnyTag);
		}

		/// <summary>
		/// Creates a FilterRules which will filter getting elements which have ANY of the given tags (TagFilterType.HasAnyTag)
		/// This FilterRules is not open to any static construction methods. 
		/// </summary>
		/// <returns>Default FilterRules of (TagFilterType.HasAnyTag)</returns>
		public static FilterRules CreateHasAnyTagsFilter(string tag, params string[] tags)
		{
			List<string> myTags = new List<string>(tags);
			myTags.Add(tag);
			return new FilterRules(myTags.ToArray(), TagFilterType.HasAnyTag);
		}

		/// <summary>
		/// Creates a FilterRules which will filter getting elements which have ALL of the given tags (TagFilterType.HasAllTags)
		/// This FilterRules is not open to any static construction methods. 
		/// </summary>
		/// <returns>Default FilterRules of (TagFilterType.HasAllTags)</returns>
		public static FilterRules CreateHasAllTagsFilter(string tag, params string[] tags)
		{
			List<string> myTags = new List<string>(tags);
			myTags.Add(tag);
			return new FilterRules(myTags.ToArray(), TagFilterType.HasAllTags);
		}

		/// <summary>
		/// Creates a FilterRules which will filter getting elements which have None of the given tags (TagFilterType.HasNoneOfTags)
		/// This FilterRules is not open to any static construction methods. 
		/// </summary>
		/// <returns>Default FilterRules of (TagFilterType.HasNoneOfTags)</returns>
		public static FilterRules CreateHasNoneOfTagsFilter(string tag, params string[] tags)
		{
			List<string> myTags = new List<string>(tags);
			myTags.Add(tag);
			return new FilterRules(myTags.ToArray(), TagFilterType.HasNoneOfTags);
		}

		public bool HasFilterPermission(Entity entity)
		{
			bool hasPermission = true;

			List<string> anyTagsToCheck = new List<string>();
			List<string> allTagsToCheck = new List<string>();
			List<string> noneTagsToCheck = new List<string>();

			for(int i = 0, c = _filterTags.Count; i < c; i++)
			{
				switch(_filterTags[i].TagFilterType)
				{
					case TagFilterType.HasAnyTag:
						anyTagsToCheck.Add(_filterTags[i].Tag);
						break;
					case TagFilterType.HasAllTags:
						allTagsToCheck.Add(_filterTags[i].Tag);
						break;
					case TagFilterType.HasNoneOfTags:
						noneTagsToCheck.Add(_filterTags[i].Tag);
						break;
				}
			}

			if(anyTagsToCheck.Count > 0)
			{
				hasPermission = entity.HasAnyTag(anyTagsToCheck.ToArray());
			}

			if(allTagsToCheck.Count > 0)
			{
				hasPermission = entity.HasAllTags(allTagsToCheck.ToArray());
			}

			if(noneTagsToCheck.Count > 0)
			{
				hasPermission = !entity.HasAnyTag(noneTagsToCheck.ToArray());
			}

			if(!hasPermission)
			{
				return false;
			}

			for(int i = 0, c = _componentsToFilterOn.Count; i < c; i++)
			{
				if(!entity.HasEntityComponent(_componentsToFilterOn[i].ComponentType, !_componentsToFilterOn[i].MustBeEnabled))
				{
					return false;
				}
			}

			return true;
		}

		public bool Equals(FilterRules filter)
		{
			if(_filterTags.Count == filter._filterTags.Count && _componentsToFilterOn.Count == filter._componentsToFilterOn.Count)
			{
				for(int i = 0, c = _filterTags.Count; i < c; i++)
				{
					TagRule ownRule = _filterTags[i];
					if(filter._filterTags.FindIndex(fc => ownRule.IsEqual(fc)) < 0)
					{
						return false;
					}
				}

				for(int i = 0, c = _componentsToFilterOn.Count; i < c; i++)
				{
					IncComponentRule ownRule = _componentsToFilterOn[i];
					if(filter._componentsToFilterOn.FindIndex(fc => ownRule.IsEqual(fc)) < 0)
					{
						return false;
					}
				}

				return true;
			}

			return false;
		}

		public TagRule[] GetTagRules()
		{
			return _filterTags.ToArray();
		}

		public IncComponentRule[] GetIncComponentRules()
		{
			return _componentsToFilterOn.ToArray();
		}

		private FilterRules(string[] tags, TagFilterType tagFilterType)
		{
			_filterTags = new List<TagRule>();
			_componentsToFilterOn = new List<IncComponentRule>();

			for(int i = 0; i < tags.Length; i++)
			{
				_filterTags.Add(new TagRule(tags[i], tagFilterType));
			}
		}

		public FilterRules(TagRule[] tagRules, IncComponentRule[] incComponentRules)
		{
			_filterTags = new List<TagRule>(tagRules);
			_componentsToFilterOn = new List<IncComponentRule>(incComponentRules);
		}

		public struct IncComponentRule
		{
			public Type ComponentType
			{
				get; private set;
			}

			public bool MustBeEnabled
			{
				get; private set;
			}

			public bool Valid
			{
				get
				{
					return ComponentType != null;
				}
			}

			public IncComponentRule(Type componentType, bool mustBeEnabled)
			{
				ComponentType = componentType;
				MustBeEnabled = mustBeEnabled;
			}

			public bool IsEqual(IncComponentRule otherRule)
			{
				return ComponentType == otherRule.ComponentType && MustBeEnabled == otherRule.MustBeEnabled;
			}
		}

		public struct TagRule
		{
			public string Tag
			{
				get; private set;
			}

			public TagFilterType TagFilterType
			{
				get; private set;
			}

			public bool Valid
			{
				get
				{
					return !string.IsNullOrEmpty(Tag);
				}
			}

			public TagRule(string tag, TagFilterType filterType)
			{
				Tag = tag;
				TagFilterType = filterType;
			}

			public bool IsEqual(TagRule otherRule)
			{
				return Tag == otherRule.Tag && TagFilterType == otherRule.TagFilterType;
			}
		}
	}
}