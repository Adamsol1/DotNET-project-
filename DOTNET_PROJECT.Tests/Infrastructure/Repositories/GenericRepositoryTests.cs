using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using DOTNET_PROJECT.Infrastructure.Data;
using DOTNET_PROJECT.Infrastructure.Repositories;

using DOTNET_PROJECT.Domain.Models;

namespace DOTNET_PROJECT.Tests.Infrastructure.Repositories;

public class GenericRepositoryTests
{
    private static AppDbContext CreateInMemoryContext(string dbName)
	{
		var options = new DbContextOptionsBuilder<AppDbContext>()
			.UseInMemoryDatabase(databaseName: dbName)
			.Options;

		return new AppDbContext(options);
	}

    // create entites for testing
    [Fact]
    public async Task Create_ShouldAddEntity()
    {
		using var context = CreateInMemoryContext(nameof(Create_ShouldAddEntity));
		var repo = new GenericRepository<Character>(context);

		var entity = new Character { Name = "Hero" };
		var created = await repo.Create(entity);

		Assert.NotNull(created);
		Assert.True(context.Set<Character>().Any(c => c.Name == "Hero"));
	}

    [Fact]
    public async Task GetAll_ShouldReturnAllEntities()
    {
        using var context = CreateInMemoryContext(nameof(GetAll_Returns_All));
		context.Set<Character>().AddRange(
			new Character { Name = "A" },
			new Character { Name = "B" }
		);
		
        await context.SaveChangesAsync();

		var repo = new GenericRepository<Character>(context);
		var all = await repo.GetAll();

		Assert.Equal(2, all.Count());
		Assert.Contains(all, e => e.Name == "A");
		Assert.Contains(all, e => e.Name == "B");
    }

    [Fact]
    public async Task GetById_Returns_Entity() {
        using var context = CreateInMemoryContext(nameof(GetById_Returns_Entity));
		
        var e = new Character { Name = "Mongo" };
		context.Set<Character>().Add(e);
		await context.SaveChangesAsync();

        var repo = new GenericRepository<Character>(context);
        var find = await repo.GetById(e.Id);

        Assert.NotNull(find);
		Assert.Equal("Mongo", find!.Name);
    }

    [Fact]
	public async Task Update_Updates_Entity()
	{
		using var context = CreateInMemoryContext(nameof(Update_Updates_Entity));
		var e = new Character { Name = "Old" };
		context.Set<Character>().Add(e);
		await context.SaveChangesAsync();

		var repo = new GenericRepository<Character>(context);
		e.Name = "New";
		var updated = await repo.Update(e);

		Assert.Equal("New", updated.Name);
        Assert.Equal("New", (await context.Set<Character>().FindAsync(e.Id)).Name);
	}

    [Fact]
    public async Task Delete_Removes_Entity()
    {
        using var context = CreateInMemoryContext(nameof(Delete_Removes_Entity));

        var e = new Character { Name = "MongoFjern" };
        context.Set<Character>().Add(e);
        await context.SaveChangesAsync();

        var repo = new GenericRepository<Character>(context);
        var deleted = await repo.Delete(e.Id);

        Assert.NotNull(deleted);
        Assert.Equal("MongoFjern", deleted!.Name);
        Assert.Null(await context.Set<Character>().FindAsync(e.Id));
    }

    [Fact]
    public async Task Delte_Returns_Null_When_Entity_Not_Found()
    {
        using var context = CreateInMemoryContext(nameof(Delete_Returns_Null_When_Entity_Not_Found));

        var repo = new GenericRepository<Character>(context);
        var deleted = await repo.Delete(99999);

        Assert.Null(deleted);
    }
}