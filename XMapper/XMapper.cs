using System.Linq.Expressions;
using System.Reflection;

namespace XMapper;

/// <summary>
/// Create a mapper that can be used for mappings from <typeparamref name="TSource"/> to <typeparamref name="TTarget"/>.
/// <para>Choose a <see cref="PropertyList"/>  (<see cref="PropertyList.Source"/> or <see cref="PropertyList.Target"/>): which properties do you want to map automatically by name? You'll probably choose the class that has fewer properties.</para>
/// <para>Use <see cref="IgnoreSourceProperty"/> or <see cref="IgnoreTargetProperty"/> to ignore properties from that list. You should ignore all properties that do not have a matching property name or type.</para>
/// <para>For ignored properties, you can do custom mappings via <see cref="IncludeAction"/> (both value type or reference type). In case of reference types, you may need to nest another <see cref="XMapper{TSource, TTarget}"/> here.</para>
/// <para>
/// Note 1: You can use fluent notation for all of this.<br />
/// Note 2: For better performance you may want to assign the mapper to a static field (so that you can re-use it). If you're also mapping reference type members, then also assign those mappers to static fields.
/// </para>
/// </summary>
public class XMapper<TSource, TTarget> where TSource : class, new() where TTarget : class, new()
{
    private readonly PropertyList _propertyList;
    private readonly List<PropertyInfo> _propertyInfos;
    private readonly List<Action<TSource, TTarget>> _includeActions = new();

    /// <summary>
    /// Create a mapper that can be used for mappings from <typeparamref name="TSource"/> to <typeparamref name="TTarget"/>.
    /// <para>Choose a <paramref name="initialPropertyList"/>  (<see cref="PropertyList.Source"/> or <see cref="PropertyList.Target"/>): which properties do you want to map automatically by name? You'll probably choose the class that has fewer properties.</para>
    /// <para>Use <see cref="IgnoreSourceProperty"/> or <see cref="IgnoreTargetProperty"/> to ignore properties from that list. You should ignore all properties that do not have a matching property name or type.</para>
    /// <para>For ignored properties, you can do custom mappings via <see cref="IncludeAction"/> (both value type or reference type). In case of reference types, you may need to nest another <see cref="XMapper{TSource, TTarget}"/> here.</para>
    /// <para>
    /// Note 1: You can use fluent notation for all of this.<br />
    /// Note 2: For better performance you may want to assign the mapper to a static field (so that you can re-use it). If you're also mapping reference type members, then also assign those mappers to static fields.
    /// </para>
    /// </summary>
    public XMapper(PropertyList initialPropertyList)
    {
        _propertyList = initialPropertyList;
        _propertyInfos = (_propertyList == PropertyList.Source ? typeof(TSource) : typeof(TTarget))
            .GetProperties()
            .ToList();
    }

    /// <summary>
    /// Only to use in case of <see cref="PropertyList.Source"/>. By default for all of <typeparamref name="TSource"/>'s properties an automatic mapping attempt is made (by equal name). If <typeparamref name="TTarget"/> does not have a matching property (name and type), you should ignore it here. You can also ignore a property and then include a custom mapping via <see cref="IncludeAction"/>.
    /// </summary>
    public XMapper<TSource, TTarget> IgnoreSourceProperty<TProp>(Expression<Func<TSource, TProp>> propertySelector)
    {
        if (_propertyList == PropertyList.Target)
        {
            throw new Exception($"Use '{nameof(IgnoreTargetProperty)}' if {nameof(PropertyList)} is {nameof(PropertyList.Target)}.");
        }
        _propertyInfos.Remove((PropertyInfo)((MemberExpression)propertySelector.Body).Member);
        return this;
    }

    /// <summary>
    /// Only to use in case of <see cref="PropertyList.Target"/>. By default for all of <typeparamref name="TTarget"/>'s properties an automatic mapping attempt is made (by equal name). If <typeparamref name="TSource"/> does not have a matching property (name and type), you should ignore it here. You can also ignore a property and then include a custom mapping via <see cref="IncludeAction"/>.
    /// </summary>
    public XMapper<TSource, TTarget> IgnoreTargetProperty<TProp>(Expression<Func<TTarget, TProp>> propertySelector)
    {
        if (_propertyList == PropertyList.Source)
        {
            throw new Exception($"Use '{nameof(IgnoreSourceProperty)}' if {nameof(PropertyList)} is {nameof(PropertyList.Source)}.");
        }
        _propertyInfos.Remove((PropertyInfo)((MemberExpression)propertySelector.Body).Member);
        return this;
    }

