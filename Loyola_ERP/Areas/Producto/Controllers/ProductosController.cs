using Loyola_ERP.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Linq;
using UI.Models;
using UI.Services;
using static System.Net.Mime.MediaTypeNames;


namespace UI.Areas.Producto.Controllers
{
    [Area("Producto")]
    public class ProductosController : Controller
    {
        private readonly TiendaProductosContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IProductoService _productoService;

        public ProductosController(TiendaProductosContext context, IWebHostEnvironment hostEnvironment, IProductoService productoService)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _productoService = productoService;
        }
        public IActionResult ListadoProductos()
        {
            return View();
        }
        public async Task<IActionResult> Producto(int productoId = 0)
        {
            var producto = await _context.Productos.FirstOrDefaultAsync(c => c.Id == productoId);
            if(producto == null)
            {
                producto = new Productos();
            }
            return View(producto);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarProducto(Productos model, IFormFile imagenFile)
        {
            // <-- Abre el bloque try que envuelve la lógica principal -->
            try
            {
                if (!ModelState.IsValid)
                    return View("CrearEditar", model);

                // SI SUBIÓ IMAGEN
                if (imagenFile != null && imagenFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "imagenes/productos");

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var extension = ".jpg"; // si quieres puedes permitir otras
                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var image = SixLabors.ImageSharp.Image.Load(imagenFile.OpenReadStream()))
                    {
                        // 🔥 COMPRESIÓN Y REDIMENSIONADO (ajustado para móviles)
                        int maxWidth = 800;  // tamaño ideal para responsivo
                        int maxHeight = 800;

                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Mode = ResizeMode.Max,
                            Size = new Size(maxWidth, maxHeight)
                        }));

                        // 🔥 GUARDAR COMO JPG OPTIMIZADO (MUY LIVIANO)
                        var encoder = new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder
                        {
                            Quality = 75 // (0-100) 75 = recomendado
                        };

                        await image.SaveAsync(filePath, encoder);
                    }

                    // GUARDAR NOMBRE DE ARCHIVO EN BD
                    var producto = new Productos
                    {
                        Id = model.Id,
                        Nombre = model.Nombre,
                        Codigo = model.Codigo,
                        Stock = model.Stock,
                        Precio = model.Precio,
                        EstadoId = model.EstadoId,
                        Imagen = fileName // Guardar nombre de la imagen en BD
                    };

                    await _productoService.Guardar(producto);

                    return Json(new { mensaje = "Guardado correctamente", ok = true });
                }
                else
                {
                    // Si no se sube imagen, se sigue la lógica original de guardado/actualización
                    // usando el modelo que viene del formulario.
                    await _productoService.Guardar(model);
                    return Json(new { mensaje = "Guardado correctamente", ok = true });
                }
            }
            // <-- Cierra el bloque try, y aquí empieza el catch -->
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListarProductos()
        {
            return View();

        }

        [HttpGet]
        public IActionResult ObtenerProductos(string nombreBusqueda, string estadoBusqueda)
        {
            try
            {
                var query = _context.ProductoView.AsQueryable();

                if (!string.IsNullOrWhiteSpace(nombreBusqueda))
                {
                    query = query.Where(p => p.nombre.Contains(nombreBusqueda));
                }

                if (!string.IsNullOrWhiteSpace(estadoBusqueda))
                {
                    query = query.Where(p => p.estadoNombre == estadoBusqueda);
                }

                return Json(query.ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ocurrió un error al obtener los productos: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult ObtenerEstados()
        {
            try
            {
                var estados = _context.ProductoView
                 .Select(v => new { id = v.estadoId, nombre = v.estadoNombre })
                 .Distinct()
                 .ToList();

                return Json(estados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


    }
}

