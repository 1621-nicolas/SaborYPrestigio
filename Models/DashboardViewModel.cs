using System.Collections.Generic;

namespace SaborPrestigioMVC.Models
{
    public class DashboardViewModel
    {
        public int TotalClientes { get; set; }
        public int TotalPedidos { get; set; }
        public int TotalReservas { get; set; }
        public int MesasDisponibles { get; set; }
        public int PlatillosDisponibles { get; set; }
        public int StockCritico { get; set; }
        public int ComprobantesEmitidos { get; set; }
        public decimal VentasTotales { get; set; }

        public List<Pedido> UltimosPedidos { get; set; } = new();
        public List<VentaPorDiaViewModel> VentasPorDia { get; set; } = new();
    }
    public class VentaPorDiaViewModel
    {
        public string Dia { get; set; }
        public decimal Total { get; set; }
    }
}