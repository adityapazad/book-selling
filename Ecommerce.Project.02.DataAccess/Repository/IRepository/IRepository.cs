﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Project._02.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        void Update(T entity);
        void Remove (T entity);
        void Remove (int id);
        void RemoveRange(IEnumerable<T> entities);
        T Get (int id);
        IEnumerable<T> GetAll (
                Expression<Func<T, bool>> filter = null,
                Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                string includeProperties = null
            );
        T FirstOrDefault(
                Expression<Func<T, bool>> filter = null,
                string includeProperties = null
            );
    }
}
