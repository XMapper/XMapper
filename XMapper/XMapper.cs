﻿using System.Linq.Expressions;
using System.Reflection;

namespace XMapper;

/// <summary>
/// Create an instance of this class to create a mapper. See the constructor for more guidance.
/// </summary>
public class XMapper<TSource, TTarget> where TSource : class, new() where TTarget : class, new()
{
    private readonly PropertyList _propertyList;
    private readonly List<PropertyInfo> _propertyInfos;
    private readonly List<Action<TSource, TTarget>> _includeActions = new();

    /// <summary>
    /// Create a mapper that can be used for mappings from <typeparamref name="TSource"/> to <typeparamref name="TTarget"/>.
    /// <para>Choose an <paramref name="initialPropertyList"/> (<see cref="PropertyList.Source"/> or <see cref="PropertyList.Target"/>): which <see cref="ValueType"/> and <see cref="string"/> properties do you want to map by name? You'll probably choose the class that has fewer properties.<br />
    /// Then use fluent notation for the next parts:<br />
    /// Use <see cref="IgnoreSourceProperty"/><br /> 
    /// or <see cref="IgnoreTargetProperty"/><br /> 
    /// - to ignore properties from that list or<br /> 
    /// - to do custom mappings via <see cref="IncludeAction"/>.<br /> 
    /// For including reference type properties in the mapping, also use <see cref="IncludeAction"/> with another <see cref="XMapper{TSource, TTarget}"/>.</para>
    /// </summary>
    public XMapper(PropertyList initialPropertyList)
    {
        _propertyList = initialPropertyList;
        _propertyInfos = (_propertyList == PropertyList.Source ? typeof(TSource) : typeof(TTarget))
            .GetProperties()
            .Where(x => x.PropertyType.IsValueType || x.PropertyType == typeof(string))
            .ToList();
    }

    /// <summary>
    /// Only to use in case of <see cref="PropertyList.Source"/>. By default for all of <typeparamref name="TSource"/>'s <see cref="ValueType"/> and <see cref="string"/> properties an automatic mapping attempt is made (by equal name). If <typeparamref name="TTarget"/> does not have a matching property, you should ignore it here. You can include a custom mapping via <see cref="IncludeAction"/>.
    /// </summary>
    public XMapper<TSource, TTarget> IgnoreSourceProperty<TProp>(Expression<Func<TSource, TProp>> propertySelector)
    {
        if (_propertyList == PropertyList.Target)
        {
            throw new Exception($"Use {nameof(IgnoreTargetProperty)} if {nameof(PropertyList)} is {nameof(PropertyList.Target)}.");
        }
        _propertyInfos.Remove((PropertyInfo)((MemberExpression)propertySelector.Body).Member);
        return this;
    }

    /// <summary>
    /// Only to use in case of <see cref="PropertyList.Target"/>. By default for all of <typeparamref name="TTarget"/>'s <see cref="ValueType"/> and <see cref="string"/> properties an automatic mapping attempt is made (by equal name). If <typeparamref name="TSource"/> does not have a matching property, you should ignore it here. You can include a custom mapping via <see cref="IncludeAction"/>.
    /// </summary>
    public XMapper<TSource, TTarget> IgnoreTargetProperty<TProp>(Expression<Func<TTarget, TProp>> propertySelector)
    {
        if (_propertyList == PropertyList.Source)
        {
            throw new Exception($"Use {nameof(IgnoreSourceProperty)} if {nameof(PropertyList)} is {nameof(PropertyList.Source)}.");
        }
        _propertyInfos.Remove((PropertyInfo)((MemberExpression)propertySelector.Body).Member);
        return this;
    }

