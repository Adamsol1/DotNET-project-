using system;

namespace WebProject_3200.Application.Interfaces;

/*
Unit of Work Pattern : created to ensure data consistancy and
integrity by grouping database requests and operations into a units.
that can be commited, or disposed. if it affects the db in negative way we can dispose of the transactions.

Keeping track of objects/business trasactions that can affect the database.
Hepls optimize preformance.

*/


public interface IUnitOfWork : IDisposable
{
    // add the Repositories here. 

    //Methods that the unit of work has.

    // begin a transaction asyncronously.
    Task BeginAsync();

    //Save change to the db.
    Task SaveAsync()

    // commit the transaction.
    Task CommitAsync();

    // rollback a transaction. that will be disposed.
    Task RollBackAsync();
}