namespace CompanyStructure.Application.DTOs.OrganisationNodes
{
    public class GetOrganisationNodeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public int? LeaderId { get; set; }
    }
}
