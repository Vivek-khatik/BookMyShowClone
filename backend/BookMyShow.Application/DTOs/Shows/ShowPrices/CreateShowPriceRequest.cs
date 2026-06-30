namespace BookMyShow.Application.DTOs.Shows.ShowPrices;

public class CreateShowPriceRequest
{
    public Guid ShowId { get; set; }

    public Guid SeatCategoryId { get; set; }

    public decimal Price { get; set; }
}

