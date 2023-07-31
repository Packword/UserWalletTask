namespace UserWallet.Data
{
    public class ApplicationDbContext: DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Deposit> Deposits { get; set; } = null!;
        public DbSet<UserBalance> UserBalances { get; set; } = null!;
        public DbSet<Currency> Currencies { get; set; } = null!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserBalance>().HasKey(p => new { p.UserId, p.CurrencyId });
            modelBuilder.UseSerialColumns();
        }
    }
}
