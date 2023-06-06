﻿using Libro.Application.Interfaces;
using Libro.Application.Roles.Commands;
using Libro.Application.Users.Queries;
using Libro.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using static System.Reflection.Metadata.BlobBuilder;


namespace Libro.Application.Books.Queries
{
    public sealed class GetSearchedBooksQueryHandler : IRequestHandler<GetSearchedBooksQuery, (List<Book>, int)>
    {
        private readonly ILogger<GetSearchedBooksQueryHandler> _logger;
        private readonly IBookRepository _bookRepository;
        public GetSearchedBooksQueryHandler(ILogger<GetSearchedBooksQueryHandler> logger,
            IBookRepository bookRepository)
        {
            _logger = logger;
            _bookRepository = bookRepository;

        }
  

        public async Task<(List<Book>,int)> Handle(GetSearchedBooksQuery request, CancellationToken cancellationToken)
        {
            if (request.Title is null && request.AuthorName is null && request.Genre is null)
                return await _bookRepository.GetAllBooksAsync(request.PageNumber, request.Count);

               List <Book> Books= null;
            //TODO
            if (request.Title is not null)
                Books= await _bookRepository.GetBooksByTitleAsync(Books,request.Title);

            if (request.AuthorName is not null)
                Books =  await _bookRepository.GetBooksByAuthorNameAsync(Books, request.AuthorName);
            
            if (request.Genre is not null)
                Books =  await _bookRepository.GetBooksByGenreAsync(Books, request.Genre);
           
            Books ??= new();


            var NumberOfPages = 1;
            if (Books.Count > 0)
                NumberOfPages = (int)Math.Ceiling((double)Books.Count / request.Count);

            return (
                Books.Skip(request.PageNumber * request.Count).Take(request.Count).ToList()
                , NumberOfPages
                );
           
        }
    }
}
