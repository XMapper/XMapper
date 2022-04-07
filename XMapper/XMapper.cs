using System.Linq.Expressions;
using System.Reflection;

namespace XMapper;

public class XMapper<TSource, TTarget> where TTarget : new()
{
    public readonly List<PropertyInfo> SourceProperties;

    public XMapper()
    {
        SourceProperties = typeof(TSource).GetProperties().Where(x => x.PropertyType.IsValueType || x.PropertyType == typeof(string)).ToList();
    }
    public XMapper<TSource, TTarget> Ignore<TProp>(Expression<Func<TSource, TProp>> propertySelector)
    {
        SourceProperties.Remove((PropertyInfo)((MemberExpression)propertySelector.Body).Member);
        return this;
    }

    public void Map(TSource source, TTarget target)
    {
        var targetType = typeof(TTarget);
        foreach(var sourcePropertyInfo in SourceProperties)
        {
            var targetPropertyInfo = targetType.GetProperty(sourcePropertyInfo.Name) ?? throw new Exception($"Property '{sourcePropertyInfo.Name}' was not found on {nameof(target)}.");
            targetPropertyInfo.SetValue(target, sourcePropertyInfo.GetValue(source));
        }
    }

    public TTarget Map(TSource source)
    {
        var target = new TTarget();
        Map(source, target);
        return target;
    }
}
