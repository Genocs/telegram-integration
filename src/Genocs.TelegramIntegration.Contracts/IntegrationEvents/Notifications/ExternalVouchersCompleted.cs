// This namespace is needed to process message coming from external source.
// DO NOT CHANGE IT
namespace Genocs.Voucher.Contracts;

public sealed record VoucherResponseEvent(string? VoucherCode, string? ExpirationDate, ResponseStatus? ResponseStatus);

public sealed record ResponseStatus(string? ErrorCode, string? Message);