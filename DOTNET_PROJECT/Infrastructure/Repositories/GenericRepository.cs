using System.Collections.Generic;
using System.Threading.Task;
using Microsoft.EntityFrameworkCore;
using DOTNET_PROJECT.Application.Interfaces;

namespace DOTNET_PROJECT.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    // set the database context
    private readonly AppDbContext _dbContext;
    // set the db set
    private readonly DbSet<T> _dbSet;

    // constructor
    public GenericRepository(AppDbContext dbContext)
	{
		_dbContext = dbContext;
		_dbSet = _dbContext.Set<T>();
	}
    
    // get by Id

    public async Task<T> GetById(int id)
    {
        // get the entity by id
        var entity = await _dbSet.FindAsync(id);

        // if no entity then error
        if (entity == null) throw new KeyNotFoundException($"Entity of type {typeof(T).Name} with id {id} was not found.");

        // return the entity
        return entity;
    }

    // get all
    public async Task<IEnumerable<T>> GetAll()
    {
        // get all the entities
        return await _dbSet.AsNoTracking().ToListAsync();

    }

    // create new entity
    public async Task<T> Create(T entity)
    {
        await _dbSet.AddAsync(entity);
		await _dbContext.SaveChangesAsync();
		return entity;
    }

    // update entity
    public async Task<T> Update(T entity)
    {
        // attach the entity to the db set
        _dbSet.Attach(entity);
        // set the state to modified
        _dbContext.Entry(entity).State = EntityState.Modified;

        // update the entity
        await _dbContext.SaveChangesAsync();

        // return the entity ? or a message?
        return entity;
    }


    // delete entity
    public async Task<T> Delete(int id)
    {
        // find the entity by id
        var entity = await _dbSet.FindAsync(id);

        // if entity is not found, throw error
        if (entity == null) throw new KeyNotFoundException($"Entity of type {typeof(T).Name} with id {id} was not found.");

        // else remove the entity
        _dbSet.Remove(entity);

        // save changes
        await _dbContext.SaveChangesAsync();

        // return the entity
        return entity;
    }
    
    
}