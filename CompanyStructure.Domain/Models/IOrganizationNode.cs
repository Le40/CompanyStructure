namespace CompanyStructure.Models
{
    public interface IOrganizationNode
    {
        int Id { get; set; }
        string Name { get; set; }
        string Code { get; set; }
        int? LeaderId { get; set; }
    }
}
