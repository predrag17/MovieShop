namespace MovieApp.Models.DTO
{
    public class AddToShoppingCartDTO
    {
        public Guid SelectedTicketId { get; set; }
        public Ticket? SelectedTicket { get; set; }
        public int Quantity { get; set; }

    }
}
