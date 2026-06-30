namespace BookMyShow.Application.DTOs.Shows.ShowPrices;

public class ShowPriceResponse
{
    public Guid Id { get; set; }

    public Guid ShowId { get; set; }

    public Guid SeatCategoryId { get; set; }

    public string SeatCategoryName { get; set; } = null!;

    public decimal Price { get; set; }
}

