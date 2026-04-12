using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Order.Application.DTOs;
using Order.Application.Interfaces;
using SharedKernel.Exceptions;

namespace Order.Application.Queries.GetOrderById;

public record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDto>;