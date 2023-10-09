using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;

namespace Bingzer.MockHelpers
{
    public static class MockHelpers
    {

        /// <summary>
        /// Create Mock DbSet based on TEntity with collection backing store
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static DbSet<TEntity> CreateMockDbSet<TEntity>(ICollection<TEntity>? entities = null) where TEntity : class
        {
            entities ??= new List<TEntity>();

            return new MockDbSet<TEntity>(entities).Object;
        }
    }
}