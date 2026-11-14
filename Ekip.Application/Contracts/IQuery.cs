using Ekip.Domain.Results;
using MediatR;

namespace Ekip.Application.Contracts;

public interface IBaseQuery;

public interface IQuery<TValue> : IRequest<Result<TValue>>, IBaseQuery;

public interface IQueryHandler<in TQuery, TValue> : IRequestHandler<TQuery, Result<TValue>>
    where TQuery : IQuery<TValue>;