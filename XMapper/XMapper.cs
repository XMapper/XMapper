using System.Linq.Expressions;
using System.Reflection;

namespace XMapper;

public class XMapper<TSource, TTarget> where TTarget : class, new()
{
    private readonly PropertyList _propertyListToUse;
    public readonly List<PropertyInfo> _propertyInfos;
    private readonly List<Action<TSource, TTarget>> _includeActions = new();

    /// <summary>
    /// Create a mapper that can be used for mappings from <see cref="TSource"/> to <see cref="TTarget"/>.
    /// <para>Choose an <paramref name="initialPropertyList"/> (<see cref="PropertyList.Source"/> or <see cref="PropertyList.Target"/>): which <see cref="ValueType"/> and <see cref="string"/> properties do you want to map by name? You'll probably choose the class that has fewer properties.<br />
    /// Then use fluent notation for the next parts:<br />
    /// Use <see cref="IgnoreSourceProperty"/><br /> 
    /// or <see cref="IgnoreTargetProperty"/><br /> 
    /// - to ignore properties from that list or<br /> 
    /// - to do custom mappings via <see cref="IncludeAction"/>.<br /> 
    /// For including reference type properties in the mapping, also use <see cref="IncludeAction"/> with another <see cref="XMapper{TSource, TTarget}"/>.</para>
    /// </summary>
    /// <typeparam name="TSource">The object to get property values from.</typeparam>
    /// <typeparam name="TTarget">The object that needs its properties to be set.</typeparam>
    /// <param name="initialPropertyList"></param>
    public XMapper(PropertyList initialPropertyList)
    {
        _propertyListToUse = initialPropertyList;
        _propertyInfos = (_propertyListToUse == PropertyList.Source ? typeof(TSource) : typeof(TTarget))
            .GetProperties()
            .Where(x => x.PropertyType.IsValueType || x.PropertyType == typeof(string))
            .ToList();
    }

    /// <summary>
    /// Only to use in case of <see cref="PropertyList.Source"/>. By default for all of <see cref="TSource"/>'s <see cref="ValueType"/> and <see cref="string"/> properties an automatic mapping attempt is made (by equal name). If <see cref="TTarget"/> does not have a matching property, you should ignore it here. You can include a custom mapping via <see cref="IncludeAction"/>.
    /// </summary>
    public XMapper<TSource, TTarget> IgnoreSourceProperty<TProp>(Expression<Func<TSource, TProp>> propertySelector)
    {
        if (_propertyListToUse == PropertyList.Target)
        {
            throw new Exception($"Use {nameof(IgnoreTargetProperty)} if {nameof(PropertyList)} is {nameof(PropertyList.Target)}.");
        }
        _propertyInfos.Remove((PropertyInfo)((MemberExpression)propertySelector.Body).Member);
        return this;
    }

    /// <summary>
    /// Only to use in case of <see cref="PropertyList.Target"/>. By default for all of <see cref="TTarget"/>'s <see cref="ValueType"/> and <see cref="string"/> properties an automatic mapping attempt is made (by equal name). If <see cref="TSource"/> does not have a matching property, you should ignore it here. You can include a custom mapping via <see cref="IncludeAction"/>.
    /// </summary>
    public XMapper<TSource, TTarget> IgnoreTargetProperty<TProp>(Expression<Func<TTarget, TProp>> propertySelector)
    {
        if (_propertyListToUse == PropertyList.Source)
        {
            throw new Exception($"Use {nameof(IgnoreSourceProperty)} if {nameof(PropertyList)} is {nameof(PropertyList.Source)}.");
        }
        _propertyInfos.Remove((PropertyInfo)((MemberExpression)propertySelector.Body).Member);
        return this;
    }

    /// <summary>
    /// The Map method for an existing <see cref="TTarget"/>.
    /// </summary>
    public void Map(TSource source, TTarget target)
    {
        if (target == null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        if (_propertyListToUse == PropertyList.Source)
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
            targetPropertyInfo.SetValue(target, sourcePropertyInfo.GetValue(source));
        }
    }

    private void TargetListMap(TSource source, TTarget target)
    {
        var sourceType = typeof(TSource);
        foreach (var targetPropertyInfo in _propertyInfos)
        {
            var sourcePropertyInfo = sourceType.GetProperty(targetPropertyInfo.Name) ?? throw new Exception($"Property '{targetPropertyInfo.Name}' was not found on {nameof(source)}.");
            targetPropertyInfo.SetValue(target, sourcePropertyInfo.GetValue(source));
        }
    }

    /// <summary>
    /// The Map method that constructs a new <see cref="TTarget"/>.
    /// </summary>
    public TTarget Map(TSource source)
    {
        var target = new TTarget();
        Map(source, target);
        return target;
    }

    /// <summary>
    /// If (<see cref="ValueType"/> or <see cref="string"/>) properties should not be mapped automatically, you can define a custom mapping here. Ignore the automatic mapping attempt via <see cref="IgnoreSourceProperty"/> or <see cref="IgnoreTargetProperty"/>.
    /// </summary>
    public XMapper<TSource, TTarget> IncludeAction(Action<TSource, TTarget> includeAction)
    {
        _includeActions.Add(includeAction);
        return this;
    }
}

/// <summary>
/// <para>Choose an initial property list (<see cref="PropertyList.Source"/> or <see cref="PropertyList.Target"/>): which <see cref="ValueType"/> and <see cref="string"/> properties do you want to map by name? You'll probably want to choose the class that has fewer properties.<br />
/// </summary>
public enum PropertyList
{
    Source,
    Target
}
