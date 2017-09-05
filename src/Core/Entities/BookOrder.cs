﻿using System;
using System.Collections.Generic;
using Core.ValueObjects;

namespace Core.Entities
{
    public class BookOrder
    {
        public BookOrder(string supplier, Guid id, BookOrderState state)
        {
            if (string.IsNullOrWhiteSpace(supplier))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(supplier));

            Supplier = supplier;
            Id = id;
            State = state;

            OrderLines = new List<OrderLine>();
        }

        public void AddBookRequest(BookRequest bookRequest)
        {
            OrderLine orderLine = new OrderLine(bookRequest.Title,
                bookRequest.Price, bookRequest.Quantity);

            OrderLines.Add(orderLine);
        }

        public string Supplier { get; }
        public Guid Id { get; }
        public List<OrderLine> OrderLines { get; }
        public BookOrderState State { get; }
    }
}