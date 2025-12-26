using UI.Models;
using UI.Data; // 👈 tu DbContext
using Microsoft.EntityFrameworkCore;

namespace UI.Services
{
    public class ProductoService : IProductoService
    {
        private readonly TiendaProductosContext _context;

        public ProductoService(TiendaProductosContext context)
        {
            _context = context;
        }

        public async Task Guardar(Productos producto)
        {
            if (producto.Id == 0)
            {
                // 🆕 NUEVO
                _context.Productos.Add(producto);
            }
            else
            {
                // ✏️ EDITAR
                _context.Productos.Update(producto);
            }

            await _context.SaveChangesAsync(); // 🔥 AQUÍ SE GUARDA
        }
    }
}
