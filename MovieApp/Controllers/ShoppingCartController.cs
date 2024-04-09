using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using MovieApp.Data;
using MovieApp.Models;
using MovieApp.Models.DTO;
using System.Data;
using System.Security.Claims;

namespace MovieApp.Controllers
{
    public class ShoppingCartController : Controller
    {

        private readonly ApplicationDbContext _context;

        public ShoppingCartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var loggedInUser = await _context.Users
                .Where(z => z.Id.Equals(userId))
                .Include(z => z.UserCart)
                .Include(z => z.UserCart.TicketInShoppingCarts)
                .Include("UserCart.TicketInShoppingCarts.Ticket")
                .Include("UserCart.TicketInShoppingCarts.Ticket.Movie")
                .FirstOrDefaultAsync();

            var tickets = loggedInUser.UserCart.TicketInShoppingCarts.Select(z => new
            {
                TicketPrice = z.Ticket.Price,
                Quantity = z.Quantity
            });

            var totalPrice = 0.0;

            foreach (var ticket in tickets)
            {
                totalPrice += ticket.TicketPrice * ticket.Quantity;
            }

            ShoppingCartDTO model = new ShoppingCartDTO
            {
                TicketInShoppingCarts = loggedInUser.UserCart.TicketInShoppingCarts.ToList(),
                TotalPrice = totalPrice
            };


            return View(model);
        }

        public async Task<IActionResult> DeleteTicketFromShoppingCart(Guid? ticketId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var loggedInUser = await _context.Users
                .Where(z => z.Id.Equals(userId))
                .Include(z => z.UserCart)
                .Include(z => z.UserCart.TicketInShoppingCarts)
                .Include("UserCart.TicketInShoppingCarts.Ticket")
                .Include("UserCart.TicketInShoppingCarts.Ticket.Movie")
                .FirstOrDefaultAsync();

            var userShoppingCart = loggedInUser.UserCart;

            var ticketInShoppingCart = userShoppingCart.TicketInShoppingCarts.Where(z => z.TicketId.Equals(ticketId))
                .FirstOrDefault();

            userShoppingCart.TicketInShoppingCarts
                .Remove(ticketInShoppingCart);

            _context.Update(userShoppingCart);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "ShoppingCart");
        }

        public async Task<IActionResult> OrderNow()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var loggedInUser = await _context.Users
                .Where(z => z.Id.Equals(userId))
                .Include(z => z.UserCart)
                .Include(z => z.UserCart.TicketInShoppingCarts)
                .Include("UserCart.TicketInShoppingCarts.Ticket")
                .Include("UserCart.TicketInShoppingCarts.Ticket.Movie")
                .FirstOrDefaultAsync();

            var userShoppingCart = loggedInUser.UserCart;

            Order order = new Order
            {
                Id = Guid.NewGuid(),
                OwnerId = userId,
                Owner = loggedInUser
            };

            _context.Add(order);

            List<TicketInOrder> ticketInOrders = userShoppingCart.TicketInShoppingCarts
                .Select(z => new TicketInOrder
                {
                    TicketId = z.TicketId,
                    Ticket = z.Ticket,
                    OrderId = order.Id,
                    Order = order
                }).ToList();

            foreach (var item in ticketInOrders)
            {
                _context.Add(item);
            }

            loggedInUser.UserCart.TicketInShoppingCarts.Clear();
            _context.Update(loggedInUser);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "ShoppingCart");
        }
    }
}