    /// <summary>
    /// For including custom mappings.
    /// <para>For properties that you don't want to be mapped automatically you can define a custom mapping here. Ignore the automatic mapping attempt via <see cref="IgnoreSourceProperty"/> or <see cref="IgnoreTargetProperty"/>.</para>
    /// <para>In case of reference type properties that have their own mapper: for better performance, initialize the member mapper outside of this method (assuming your intention is to re-use mappers for better execution times).</para>
    /// </summary>
    public XMapper<TSource, TTarget> IncludeAction(Action<TSource, TTarget> includeAction)
    {
        _includeActions.Add(includeAction);
        return this;
    }

    /// <summary>
    /// The Map method that constructs a new <typeparamref name="TTarget"/>.
    /// </summary>
    public TTarget Map(TSource source)
    {
        var target = new TTarget();
        Map(source, target);
        return target;
    }

    /// <summary>
    /// The Map method for an existing <typeparamref name="TTarget"/>.
    /// </summary>
    public void Map(TSource source, TTarget target)
    {
        if (target == null)
        {
            throw new Exception($"Argument '{nameof(target)}' in 'XMapper<{typeof(TSource).Name}, {typeof(TTarget).Name}>.Map(...)' should not be null.");
        }
        else if (source == null)
        {
            throw new Exception($"Argument '{nameof(source)}' in 'XMapper<{typeof(TSource).Name}, {typeof(TTarget).Name}>.Map(...)' should not be null.");
        }

        if (_propertyList == PropertyList.Source)
        {
            SourceListMap(source, target);
        }
        else
        {
            TargetListMap(source, target);
        }

        _includeActions.ForEach(action => action(source, target));
    }

    private void SourceListMap(TSource source, TTarget target)
    {
        var targetType = typeof(TTarget);
        foreach (var sourcePropertyInfo in _propertyInfos)
        {
            var targetPropertyInfo = targetType.GetProperty(sourcePropertyInfo.Name) ?? throw new Exception($"Property '{sourcePropertyInfo.Name}' was not found on {nameof(target)} '{typeof(TTarget).Name}'.");
            MapPropertyValue(source, target, sourcePropertyInfo, targetPropertyInfo);
        }
    }

    private void TargetListMap(TSource source, TTarget target)
    {
        var sourceType = typeof(TSource);
        foreach (var targetPropertyInfo in _propertyInfos)
        {
            var sourcePropertyInfo = sourceType.GetProperty(targetPropertyInfo.Name) ?? throw new Exception($"Property '{targetPropertyInfo.Name}' was not found on {nameof(source)} '{typeof(TSource).Name}'.");
            MapPropertyValue(source, target, sourcePropertyInfo, targetPropertyInfo);
        }
    }

    private static void MapPropertyValue(TSource source, TTarget target, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo)
    {
        var value = sourcePropertyInfo.GetValue(source);
        if (value == null && (
            targetPropertyInfo.PropertyType.IsValueType && Nullable.GetUnderlyingType(targetPropertyInfo.PropertyType) == null
            || new NullabilityInfoContext().Create(targetPropertyInfo).WriteState != NullabilityState.Nullable))
        {
            throw new Exception($"'{typeof(TSource).Name}.{sourcePropertyInfo.Name}' was null, but '{typeof(TTarget).Name}.{targetPropertyInfo.Name}' is not nullable.");
        }
        targetPropertyInfo.SetValue(target, value);
    }
}

/// <summary>
/// Choose an initial property list (<see cref="PropertyList.Source"/> or <see cref="PropertyList.Target"/>): which properties do you want to map by name? You'll probably want to choose the class that has fewer properties.
/// </summary>
public enum PropertyList
{
    /// <summary>
    /// Take the property list of the source object and automatically map from these properties.<br/>
    /// Optionally use <see cref="XMapper{TSource, TTarget}.IgnoreSourceProperty"/> and <see cref="XMapper{TSource, TTarget}.IncludeAction"/> for custom mapping.<br/>
    /// </summary>
    Source,
    /// <summary>
    /// Take the property list of the target object and automatically map to these properties.<br/>
    /// Optionally use <see cref="XMapper{TSource, TTarget}.IgnoreTargetProperty"/> and <see cref="XMapper{TSource, TTarget}.IncludeAction"/> for custom mapping.<br/>
    /// </summary>
    Target
}
