using Microsoft.EntityFrameworkCore;
using Casus.Objects;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Build.Framework;

namespace Casus
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options)
            : base(options)
        {
            this.Database.EnsureCreated();
        }

        public DbSet<Order> Orders { get; set; } = null!;

        public DbSet<Klant> Klanten { get; set; } = null!;
        public DbSet<Orderregel> Orderregels { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            DbSeed(builder);
        }

        private void DbSeed(ModelBuilder builder)
        {
            builder.Entity<Klant>(k =>
            {
                k.HasData(new Klant(5, "Frenkie de Jong"));
                k.HasData(new Klant(1, "Mepmhis Depay"));
                k.HasData(new Klant(2, "Virgil van Dijk"));
                k.HasData(new Klant(3, "Cody Gakpo"));
                k.HasData(new Klant(4, "Andries Noppert"));
            });

            builder.Entity<Product>(p =>
            {
                p.HasData(new Product("Broodje Gezond", 1.11m));
                p.HasData(new Product("Tosti", 2.22m));
                p.HasData(new Product("Pannenkoek", 3.33m));
                p.HasData(new Product("Koffie", 4.44m));
                p.HasData(new Product("Thee", 5.55m));
                p.HasData(new Product("Appel", 6.66m));
            });

            //Create Orders
            Order o1 = new Order(3);
            Order.incrementId();
            Order o2 = new Order(1);
            Order.incrementId();
            Order o3 = new Order(4);
            Order.incrementId();
            Order o4 = new Order(5);
            Order.incrementId();
            Order o5 = new Order(3);
            Order.incrementId();

            Orderregel or11 = new Orderregel(o1.Id, "Pannenkoek", 2);
            Orderregel or12 = new Orderregel(o1.Id, "Koffie", 1);
            Orderregel or2 = new Orderregel(o2.Id, "Appel", 1);
            Orderregel or31 = new Orderregel(o3.Id, "Appel", 1);
            Orderregel or32 = new Orderregel(o3.Id, "Thee", 1);
            Orderregel or4 = new Orderregel(o4.Id, "Banaan", 3);
            Orderregel or51 = new Orderregel(o5.Id, "Appel", 1);
            Orderregel or52 = new Orderregel(o5.Id, "Koffie", 2);

            //Create orders
            builder.Entity<Order>(o =>
            {
                // order(id, klantid)
                o.HasData(o1);
                o.HasData(o2);
                o.HasData(o3);
                o.HasData(o4);
                o.HasData(o5);
            });

            //TODO: seed orderregels
            builder.Entity<Orderregel>(or =>
            {
                or.HasData(or11);           //order1
                or.HasData(or12);           //order1
                or.HasData(or2);            //Order2
                or.HasData(or31);           //Order3
                or.HasData(or32);           //Order3
                or.HasData(or4);            //Order4
                or.HasData(or51);           //Order5
                or.HasData(or52);           //Order5
            });
        }
    }
}



