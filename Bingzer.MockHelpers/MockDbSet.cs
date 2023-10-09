using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bingzer.MockHelpers
{
    public class MockDbSet<TEntity> : Mock<DbSet<TEntity>> where TEntity : class
    {
        private readonly ICollection<TEntity> _entities;

        public MockDbSet(ICollection<TEntity> entities)
        {
            _entities = entities;

            var queryable = _entities.AsQueryable();

            As<IQueryable<TEntity>>()
                .Setup(m => m.Provider)
                .Returns(queryable.Provider);
            As<IQueryable<TEntity>>()
                .Setup(m => m.Expression)
                .Returns(queryable.Expression);
            As<IQueryable<TEntity>>()
                .Setup(m => m.ElementType)
                .Returns(queryable.ElementType);
            As<IQueryable<TEntity>>()
                .Setup(m => m.GetEnumerator())
                .Returns(() => queryable.GetEnumerator());

            Setup(m => m.Add(It.IsAny<TEntity>()))
                .Callback<TEntity>((entity) => entities.Add(entity));
            Setup(m => m.Remove(It.IsAny<TEntity>()))
                .Callback<TEntity>((entity) => entities.Remove(entity));
            Setup(m => m.AddRange(It.IsAny<IEnumerable<TEntity>>()))
                .Callback<IEnumerable<TEntity>>(items =>
                {
                    foreach (var item in items)
                        entities.Add(item);
                });
            Setup(m => m.RemoveRange(It.IsAny<IEnumerable<TEntity>>()))
                .Callback<IEnumerable<TEntity>>((items) =>
                {
                    var itemsToRemove = items.ToList();
                    foreach (var item in itemsToRemove)
                    {
                        entities.Remove(item);
                    }
                });
        }
    }
}
