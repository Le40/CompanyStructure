using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyStructure.Application.Nodes.DTOs
{
    public class CompanyStructureResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Code { get; set; } = "";
        public List<DivisionStructureResponse> Divisions { get; set; } = [];
    }

    public class DivisionStructureResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Code { get; set; } = "";
        public List<ProjectStructureResponse> Projects { get; set; } = [];
    }

    public class ProjectStructureResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Code { get; set; } = "";
        public List<DepartmentStructureResponse> Departments { get; set; } = [];
    }

    public class DepartmentStructureResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Code { get; set; } = "";
    }
}
