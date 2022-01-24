using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ERT.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartCopy> CartCopies { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<QRCode> QRCodes { get; set; }

        public DbSet<IdentityUserRole> UserInRole { get; set; }
        // public DbSet<ApplicationUser> appUsers { get; set; }
        public DbSet<ApplicationRole> appRoles { get; set; }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

		public System.Data.Entity.DbSet<ERT.Models.OrderStatus> OrderStatus { get; set; }

		public System.Data.Entity.DbSet<ERT.Models.Eshop> Eshops { get; set; }

		public System.Data.Entity.DbSet<ERT.Models.Supplier> Suppliers { get; set; }

		public System.Data.Entity.DbSet<ERT.Models.Client> Clients { get; set; }

		public System.Data.Entity.DbSet<ERT.Models.Driver> Drivers { get; set; }

		public System.Data.Entity.DbSet<ERT.Models.Schedule> Schedules { get; set; }

		public System.Data.Entity.DbSet<ERT.Models.Beneficiary_Signature> Beneficiary_Signature { get; set; }

		public System.Data.Entity.DbSet<ERT.Models.Return> Returns { get; set; }
	}
}