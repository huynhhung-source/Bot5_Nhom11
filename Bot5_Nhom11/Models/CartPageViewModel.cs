namespace doanweb.Models
{
    public class CartLineViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
        public int StockQuantity { get; set; }
        public bool IsOutOfStock { get; set; }
        public bool IsUnavailable { get; set; }
        public decimal LineTotal => Price * Quantity;
    }

    public class CartAddRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
