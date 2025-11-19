namespace ERP_Models.Events
{
    public record UserRegisteredEvent(int UserId, string FullName, string Email, List<string> Roles);
}
