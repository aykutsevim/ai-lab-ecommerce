using GameVault.Core.Entities;

namespace GameVault.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<Category> Categories { get; }
    IRepository<Product> Products { get; }
    IRepository<ProductVariant> ProductVariants { get; }
    IRepository<ProductImage> ProductImages { get; }
    IRepository<ProductSpec> ProductSpecs { get; }
    IRepository<ProductTag> ProductTags { get; }
    IRepository<User> Users { get; }
    IRepository<Comment> Comments { get; }
    IRepository<Order> Orders { get; }
    IRepository<OrderItem> OrderItems { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