    /// <summary>
    /// The Map method for an existing <typeparamref name="TTarget"/>.
    /// </summary>
    public void Map(TSource source, TTarget target)
    {
        if (target == null)
        {
            throw new ArgumentNullException(nameof(target));
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
            var targetPropertyInfo = targetType.GetProperty(sourcePropertyInfo.Name) ?? throw new Exception($"Property '{sourcePropertyInfo.Name}' was not found on {nameof(target)}.");
            var value = sourcePropertyInfo.GetValue(source);
            if (value == null && targetPropertyInfo.PropertyType.IsValueType && Nullable.GetUnderlyingType(targetPropertyInfo.PropertyType) == null)
            {
                throw new Exception($"'{typeof(TSource).Name}.{sourcePropertyInfo.Name}' was null, but '{typeof(TTarget).Name}.{targetPropertyInfo.Name}' is not nullable.");
            }
            targetPropertyInfo.SetValue(target, value);
        }
    }

    private void TargetListMap(TSource source, TTarget target)
    {
        var sourceType = typeof(TSource);
        foreach (var targetPropertyInfo in _propertyInfos)
        {
            var sourcePropertyInfo = sourceType.GetProperty(targetPropertyInfo.Name) ?? throw new Exception($"Property '{targetPropertyInfo.Name}' was not found on {nameof(source)}.");
            var value = sourcePropertyInfo.GetValue(source);
            if (value == null && targetPropertyInfo.PropertyType.IsValueType && Nullable.GetUnderlyingType(targetPropertyInfo.PropertyType) == null)
            {
                throw new Exception($"'{typeof(TSource).Name}.{sourcePropertyInfo.Name}' was null, but '{typeof(TTarget).Name}.{targetPropertyInfo.Name}' is not nullable.");
            }
            targetPropertyInfo.SetValue(target, value);
        }
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
    /// - If (<see cref="ValueType"/> or <see cref="string"/>) properties should not be mapped automatically, you can define a custom mapping here. Ignore the automatic mapping attempt via <see cref="IgnoreSourceProperty"/> or <see cref="IgnoreTargetProperty"/>.<br/>
    /// - For including reference type properties in the mapping, your can also use this method. For better performance, initialize the member mapper outside of this method (assuming your intention is to re-use mappers for better execution times).
    /// </summary>
    public XMapper<TSource, TTarget> IncludeAction(Action<TSource, TTarget> includeAction)
    {
        _includeActions.Add(includeAction);
        return this;
    }
}

/// <summary>
/// Choose an initial property list (<see cref="PropertyList.Source"/> or <see cref="PropertyList.Target"/>): which <see cref="ValueType"/> and <see cref="string"/> properties do you want to map by name? You'll probably want to choose the class that has fewer properties.
/// </summary>
public enum PropertyList
{
    /// <summary>
    /// Take the property list of the source object and automatically map from these properties.<br/>
    /// Optionally use <see cref="XMapper{TSource, TTarget}.IgnoreSourceProperty"/> and <see cref="XMapper{TSource, TTarget}.IncludeAction"/> for custom mapping.<br/>
    /// Note that only <see cref="ValueType"/> and <see cref="string"/> property types can be mapped automatically by name.<br/>
    /// For including reference type properties, also use <see cref="XMapper{TSource, TTarget}.IncludeAction"/> but then for a call to <see cref="XMapper{TSource, TTarget}.Map(TSource, TTarget)"/>.
    /// </summary>
    Source,
    /// <summary>
    /// Take the property list of the target object and automatically map to these properties.<br/>
    /// Optionally use <see cref="XMapper{TSource, TTarget}.IgnoreTargetProperty"/> and <see cref="XMapper{TSource, TTarget}.IncludeAction"/> for custom mapping.<br/>
    /// Note that only <see cref="ValueType"/> and <see cref="string"/> property types can be mapped automatically by name.<br/>
    /// For including reference type properties, also use <see cref="XMapper{TSource, TTarget}.IncludeAction"/> but then for a call to <see cref="XMapper{TSource, TTarget}.Map(TSource, TTarget)"/>.
    /// </summary>
    Target
}
