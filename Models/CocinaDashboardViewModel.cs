namespace SaborPrestigioMVC.Models
{
    public class CocinaDashboardViewModel
    {
        public int PedidosEnEspera { get; set; }
        public int PedidosEnCocina { get; set; }
        public int PedidosListos { get; set; }

        public List<Pedido> PedidosCocina { get; set; } = new();
        public List<PlatilloMasPedidoViewModel> PlatillosMasPedidos { get; set; } = new();
    }

    public class PlatilloMasPedidoViewModel
    {
        public string NombrePlatillo { get; set; }
        public int CantidadVendida { get; set; }
    }
}