namespace PlayMe.Common.Model
{
    public class User : DataObject
    {
        public string Username { get; set; }
        public bool IsAdmin { get; set; }
    }
}
