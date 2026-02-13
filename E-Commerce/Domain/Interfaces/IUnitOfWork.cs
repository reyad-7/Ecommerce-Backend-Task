using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Models;

namespace Domain.Interfaces
{
        public interface IUnitOfWork : IDisposable
        {
            public IRepository<BaseUser> Users { get; }
            public IRepository<Order> Orders { get; }
            public IRepository<OrderItem> OrderItems { get; }
            public IRepository<Category> Categories { get; }
            public IRepository<Product> Products { get; }
        
            Task<int> SaveAsync();
        }

}
