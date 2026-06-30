using BookMyShow.Domain.Enums;

namespace BookMyShow.Application.DTOs.Shows.Shows;

public class UpdateShowRequest
{
    public Guid? MovieId { get; set; }

    public Guid? EventId { get; set; }

    public Guid ScreenId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public ShowStatus Status { get; set; }
}

