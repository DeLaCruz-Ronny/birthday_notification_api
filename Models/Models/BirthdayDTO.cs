﻿namespace birthday_notification_api.Models.Models
{
    public class BirthdayDTO
    {
        public string nombre { get; set; } = null!;
        public string telefono { get; set; } = null!;
        public DateTime fecha_cumpleanos { get; set; }
        public string url_imag { get; set; }
    }
}
