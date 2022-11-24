using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using OptimizeQueryWithBenchmark.Context;
using OptimizeQueryWithBenchmark.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OptimizeQueryWithBenchmark.Services
{
    [MemoryDiagnoser]
    public class BenchmarkService
    {

        [Benchmark]
        public List<AuthorDto> GetAuthor()
        {
            using var dbContext = new AppDbContext();

            var authors = dbContext.Authors
                                           .Include(x => x.User)
                                           .ThenInclude(x => x.UserRoles)
                                           .ThenInclude(x => x.Role)
                                           .Include(x => x.Books)
                                           .ThenInclude(x => x.Publisher)
                                           .ToList()
                                           .Select( x => new AuthorDto 
                                           { 
                                             UserCreated = x.User.Created,
                                             UserEmailConfirmed = x.User.EmailConfirmed,
                                             UserFirstName = x.User.FirstName,
                                             UserLastActivity = x.User.LastActivity,
                                             UserLastName = x.User.LastName,
                                             UserEmail = x.User.Email,
                                             UserName = x.User.UserName,
                                             UserId = x.User.Id,
                                             RoleId = x.User.UserRoles.FirstOrDefault(y => y.UserId == x.UserId).RoleId,
                                             BooksCount = x.BooksCount,
                                             AllBooks = x.Books.Select(y => new BookDto 
                                             { 
                                                Id = y.Id,
                                                Name = y.Name,
                                                Published = y.Published,
                                                ISBN = y.ISBN,
                                                PublisherName = y.Publisher.Name
                                             }).ToList(),
                                             AuthorAge = x.Age,
                                             AuthorCountry = x.Country,
                                             AuthorNickName = x.NickName,
                                             Id = x.Id
                                           })
                                           .ToList()
                                           .Where(x => x.AuthorCountry == "Serbia" && x.AuthorAge == 27)
                                           .ToList();

            var orderedAuthors = authors.OrderByDescending(x => x.BooksCount).ToList().Take(2).ToList();

            List<AuthorDto> finalAuthors = new List<AuthorDto>();

            foreach (var author in orderedAuthors)
            {
                List<BookDto> books = new List<BookDto>();

                var allBooks = author.AllBooks;

                foreach (var book in allBooks)
                {
                    if (book.Published.Year < 1900)
                    {
                        book.PublishedYear = book.Published.Year;
                        books.Add(book);
                    }
                }

                author.AllBooks = books;
                finalAuthors.Add(author);
            }

            return finalAuthors;
        }

        //[Benchmark]
        public string StringConcat()
        {
            string s = "";

            for (int i = 0; i < 1000; i++)
            {
                s += i;
            }

            return s;
        }

        //[Benchmark]
        public string StringBuilder()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < 1000; i++)
            {
                sb.Append(i);
            }

            return sb.ToString();
        }
    }
}
