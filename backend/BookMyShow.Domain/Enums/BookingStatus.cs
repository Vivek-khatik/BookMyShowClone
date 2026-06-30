namespace BookMyShow.Domain.Enums;

public enum BookingStatus
{
    Pending = 1,
    Confirmed = 2,
    Cancelled = 3,
    Expired = 4,
    PaymentPending = 5,
    PaymentFailed = 6
}
