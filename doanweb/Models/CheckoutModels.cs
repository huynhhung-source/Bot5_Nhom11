namespace doanweb.Models
{
    public class CheckoutRequest
    {
        public List<CartItemRequest> CartItems { get; set; }
        public string deliveryAddress { get; set; }
        public string paymentMethod { get; set; }
        public string notes { get; set; }
    }

    public class CartItemRequest
    {
        public int productId { get; set; }
        public string productName { get; set; }
        public decimal price { get; set; }
        public int quantity { get; set; }
    }
}
