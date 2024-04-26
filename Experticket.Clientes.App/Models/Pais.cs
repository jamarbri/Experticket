using System.ComponentModel.DataAnnotations;

namespace Experticket.Clientes.App.Models
{
    public class Pais
    {
        [Key]
        public int Id { get; set; }

        public string Nombre { get; set; }
    }
}
