﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Adapter.Persistence.MySql.Repositories.Actions;
using Adapter.Persistence.MySql.Repositories.Dtos;
using Domain.Entities;
using Domain.Ports.Persistence;
using MySql.Data.MySqlClient;

namespace Adapter.Persistence.MySql.Repositories
{
    internal class BookOrderLineConflictRepository : IBookOrderLineConflictRepository
    {
        private readonly string _connectionString;

        public BookOrderLineConflictRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

            _connectionString = connectionString;
        }

        public void Store(BookOrderLineConflict bookOrderLineConflict)
        {
            var dto = BookOrderLineConflictDtoMapper.From(bookOrderLineConflict);

            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    InsertBookOrderLineConflictAction.Execute(connection, dto);
                    transaction.Commit();
                }
            }
        }



        public void Store(IEnumerable<BookOrderLineConflict> conflict)
        {
            throw new NotImplementedException();
        }

        public BookOrderLineConflict Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BookOrderLineConflict> Get()
        {
            IEnumerable<BookOrderLineConflictDto> results = null;

            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    results = GetBookOrderLineConflictsAction.Execute(connection);
                    transaction.Commit();
                }
            }

            var bookOrderLineConflicts = results.Select(dto => BookOrderLineConflictMapper.From(dto));

            return bookOrderLineConflicts;
        }

        private IDbConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}