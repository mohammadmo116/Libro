﻿using Libro.Domain.Entities;

namespace Libro.Infrastructure.Repositories
{
    public interface IReadingListRepository
    {
        Task CreateReadingListAsync(ReadingList readingList);
        Task<(ReadingList, int)> GetReadingListWithBooksAsync(Guid ReadingListId, int PageNumber, int Count);
    }
}