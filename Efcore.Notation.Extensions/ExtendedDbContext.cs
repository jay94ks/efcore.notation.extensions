using Efcore.Notation.Extensions.Internals;
using Efcore.Notation.Extensions.Utilities;
using Efcore.Notation.Extensions.Utilities.Internals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace Efcore.Notation.Extensions
{
    /// <summary>
    /// Database context.
    /// </summary>
    public abstract class ExtendedDbContext : DbContext
    {
        /// <summary>
        /// Initialize a new <see cref="ExtendedDbContext"/> instance.
        /// </summary>
        public ExtendedDbContext()
        {
        }

        /// <summary>
        /// Initialize a new <see cref="ExtendedDbContext"/> instance.
        /// </summary>
        /// <param name="Options"></param>
        public ExtendedDbContext(DbContextOptions Options) : base(Options)
        {
        }

        /// <inheritdoc/>
        protected sealed override void OnModelCreating(ModelBuilder Models)
        {
            base.OnModelCreating(Models);

            var Args = new object[] { this, Models };
            var Types = new ExtendedDbEntitySet();

            ScanEntitiesFromDbSets(Types);
            OnModelCreating(Models, Types);

            foreach (var EachType in Types)
            {
                if (EachType.IsAbstract == true)
                    continue;

                if (EachType.IsClass == false)
                    continue;

                typeof(Proxy<>).MakeGenericType(EachType)
                    .GetConstructors().FirstOrDefault()
                    .Invoke(Args);
            }
        }

        /// <summary>
        /// Scan entity types from <see cref="DbSet{TEntity}"/> in the context.
        /// </summary>
        /// <param name="Types"></param>
        private void ScanEntitiesFromDbSets(ExtendedDbEntitySet Types)
        {
            var Builtins = GetType().GetProperties().Select(X => X.PropertyType)
                .Where(X => X.IsConstructedGenericType)
                .Where(X => X.GetGenericTypeDefinition() == typeof(DbSet<>))
                .Select(X => X.GetGenericArguments().First())
                .ToArray();

            foreach (var Each in Builtins)
                Types.Add(Each);
        }

        /// <summary>
        /// Called to configure <see cref="ModelBuilder"/> and collecting model types.
        /// </summary>
        /// <param name="Models"></param>
        /// <param name="Types"></param>
        protected abstract void OnModelCreating(ModelBuilder Models, ExtendedDbEntitySet Types);

        /// <summary>
        /// Call `<see cref="OnConfigureEntity{TEntity}(ModelBuilder, EntityTypeBuilder{TEntity})"/>` method.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        private class Proxy<TEntity> where TEntity : class
        {
            /// <summary>
            /// Call the `<see cref="OnConfigureEntity{TEntity}(ModelBuilder, EntityTypeBuilder{TEntity})"/>` method.
            /// </summary>
            /// <param name="Context"></param>
            /// <param name="Models"></param>
            public Proxy(ExtendedDbContext Context, ModelBuilder Models) => Context.OnConfigureEntity(Models, Models.Entity<TEntity>());
        }

        /// <summary>
        /// Configure the entity builder.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="Models"></param>
        /// <param name="Entity"></param>
        private void OnConfigureEntity<TEntity>(ModelBuilder Models, EntityTypeBuilder<TEntity> Entity) where TEntity : class
        {
            var Properties = typeof(TEntity).GetProperties()
                .Where(X => X.GetCustomAttribute<NotMappedAttribute>() is null)
                ;

            var Context = new ModelContext<TEntity>();
            var Args = new object[] { this, Context };
            foreach (var Property in Properties)
            {
                var Type = Property.PropertyType;

                Context.Models = Models;
                Context.Entity = Entity;
                Context.PropertyInfo = Property;

                typeof(Proxy<,>).MakeGenericType(typeof(TEntity), Type)
                    .GetConstructors().FirstOrDefault()
                    .Invoke(Args);
            }

            while (Context.LazyWorks.TryDequeue(out var Action))
                Action.Invoke();
        }

        /// <summary>
        /// Call <see cref="OnConfigureProperty{TEntity, TProperty}(ModelContext{TEntity})"/> method.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        private class Proxy<TEntity, TProperty> where TEntity : class
        {
            /// <summary>
            /// Call <see cref="OnConfigureProperty{TEntity, TProperty}(ModelContext{TEntity})"/> method.
            /// </summary>
            /// <param name="Database"></param>
            /// <param name="Context"></param>
            public Proxy(ExtendedDbContext Database, ModelContext<TEntity> Context)
            {
                Database.OnConfigureProperty<TEntity, TProperty>(Context);
            }
        }

        /// <summary>
        /// Configure the property of entity.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="Models"></param>
        private void OnConfigureProperty<TEntity, TProperty>(ModelContext<TEntity> Models) where TEntity : class
        {
            var Param = Expression.Parameter(typeof(TEntity));
            var Access = Expression.Property(Param, Models.PropertyInfo);
            var Lambda = Expression.Lambda<Func<TEntity, TProperty>>(Access, Param);
            var Context = new NotationContext<TEntity, TProperty>(
                Models, Models.Entity.Property(Lambda));

            ApplyPrenotatedTypes(Context);
            OnConfigureProperty(Context);

            ApplyTypeNotations(Context);
            ApplyNotations(Context);
        }

        /// <summary>
        /// Apply the notations.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="Context"></param>
        private void ApplyNotations<TEntity, TProperty>(NotationContext<TEntity, TProperty> Context) where TEntity : class
        {
            var Notations = Context.PropertyInfo
                .GetCustomAttributes<NotationAttribute>(true);

            if (Notations is null)
                return;

            foreach (var Notation in Notations)
            {
                Notation.Configure(Context);
            }
        }

        /// <summary>
        /// Apply the type notations.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="Context"></param>
        private void ApplyTypeNotations<TEntity, TProperty>(NotationContext<TEntity, TProperty> Context) where TEntity : class
        {
            var TypeNotations = Context.PropertyInfo.PropertyType
                .GetCustomAttributes<NotationAttribute>();

            if (TypeNotations is null)
                return;

            foreach (var Notation in TypeNotations)
                Notation.Configure(Context);
        }

        /// <summary>
        /// Configure the pre-notated types.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="Context"></param>
        private void ApplyPrenotatedTypes<TEntity, TProperty>(NotationContext<TEntity, TProperty> Context) where TEntity : class
        {
            var Action = PrenotationProvider.Get(typeof(TProperty));
            if (Action != null)
            {
                Action.Invoke(Context.Property);
                return;
            }

            if (typeof(TProperty).IsConstructedGenericType)
            {
                var Generic = typeof(TProperty).GetGenericTypeDefinition();
                if (Generic != typeof(SubTypeOf<>))
                    return;

                var SuperType = typeof(TProperty).GetGenericArguments().First();
                SubTypeConversion.Configure(SuperType, Context.Property);
                return;
            }

            if (typeof(TProperty).IsAssignableTo(typeof(IBinarySerializable)))
            {
                if (typeof(TProperty).IsAbstract)
                    return;

                BinarySerializables<TProperty>.Configure(Context.Property);
                return;
            }
        }


        /// <summary>
        /// Configure the property of entity.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="Context"></param>
        protected virtual void OnConfigureProperty<TEntity, TProperty>(NotationContext<TEntity, TProperty> Context) where TEntity : class
        {

        }
    }
}
