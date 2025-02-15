using AutoMapper;
using birthday_notification_api.Models;
using birthday_notification_api.Models.Models;

namespace birthday_notification_api.Mapeo
{
    public class ModelsProfile:Profile
    {
        public ModelsProfile()
        {
            CreateMap<Birthday, BirthdayDTO>().ReverseMap();
        }
    }
}
