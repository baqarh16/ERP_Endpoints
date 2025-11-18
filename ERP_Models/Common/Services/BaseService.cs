using ERP_Models.Common.Interfaces;
using ERP_Models.Entities;
using ERP_Models.Entities.Common.Responses;
using Microsoft.EntityFrameworkCore;

namespace ERP_Models.Common.Services
{
    public class BaseService<T> : IBaseService<T> where T : AuditableEntity
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseService(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<ApiResponse<T>> CreateAsync(T entity)
        {
            // CreatedAt & CreatedBy are already set in DbContext.SaveChanges()
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
            return ApiResponse<T>.Ok(entity, "Created successfully");
        }

        public virtual async Task<ApiResponse<T>> GetByIdAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);

            if (entity == null || entity.IsDeleted)
                return ApiResponse<T>.Fail("Entity not found");

            return ApiResponse<T>.Ok(entity);
        }

        public virtual async Task<ApiResponse<List<T>>> GetAllAsync()
        {
            var data = await _dbSet.ToListAsync(); // Soft-delete filter already applied globally
            return ApiResponse<List<T>>.Ok(data);
        }

        public virtual async Task<ApiResponse<T>> UpdateAsync(T entity)
        {
            if (entity == null)
                return ApiResponse<T>.Fail("Entity cannot be null");

            // UpdatedAt & UpdatedBy are automatically set in BaseDbContext.SaveChanges()
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return ApiResponse<T>.Ok(entity, "Updated successfully");
        }

        public virtual async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);

            if (entity == null || entity.IsDeleted)
                return ApiResponse<bool>.Fail("Entity not found");

            // Soft delete – DbContext will set DeletedAt/DeletedBy automatically
            entity.IsDeleted = true;
            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true, "Deleted successfully (soft delete)");
        }
    }
}