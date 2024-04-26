using Microsoft.EntityFrameworkCore;

public class ClientesDataContext : DbContext
{
    public ClientesDataContext(DbContextOptions<ClientesDataContext> options)
        : base(options)
    {
    }

    public DbSet<Experticket.Clientes.App.Models.Cliente> Cliente { get; set; }

    public DbSet<Experticket.Clientes.App.Models.Pais> Pais { get; set; }
}

