using City_Bus_Management_System.DataLayer.Data;
using Core_Layer.IRepositries;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositries
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        public AppDbContext _context { get; set; }
        public BaseRepository(AppDbContext context)
        {
            this._context = context;
        }
        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }

        public void DeleteAsync(T entity)
            => _context.Update(entity);

        public T Find(Expression<Func<T, bool>> criteria, string[] includes = null)
        {
            var entity = _context.Set<T>()
                 .AsNoTracking();
            if (includes != null)
                foreach (var include in includes)
                      entity = entity.Include(include);

            return entity.FirstOrDefault(criteria)!;
        }

        public IQueryable<T> FindAll(Expression<Func<T, bool>> criteria, string[] includes = null)
        {
            var entity = _context.Set<T>()
                 .AsNoTracking();

            if (includes != null)
                foreach (var include in includes)
                    entity = entity.Include(include);

            return entity.Where(criteria)!;
        }

        public IQueryable<TDto> FindAll<TDto>(Expression<Func<T, bool>> criteria, string[] includes)
        {
            var entity = _context.Set<T>()
                 .AsNoTracking();

            if (includes != null)   
                foreach (var include in includes)
                    entity = entity.Include(include);

            return entity.Where(criteria).ProjectToType<TDto>()!;
        }

        public IQueryable<TDto> GetAll<TDto>()
              => _context.Set<T>().AsNoTracking().ProjectToType<TDto>();


        public T GetById(int id)
           => _context.Set<T>().Find(id)!;

        public T GetByUserId(string id)
           => _context.Set<T>().Find(id)!;

        public Task<T> UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            return Task.FromResult(entity);
        }

        public TDto Find<TDto>(Expression<Func<T, bool>> criteria, string[] includes = null)
        {
            var entity = _context.Set<T>()
                 .AsNoTracking();
            if (includes != null)
                foreach (var include in includes)
                entity = entity.Include(include);

            return entity.Where(criteria).ProjectToType<TDto>().FirstOrDefault()!;
        }
    }
}
