using System.Linq.Expressions;
using System.Reflection;

namespace XMapper;

public class XMapper<TSource, TTarget> where TTarget : new()
{
    private readonly UsePropertyListOf _propertyListToUse;
    public readonly List<PropertyInfo> _propertyInfos;
    private readonly List<Action<TSource, TTarget>> _memberMappingActions = new();

    /// <summary>
    /// Create a mapper that can be used for mappings from <see cref="TSource"/> to <see cref="TTarget"/>.
    /// <para>Choose an <paramref name="initialPropertyList"/> (<see cref="UsePropertyListOf.Source"/> or <see cref="UsePropertyListOf.Target"/>): which <see cref="ValueType"/> and <see cref="string"/> properties do you want to map by name? You'll probably want to choose the class that has fewer <see cref="ValueType"/> and <see cref="string"/> properties.<br />
    /// Then use fluent notation for the next parts:<br />
    /// Use <see cref="IgnoreSourceProperty"/><br /> 
    /// or <see cref="IgnoreTargetProperty"/><br /> 
    /// - to ignore properties from that list or<br /> 
    /// - to do custom mappings via <see cref="IncludeCustomAction"/>.<br /> 
    /// For including reference type properties in the mapping, use <see cref="IncludeReferenceTypeProperty"/>.</para>
    /// </summary>
    /// <typeparam name="TSource">The object to get property values from.</typeparam>
    /// <typeparam name="TTarget">The object that needs its properties to be set.</typeparam>
    /// <param name="initialPropertyList"></param>
    public XMapper(UsePropertyListOf initialPropertyList)
    {
        _propertyListToUse = initialPropertyList;
        _propertyInfos = (_propertyListToUse == UsePropertyListOf.Source ? typeof(TSource) : typeof(TTarget))
            .GetProperties()
            .Where(x => x.PropertyType.IsValueType || x.PropertyType == typeof(string))
            .ToList();
    }

    /// <summary>
    /// Only to use in case of <see cref="UsePropertyListOf.Source"/>. By default for all of <see cref="TSource"/>'s <see cref="ValueType"/> and <see cref="string"/> properties an automatic mapping attempt is made (by equal name). If <see cref="TTarget"/> does not have a matching property, you should ignore it here. You can include a custom mapping via <see cref="IncludeCustomAction"/>.
    /// </summary>
    public XMapper<TSource, TTarget> IgnoreSourceProperty<TProp>(Expression<Func<TSource, TProp>> propertySelector)
    {
        if (_propertyListToUse == UsePropertyListOf.Target)
        {
            throw new Exception($"Use {nameof(IgnoreTargetProperty)} if {nameof(UsePropertyListOf)} is {nameof(UsePropertyListOf.Target)}.");
        }
        _propertyInfos.Remove((PropertyInfo)((MemberExpression)propertySelector.Body).Member);
        return this;
    }

    /// <summary>
    /// Only to use in case of <see cref="UsePropertyListOf.Target"/>. By default for all of <see cref="TTarget"/>'s <see cref="ValueType"/> and <see cref="string"/> properties an automatic mapping attempt is made (by equal name). If <see cref="TSource"/> does not have a matching property, you should ignore it here. You can include a custom mapping via <see cref="IncludeCustomAction"/>.
    /// </summary>
    public XMapper<TSource, TTarget> IgnoreTargetProperty<TProp>(Expression<Func<TTarget, TProp>> propertySelector)
    {
        if (_propertyListToUse == UsePropertyListOf.Source)
        {
            throw new Exception($"Use {nameof(IgnoreSourceProperty)} if {nameof(UsePropertyListOf)} is {nameof(UsePropertyListOf.Source)}.");
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

        if (_propertyListToUse == UsePropertyListOf.Source)
        {
            SourceListMap(source, target);
        }
        else
        {
            TargetListMap(source, target);
        }

        _memberMappingActions.ForEach(action => action(source, target));
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
    /// Reference type properties are ignored by default, but you can include their <see cref="XMapper{TSourceProp, TTargetProp}"/> to have them mapped.
    /// </summary>
    public XMapper<TSource, TTarget> IncludeReferenceTypeProperty<TSourceProp, TTargetProp>(
        Expression<Func<TSource, TSourceProp>> sourcePropertySelector,
        Expression<Func<TTarget, TTargetProp>> targetPropertySelector,
        XMapper<TSourceProp, TTargetProp> referenceTypeMemberMapper) where TTargetProp : new()
    {
        var sourcePropertyInfo = (PropertyInfo)((MemberExpression)sourcePropertySelector.Body).Member;
        var targetPropertyInfo = (PropertyInfo)((MemberExpression)targetPropertySelector.Body).Member;
        _memberMappingActions.Add((source, target) =>
        {
            var sourcePropertyValue = (TSourceProp?)sourcePropertyInfo.GetValue(source);
            if (sourcePropertyValue == null)
            {
                targetPropertyInfo.SetValue(target, null);
            }
            else
            {
                var targetPropertyInstance = ((TTargetProp?)targetPropertyInfo.GetValue(target));
                if (targetPropertyInstance == null)
                {
                    targetPropertyInstance = new TTargetProp();
                    targetPropertyInfo.SetValue(target, targetPropertyInstance);
                }
                referenceTypeMemberMapper.Map(sourcePropertyValue, targetPropertyInstance);
            }
        });
        return this;
    }

    /// <summary>
    /// If (<see cref="ValueType"/> or <see cref="string"/>) properties should not be mapped automatically, you can define a custom mapping here (and ignore the automatic mapping attempt via <see cref="IgnoreSourceProperty"/> or <see cref="IgnoreTargetProperty"/>).
    /// </summary>
    public XMapper<TSource, TTarget> IncludeCustomAction(Action<TSource, TTarget> customIncludeAction)
    {
        _memberMappingActions.Add(customIncludeAction);
        return this;
    }
}

/// <summary>
/// Which properties do you want to map by name? You'll probably want to choose the class that has fewer <see cref="ValueType"/> and <see cref="string"/> properties. Then use <see cref="XMapper{TSource, TTarget}.IgnoreSourceProperty"/> or <see cref="XMapper{TSource, TTarget}.IgnoreTargetProperty"/> for properties that your don't want to map automatically (by name) from that PropertyList.
/// </summary>
public enum UsePropertyListOf
{
    Source,
    Target
}
