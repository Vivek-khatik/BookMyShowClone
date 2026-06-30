namespace BookMyShow.Application.DTOs.Shows.ShowPrices;

public class UpdateShowPriceRequest
{
    public Guid SeatCategoryId { get; set; }

    public decimal Price { get; set; }
}

