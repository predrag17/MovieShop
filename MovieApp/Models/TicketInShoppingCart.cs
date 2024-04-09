using System.ComponentModel.DataAnnotations;

namespace MovieApp.Models
{
    public class TicketInShoppingCart
    {
        [Key]
        public Guid Id { get; set; }

        public Guid TicketId { get; set; }
        public Ticket Ticket { get; set; }
        public Guid ShoppingCartId { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
        public int Quantity { get; set; }

    }
}
