namespace SaborPrestigioMVC.Models
{
    public class SalonDashboardViewModel
    {
        public int MesasDisponibles { get; set; }
        public int MesasOcupadas { get; set; }
        public int ReservasDelDia { get; set; }
        public int PedidosActivos { get; set; }

        public List<Mesa> Mesas { get; set; } = new();
        public List<Reserva> ReservasHoy { get; set; } = new();
        public List<Pedido> PedidosActivosLista { get; set; } = new();
    }
}