using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Infrastructure.Migrations;
using Libro.Application.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Infrastructure
{
    internal sealed class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync()
        {

            return await _context.SaveChangesAsync() ;
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();

        }
        public async Task CommitAsync(IDbContextTransaction contextTransaction)
        {

            await contextTransaction.CommitAsync();

        }
    }
}
