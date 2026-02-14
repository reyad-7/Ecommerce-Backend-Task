using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Models;
using Domain.Interfaces.IRepository;
using Domain.Interfaces.IunitOfWork;
using Infrastructure.Data;

namespace Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private WaffarXEcommerceDBContext _context;

        public UnitOfWork(WaffarXEcommerceDBContext context)
        {
            _context = context;
            Users = new Repository<BaseUser>(_context);
            Orders = new Repository<Order>(_context);
            OrderItems = new Repository<OrderItem>(_context);
            Categories = new Repository<Category>(_context);
            Products = new Repository<Product>(_context);

        }

        public IRepository<BaseUser> Users { get; }
        public IRepository<Order> Orders { get; }
        public IRepository<OrderItem> OrderItems { get; } 
        public IRepository<Category> Categories { get; } 
        public IRepository<Product> Products { get; } 

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

    }
}
