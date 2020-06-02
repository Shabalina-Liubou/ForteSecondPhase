using System;
using System.Linq;
using System.Linq.Expressions;

namespace DemoOybek.Data
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Where(Expression<Func<T, bool>> predicate);
        IQueryable<T> GetAll();
        void Insert(T entry);
        int SaveChanges();

        string HashPassword(string password);
    }
}
