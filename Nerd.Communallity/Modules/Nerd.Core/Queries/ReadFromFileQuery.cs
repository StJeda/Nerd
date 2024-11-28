using MediatR;

namespace Nerd.Domain.DTOs;

public record ReadFromFileQuery(string path) : IRequest<OperationResponse>;
