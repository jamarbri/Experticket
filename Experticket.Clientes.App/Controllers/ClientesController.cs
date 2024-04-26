using Experticket.Clientes.App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Experticket.Clientes.App.Controllers
{
    public class ClientesController : Controller
    {
        private readonly ClientesDataContext _context;

        public ClientesController(ClientesDataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Cliente.Include("Pais").ToListAsync());
        }

        public IActionResult Create()
        {
            GetPaisesDropDrownList();

            return View();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await ObtenerClientePorId(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Cliente cliente = await ObtenerClientePorId(id);

            if (cliente == null)
            {
                return NotFound();
            }

            GetPaisesDropDrownList();

            return View(cliente);
        }
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await ObtenerClientePorId(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Apellidos,Sexo,FechaNacimiento,Direccion,Pais,CodigoPostal,Email")] Cliente cliente)
        {
            ValidarCliente(cliente);

            if (ModelState.IsValid)
            {
                //_context.Add(cliente);
                //await _context.SaveChangesAsync();

                

                await SaveCliente(cliente, nuevo: true);

                return RedirectToAction(nameof(Index));
            }

            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellidos,Sexo,FechaNacimiento,Direccion,Pais,CodigoPostal,Email")] Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return NotFound();
            }

            ValidarCliente(cliente);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Cliente.FindAsync(id);
            _context.Cliente.Remove(cliente);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<Cliente> ObtenerClientePorId(int? id)
        {
            return await _context.Cliente
                                 .Include("Pais")
                                 .FirstOrDefaultAsync(m => m.Id == id);
        }

        private void GetPaisesDropDrownList()
        {
            ViewBag.Paises = _context.Pais.Select(i => new SelectListItem
                     {
                         Value = i.Id.ToString(),
                         Text = i.Nombre
                     }).ToList();
        }

        private bool ClienteExists(int id)
        {
            return _context.Cliente.Any(e => e.Id == id);
        }

        private void ValidarCliente(Cliente cliente)
        {

            if (string.IsNullOrWhiteSpace(cliente.Nombre))
            {
                ModelState.AddModelError("Nombre", "El campo Nombre no puede estar vacío");
            }

            if (!string.IsNullOrWhiteSpace(cliente.CodigoPostal) && IsValidPostalCode(cliente.CodigoPostal))
            {
                ModelState.AddModelError("CP", "El campo CP es correcto. Debe ser un número de 5 dígitos");
            }

            if (string.IsNullOrWhiteSpace(cliente.Email))
            {
                ModelState.AddModelError("Email", "El campo Email no puede estar vacío.");
            }
            else if (!IsValidEmail(cliente.Email))
            {
                ModelState.AddModelError("email", "El Email introducido no es correcto.");
            }
        }

        private Task<int> SaveCliente(Cliente cliente, bool nuevo)
        {
            cliente.PaisId = cliente.Pais.Id;
            cliente.Pais = null;

            if (nuevo)
            {
                _context.Add(cliente);
            }
            else
            {
                _context.Update(cliente);
            }

            return _context.SaveChangesAsync();
        }


        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidPostalCode(string postalCode)
        {
            string pattern = @"\d{5}";

            var regex = new Regex(pattern);

            return regex.IsMatch(postalCode);
        }
    }
}
