
namespace Ekip.Application.DTOs.User
{
    public class MyEkipDto
    {
        public long RequestId { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public int Members { get; set; }
        public string Craetor { get; set; }
        public DateTime StartEventDateTime { get; set; }
    }
}
