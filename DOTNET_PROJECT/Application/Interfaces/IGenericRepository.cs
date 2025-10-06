namespace DOTNET_PROJECT.Application.Interfaces;

/*
    Generic Repository interface,
    Has all the Basic crud operations that all repositories share

*/

public interface IGenericRepository<T> where T : class
{

    // get By Id Asyncrounusly
    Task<T> GetById(int id);

    // get All Asyncrounusly
    Task<IEnumerable<T>> GetAll();

    // create Async
    Task<T> Create(T entity);

    Task<T> Update(T entity);

    Task<T> Delete(int id);

}