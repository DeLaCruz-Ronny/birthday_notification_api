namespace birthday_notification_api.Models
{
    public class Birthday
    {
        public int id { get; set; }
        public string nombre { get; set; } = null!;
        public string telefono { get; set; } = null!;
        public DateTime fecha_cumpleanos { get; set; }
        public string url_img { get; set; } = null!;
        public DateTime fecha_registro { get; set; }
    }
}
