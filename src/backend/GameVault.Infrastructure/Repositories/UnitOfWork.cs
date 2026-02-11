using GameVault.Core.Entities;
using GameVault.Core.Interfaces;
using GameVault.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace GameVault.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public IRepository<Category> Categories { get; }
    public IRepository<Product> Products { get; }
    public IRepository<ProductVariant> ProductVariants { get; }
    public IRepository<ProductImage> ProductImages { get; }
    public IRepository<ProductSpec> ProductSpecs { get; }
    public IRepository<ProductTag> ProductTags { get; }
    public IRepository<User> Users { get; }
    public IRepository<Comment> Comments { get; }
    public IRepository<Order> Orders { get; }
    public IRepository<OrderItem> OrderItems { get; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;

        Categories = new Repository<Category>(context);
        Products = new Repository<Product>(context);
        ProductVariants = new Repository<ProductVariant>(context);
        ProductImages = new Repository<ProductImage>(context);
        ProductSpecs = new Repository<ProductSpec>(context);
        ProductTags = new Repository<ProductTag>(context);
        Users = new Repository<User>(context);
        Comments = new Repository<Comment>(context);
        Orders = new Repository<Order>(context);
        OrderItems = new Repository<OrderItem>(context);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await SaveChangesAsync();
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
