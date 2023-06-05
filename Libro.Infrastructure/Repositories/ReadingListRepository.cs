using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Infrastructure.Repositories
{
    
    public class ReadingListRepository
    {
        private readonly ApplicationDbContext _context;

        public ReadingListRepository(ApplicationDbContext context)
        {
            _context = context;
        }
       /* public async Task<Role> CreateReadingList(ReadingList readingList)
        {
         //   _context
        }*/


    }
}
