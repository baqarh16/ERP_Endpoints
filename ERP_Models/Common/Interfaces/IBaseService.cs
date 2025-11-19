using ERP_Models.Entities.Common.Responses;

namespace ERP_Models.Common.Interfaces
{
    public interface IBaseService<T> where T : class
    {
        Task<ApiResponse<T>> CreateAsync(T entity);
        Task<ApiResponse<T>> GetByIdAsync(int id);
        Task<ApiResponse<List<T>>> GetAllAsync();
        Task<ApiResponse<T>> UpdateAsync(T entity);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}