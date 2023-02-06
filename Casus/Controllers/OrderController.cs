using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Casus;
using Casus.Objects;
using static Casus.Controllers.KlantController;

namespace Casus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        public struct ProductInput
        {
            public string ProductId { get; set; }
            public int Aantal { get; set; }
        }

        public struct OrderInput
        {
            public int KlantId { get; set; }
            public List<ProductInput> Producten { get; set; }
        }

        private readonly OrderDbContext _context;

        public OrderController(OrderDbContext context)
        {
            Console.WriteLine("Ordercontroller context");
            _context = context;
        }

        // Create Order
        // POST: api/Order
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrderInput>> PostOrder(OrderInput orderInput)
        {
            Order order = new Order(orderInput.KlantId);
            _context.Orders.Add(order);

            foreach (ProductInput pi in orderInput.Producten)
            {
                _context.Orderregels.AddRange(new Orderregel(order.Id, pi.ProductId, pi.Aantal));
            }
            await _context.SaveChangesAsync();
            Order.incrementId();

            return CreatedAtAction("GetOrder", new { id = order.Id }, order);
        }

        // Delete order
        // DELETE: api/Order/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            Order order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            List<Orderregel> ors = await _context.Orderregels.Where(or => or.Ordernr == order.Id).ToListAsync();
            foreach(Orderregel or in ors)
            {
                _context.Orderregels.Remove(or);
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Wijzigen order
        // PUT: api/Order/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //Changing order should change orderregels, as thats where the real info is stored from an order. 
        [HttpPut("Change/{id}")]
        public async Task<IActionResult> PutOrder(int id, OrderInput orderInput)
        {
            // If there is no order with this id and KlantId, give badRequest. 
            if (_context.Orders.Any(o => o.Id == id))
            {
                return BadRequest();
            }

            //Update orderregels belonging to this orderId. 
            //We do this by assuming the GUI or whatever application first pulls all the orderregels belonging to the order. 
            //Then modify it, and send it back correctly. 
            //Delete the orderregels essentially, and then recreate. Because modifying an order can consist of changing products, amounts, deleting them. etc. 

            //Delete existing
            foreach (Orderregel or in _context.Orderregels.Where(or => or.Ordernr == id)) 
            {
                _context.Orderregels.Remove(or);
            }

            //Recreate the Orderregels.
            foreach (ProductInput pi in orderInput.Producten)
            {
                _context.Orderregels.AddRange(new Orderregel(id, pi.ProductId, pi.Aantal));
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
