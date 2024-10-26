// This namespace is needed to process message coming from external source.
// DO NOT CHANGE IT
namespace UTU.Voucher.Contracts;

/// <summary>
/// This event is raised when a voucher issuing is completed.
/// The event is issued outside the system.
/// </summary>
/// <param name="VoucherCode"></param>
public sealed record VoucherResponseEvent(string? VoucherCode);
