namespace CompanyStructure.Application.Nodes.DTOs
{
    public class NodeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public int? LeaderId { get; set; }
    }
}
