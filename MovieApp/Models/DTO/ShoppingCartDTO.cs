namespace MovieApp.Models.DTO
{
    public class ShoppingCartDTO
    {

        public List<TicketInShoppingCart> TicketInShoppingCarts { get; set; }
        public double TotalPrice { get; set; }
    }
}
