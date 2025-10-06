using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using DOTNET_PROJECT.Application.Interfaces;
using DOTNET_PROJECT.Application.Exceptions;


namespace DOTNET_PROJECT.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    // set the database context
    private readonly AppDbContext _db;
    // set the db set
    private readonly DbSet<T> _dbSet;

    // constructor
    public GenericRepository(AppDbContext db)
    {
        _db = db;
        _dbSet = db.Set<T>();
    }
    
    // get by Id

    public async Task<T> GetById(int id) 
    {
        // find the entity by id
        return await _dbSet.FindAsync(id);
    }

    // get all
    public async Task<IEnumerable<T>> GetAll()
    {
        // get all the entities
        return await _dbSet.AsNoTracking()
            .ToListAsync();
    }

    // create new entity
    public async Task<T> Create(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    // update entity
    public async Task<T> Update(T entity)
    {
        // attach the entity to the db set
        _dbSet.Attach(entity);
        // set the state to modified
        _db.Entry(entity).State = EntityState.Modified;

        // update the entity
        await _db.SaveChangesAsync();

        // return the entity ? or a message?
        return entity;
    }


    // delete entity
    public async Task<T> Delete(int id)
    {
        // find the entity by id
        var entity = await _dbSet.FindAsync(id);

        // if entity is not found, return null
        if (entity == null) return null;

        // else remove the entity
        _dbSet.Remove(entity);

        // save changes
        await _db.SaveChangesAsync();

        // return the entity
        return entity;
    }
    
    
}