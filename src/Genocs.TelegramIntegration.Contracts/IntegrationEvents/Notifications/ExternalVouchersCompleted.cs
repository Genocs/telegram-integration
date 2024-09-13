// This namespace is needed to process message coming from external source.
// DO NOT CHANGE IT
namespace UTU.Voucher.Contracts;

public sealed record VoucherResponseEvent(string? VoucherCode, string? BarcodeFormat, string? BarcodeString, string? ExpirationDate, string? GiftCardCode, string? CaId, ResponseStatus? ResponseStatus);

public sealed record ResponseStatus(string? ErrorCode, string? Message);