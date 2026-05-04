namespace CompanyStructure.Domain.Models
{
    public interface IOrganisationNode
    {
        int Id { get; set; }
        string Name { get; set; }
        string Code { get; set; }
        int CompanyId { get; set; }
        int? LeaderId { get; set; }
    }
}
