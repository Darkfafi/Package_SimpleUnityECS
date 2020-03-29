using System;
using System.Collections.Generic;

namespace RasofiaGames.SimpleUnityECS
{
	using RasofiaGames.SimpleUnityECS.Core;
	using static FilterRules;

	public class FilterRulesBuilder
	{
		private List<TagRule> _filterTags = new List<TagRule>();
		private List<IncComponentRule> _componentsToFilterOn = new List<IncComponentRule>();

		/// <summary>
		/// Creates a FilterRulesBuilder which creates a FilterRules with no tags to filter on (TagFilterType.None)
		/// </summary>
		/// <returns>FilterRulesBuilder</returns>
		public static FilterRulesBuilder SetupNoTagsBuilder()
		{
			return new FilterRulesBuilder(TagFilterType.HasAnyTag, new string[] { });
		}

		/// <summary>
		/// Creates a FilterRulesBuilder which creates a FilterRules which will filter getting elements which have the given tag on them.
		/// </summary>
		/// <returns>FilterRulesBuilder</returns>
		public static FilterRulesBuilder SetupHasTagBuilder(string tag)
		{
			return SetupHasAnyTagsBuilder(tag);
		}

		/// <summary>
		/// Creates a FilterRulesBuilder which creates a FilterRules which will filter getting elements which have ANY of the given tags (TagFilterType.HasAnyTag)
		/// </summary>
		/// <returns>FilterRulesBuilder</returns>
		public static FilterRulesBuilder SetupHasAnyTagsBuilder(string tag, params string[] tags)
		{
			List<string> myTags = new List<string>(tags);
			myTags.Add(tag);
			return new FilterRulesBuilder(TagFilterType.HasAnyTag, myTags.ToArray());
		}

		/// <summary>
		/// Creates a FilterRulesBuilder which creates a FilterRules which will filter getting elements which have ALL of the given tags (TagFilterType.HasAllTags)
		/// </summary>
		/// <returns>FilterRulesBuilder</returns>
		public static FilterRulesBuilder SetupHasAllTagsBuilder(string tag, params string[] tags)
		{
			List<string> myTags = new List<string>(tags);
			myTags.Add(tag);
			return new FilterRulesBuilder(TagFilterType.HasAllTags, myTags.ToArray());
		}

		/// <summary>
		/// Creates a FilterRulesBuilder which creates a FilterRules which will filter getting elements which have None of the given tags (TagFilterType.HasNoneOfTags)
		/// </summary>
		/// <returns>FilterRulesBuilder</returns>
		public static FilterRulesBuilder SetupHasNoneOfTagsBuilder(string tag, params string[] tags)
		{
			List<string> myTags = new List<string>(tags);
			myTags.Add(tag);
			return new FilterRulesBuilder(TagFilterType.HasNoneOfTags, myTags.ToArray());
		}
		/// <summary>
		/// Creates a FilterRulesBuilder which builds upon a FilterRules that is a copy of the given FilterRules
		/// </summary>
		/// <returns>FilterRulesBuilder</returns>
		public static FilterRulesBuilder SetupFromFilterRules(FilterRules filterRules)
		{
			return new FilterRulesBuilder(filterRules);
		}

		private FilterRulesBuilder(TagFilterType tagFilterType = TagFilterType.HasAnyTag, params string[] tags)
		{
			for(int i = 0; i < tags.Length; i++)
			{
				_filterTags.Add(new TagRule(tags[i], tagFilterType));
			}
		}

		private FilterRulesBuilder(FilterRules filterRules)
		{
			_filterTags = new List<TagRule>(filterRules.GetTagRules());
			_componentsToFilterOn = new List<IncComponentRule>(filterRules.GetIncComponentRules());
		}

		/// <summary>
		/// Adds a component type to the filter, so it will only get entries with the given component present.
		/// The filter will return entries which have ALL of the components given to it.
		/// </summary>
		public FilterRulesBuilder AddHasComponentRule<T>(bool mustBeEnabled) where T : EntityComponent
		{
			return AddHasComponentRule(typeof(T), mustBeEnabled);
		}

		/// <summary>
		/// Adds a component type to the filter, so it will only get entries with the given component present.
		/// The filter will return entries which have ALL of the components given to it.
		/// </summary>
		public FilterRulesBuilder AddHasComponentRule(Type entityComponentType, bool mustBeEnabled)
		{
			if(!typeof(EntityComponent).IsAssignableFrom(entityComponentType))
			{
				throw new InvalidCastException($"Can't add component rule because `{entityComponentType}` is not of type `{nameof(EntityComponent)}`");
			}

			IncComponentRule rule = new IncComponentRule(entityComponentType, mustBeEnabled);
			if(!_componentsToFilterOn.Contains(rule))
			{
				_componentsToFilterOn.Add(rule);
			}
			return this;
		}

		/// <summary>
		/// Adds a tag to the filter, so it will filter with the given tag associated with the given tag filter type 
		/// The filter will return entries which are valid to all Tag rules given to it.
		/// </summary>
		public FilterRulesBuilder AddTagRule(string tag, TagFilterType filterType)
		{
			TagRule rule = new TagRule(tag, filterType);
			if(!_filterTags.Contains(rule) && rule.Valid)
			{
				_filterTags.Add(rule);
			}
			return this;
		}

		/// <summary>
		/// Closes the creation of the filter and gives the constructed filter, using the static methods, into the out parameter
		/// </summary>
		/// <param name="filterCreated"> The constructed Filter </param>
		public FilterRules Result()
		{
			return new FilterRules(_filterTags.ToArray(), _componentsToFilterOn.ToArray());
		}
	}
}