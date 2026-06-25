namespace SaborPrestigioMVC.Models
{
    public class ClienteDashboardViewModel
    {
        public int PlatillosDisponibles { get; set; }
        public int ReservasActivas { get; set; }
        public int PedidosWebActivos { get; set; }

        public List<Platillo> MenuPlatillos { get; set; } = new();
        public List<Pedido> PedidosWeb { get; set; } = new();
    }
}