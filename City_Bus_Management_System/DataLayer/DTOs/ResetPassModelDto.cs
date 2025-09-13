namespace City_Bus_Management_System.DataLayer.DTOs
{
    public class ResetPassModelDto
    {
       public string Email { get; set; }
        public string token { get; set; }
        public string NewPassword { get; set; }
        //public string ConfirmPassword { get; set; }
    }
}
