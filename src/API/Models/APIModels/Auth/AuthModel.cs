namespace API.Models.APIModels.Auth
{
    public class TokenModel
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public List<int> RoleIds { get; set; }
        public string JwtToken { get; set; }
        public DateTime? ExpiredDate { get; set; }
    }
}
