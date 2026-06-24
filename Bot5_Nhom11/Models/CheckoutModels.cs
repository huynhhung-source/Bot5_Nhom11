namespace doanweb.Models
{
    public class CheckoutRequest
    {
        public List<CartItemRequest> CartItems { get; set; } = new();
        public string deliveryAddress { get; set; } = string.Empty;
        public string paymentMethod { get; set; } = string.Empty;
        public string? transactionId { get; set; }
        public string? notes { get; set; }
    }

    public class CartItemRequest
    {
        public int productId { get; set; }
        public string productName { get; set; } = string.Empty;
        public decimal price { get; set; }
        public int quantity { get; set; }
        public string? imageUrl { get; set; }
        public int? stockQuantity { get; set; }
        public bool outOfStock { get; set; }
        public bool unavailable { get; set; }
    }
}
