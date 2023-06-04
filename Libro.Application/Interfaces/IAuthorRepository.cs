﻿using Libro.Domain.Entities;

namespace Libro.Application.Repositories
{
    public interface IAuthorRepository
    {
        Task CreateAuthorAsync(Author author);
    }
}