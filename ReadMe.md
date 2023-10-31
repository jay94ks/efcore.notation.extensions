# Notation extender for EntityFramework Core.
This library adds support for Notation Attribute in EntityFramework Core.
It is also possible to create custom data types and add custom notations.
You don't need to learn anything else to use this library. 
All you need is to know a few Notations in addition to the standard features of 
EntityFramework Core and how to write custom Notations!

## Installation.
Visit nuget package gallery: https://www.nuget.org/packages/Efcore.Notation.Extensions

## A way to implement custom notation attribute.
Custom Notation attributes can be specified on both `Type` and `Property`. 
The previously mentioned `Type` is a data `Type`, not an `Entity`. 
This means that any manipulation of how a `Property` is stored, 
how it is structured in the database, etc. can be manipulated.


Below code is example notation for creating `Index` to database entity.
```
/// <summary>
/// Index attribute.
/// Marks a property as index.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public class IndexAttribute : NotationAttribute
{
    /// <summary>
    /// Name of index.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Order.
    /// </summary>
    public int Order { get; set; } = int.MaxValue / 2;

    /// <summary>
    /// Collection.
    /// </summary>
    private class Collection : HashSet<(string Name, int Order, string Property)>
    {

    }

    /// <inheritdoc/>
    protected override void OnConfigure<TEntity, TProperty>(NotationContext<TEntity, TProperty> Context)
    {
        Context.Items.TryGetValue(typeof(Collection), out var Temp);

        if (Temp is not Collection Collection)
        {
            Context.Items[typeof(Collection)] = Collection = new Collection();
            Context.LazyWorks.Enqueue(() =>
            {
                foreach(var Each in Collection.GroupBy(X => X.Name))
                {
                    var Properties = Each
                        .OrderBy(X => X.Order).Select(X => X.Property)
                        .ToArray();

                    Context.Entity.HasIndex(Properties, Each.Key).IsUnique(false);
                }
            });
        }

        var Name = this.Name;
        if (string.IsNullOrWhiteSpace(Name))
        {
            var Property = Context.PropertyInfo.Name.ToUpper();
            var Entity = Context.PropertyInfo.DeclaringType.Name.ToUpper();

            Name = $"IX_{Entity}_{Property}";
        }

        Collection.Add((Name, Order, Property: Context.PropertyInfo.Name));
    }
}
```

Usage of `IndexAttribute` is:
```
[Table("MyEntities")]
public class MyEntity
{
    /// <summary>
    /// GUID.
    /// </summary>
    [MultiKey(Order = 0)]
    public Guid Guid { get; set; }

    /// <summary>
    /// Number.
    /// </summary>
    [MultiKey(Order = 1), Index(Name = "MyIndex", Order = 1)]
    public int Number { get; set; }

    /// <summary>
    /// Remote Address.
    /// </summary>
    [Index(Name = "MyIndex", Order = 0)]
    public IPAddress RemoteAddress { get; set; }

    /// <summary>
    /// Text.
    /// </summary>
    [Column(TypeName = "LONGTEXT")]
    public string Text { get; set; }

    /// <summary>
    /// Password.
    /// </summary>
    public Password Password { get; set; }
}
```

As you can see above, it's very simple.

## A way to implement custom type conversion attribute.
This is an example of a conversion attribute that is always converted to 
JSON and saved when this Attribute is specified.
```
/// <summary>
/// Store the property as JSON string.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class StoreAsJsonAttribute : ConversionAttribute
{
    /// <inheritdoc/>
    protected override string Stringify<TProperty>(TProperty Property)
    {
        if (Property is null)
            return string.Empty;

        try { return JsonConvert.SerializeObject(Property, Formatting.None); }
        catch
        {
        }

        return string.Empty;
    }

    /// <inheritdoc/>
    protected override bool TryParse<TProperty>(string Input, out TProperty Property)
    {
        if (string.IsNullOrWhiteSpace(Input))
        {
            Property = default;
            return false;
        }

        try
        {
            Property = JsonConvert.DeserializeObject<TProperty>(Input);
            return true;
        }
        catch { }

        Property = default;
        return false;
    }

    /// <inheritdoc/>
    protected override void OnConfigureProperty<TEntity, TProperty>(NotationContext<TEntity, TProperty> Context)
    {
        base.OnConfigureProperty(Context);
        Context.Property.HasColumnType("LONGTEXT");
    }
}
```

You can even save custom `Type`s right away using this method! 
Take a look at how the `Password` structure is implemented in this library as example!