// Define the IProductoService interface to resolve CS0246
using System.Threading.Tasks;
using UI.Models;

namespace UI.Services
{
    public interface IProductoService
    {
        Task Guardar(Productos producto);
    }
}