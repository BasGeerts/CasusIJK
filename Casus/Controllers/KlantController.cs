using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Casus;
using Casus.Objects;

namespace Casus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KlantController : ControllerBase
    {
        public struct KlantOrders
        {
            public Klant klant { get; set; }
            public List<Orderregel> orderregels { get; set; }
        }

        private readonly OrderDbContext _context;

        public KlantController(OrderDbContext context)
        {
            _context = context;
        }

        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<KlantOrders>>> GetAlleKlanten()
        {
            List<Klant> klanten = await _context.Klanten.OrderBy(k => k.Id).ToListAsync();
            List<Order> orders = await _context.Orders.OrderBy(o => o.KlantID).ToListAsync();
            List<Orderregel> orderregels = await _context.Orderregels.OrderBy(or => or.Ordernr).ToListAsync();
           
            List<KlantOrders> klantOrders = new List<KlantOrders>();
            foreach (Klant k in klanten)
            {   
                List<Orderregel> ors = new List<Orderregel>();
                foreach (Order o in orders.Where(o => o.KlantID == k.Id).ToList())
                {
                    ors.AddRange((orderregels.Where(or => or.Ordernr == o.Id).ToList()));
                }
                KlantOrders ko = new KlantOrders
                {
                    klant = k,
                    orderregels = ors 
                };
                klantOrders.Add(ko);
            }
            return klantOrders;
        }

        // GET: api/Klant/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KlantOrders>> GetKlant(int id)
        {
            Klant klant = await _context.Klanten.FindAsync(id);
            if (klant == null)
            {
                return NotFound();
            }
           
            int[] orderIds = await _context.Orders.Where(o => o.KlantID.Equals(id)).Select(o => o.Id).ToArrayAsync();
            List<Orderregel> orderregels = new List<Orderregel>();
            foreach (int i in orderIds)
                orderregels.AddRange(await _context.Orderregels.Where(or => or.Ordernr == i).ToListAsync());


            //Gegevens van de klant met de orders.
            KlantOrders ko = new KlantOrders
            {
                klant = klant,
                orderregels = orderregels
            };
            return ko;
        }

        //Create Klant
        // POST: api/Klant
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Klant>> PostKlant(Klant klant)
        {
            _context.Klanten.Add(klant);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetKlant", new { id = klant.Id }, klant);
        }

        // Delete Klant
        // DELETE: api/Klant/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKlant(int id)
        {
            var klant = await _context.Klanten.FindAsync(id);
            if (klant == null)
            {
                return NotFound();
            }

            //Get outstanding orders
            List<Order> orders = await _context.Orders.Where(o => o.KlantID == id).ToListAsync();

            //Remove orderregels
            foreach(Order o in orders)
            {
                List<Orderregel> orderregels = await _context.Orderregels.Where(or => or.Ordernr == o.Id).ToListAsync();
                foreach (Orderregel or in orderregels)
                    _context.Orderregels.Remove(or);
            }

            //Remove orders
            foreach (Order o in orders)
                _context.Orders.Remove(o);


            _context.Klanten.Remove(klant);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Update Klant
        // CHANGE: api/Klant/Change/5
        [HttpPut("Change/{id}")]
        public async Task<IActionResult> ChangeKlant(int id, Klant klant)
        {
            if (id != klant.Id)
            {
                return BadRequest();
            }

            _context.Entry(klant).State = EntityState.Modified;


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KlantExists(id))
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

        private bool KlantExists(int id)
        {
            return _context.Klanten.Any(e => e.Id == id);
        }

    }
}
