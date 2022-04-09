using System.Linq.Expressions;
using System.Reflection;

namespace XMapper;

public class XMapper<TSource, TTarget> where TTarget : new()
{
    private readonly UsePropertyListOf _propertyListToUse;
    public readonly List<PropertyInfo> _propertyInfos;
    private readonly List<Action<TSource, TTarget>> _memberMappingActions = new();
    public XMapper(UsePropertyListOf initialPropertyList)
    {
        _propertyListToUse = initialPropertyList;
        _propertyInfos = (_propertyListToUse == UsePropertyListOf.Source ? typeof(TSource) : typeof(TTarget))
            .GetProperties()
            .Where(x => x.PropertyType.IsValueType || x.PropertyType == typeof(string))
            .ToList();
    }

    /// <summary>
    /// Select a property that you don't want to map. Note: only to use in case of <see cref="UsePropertyListOf.Source"/>.
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
    /// Select a property that you don't want to map. Note: only to use in case of <see cref="UsePropertyListOf.Target"/>.
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

    public TTarget Map(TSource source)
    {
        var target = new TTarget();
        Map(source, target);
        return target;
    }

    public XMapper<TSource, TTarget> RegisterPropertyMapper<TSourceProp, TTargetProp>(
        Expression<Func<TSource, TSourceProp>> sourcePropertySelector,
        Expression<Func<TTarget, TTargetProp>> targetPropertySelector,
        XMapper<TSourceProp, TTargetProp> memberMapper) where TTargetProp : new()
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
                memberMapper.Map(sourcePropertyValue, targetPropertyInstance);
            }
        });
        return this;
    }
}

/// <summary>
/// You'll probably want to choose the class that has fewer <see cref="ValueType"/> and <see cref="string"/> properties: <see cref="{TSource}"/> or <see cref="{TTarget}"/>? Then use one of the Ignore methods for properties that your don't want to map automatically from that PropertyList."/>
/// </summary>
public enum UsePropertyListOf
{
    Source,
    Target
}
