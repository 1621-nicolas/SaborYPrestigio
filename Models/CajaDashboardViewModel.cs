namespace SaborPrestigioMVC.Models
{
    public class CajaDashboardViewModel
    {
        public int PedidosPorCobrar { get; set; }
        public int ComprobantesEmitidos { get; set; }
        public decimal VentasDelDia { get; set; }
        public decimal IgvDelDia { get; set; }

        public List<Pedido> PedidosPendientesPago { get; set; } = new();
        public List<ComprobantePago> ComprobantesDelDia { get; set; } = new();
        public List<MetodoPagoViewModel> MetodosPago { get; set; } = new();
    }

    public class MetodoPagoViewModel
    {
        public string MetodoPago { get; set; } = "";
        public decimal Total { get; set; }
        public int Cantidad { get; set; }
    }
}