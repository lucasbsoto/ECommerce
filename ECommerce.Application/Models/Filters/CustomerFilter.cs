using ECommerce.Domain.Enums;

namespace ECommerce.Application.Models.Filters
{
    public class CustomerFilter
    {
        public CustomerFilter() {}
        public CustomerFilter(Guid? identifier, string? name, string? cpf, CustomerCategory? category)
        {
            Identifier = identifier;
            Name = name;
            Cpf = cpf;
            Category = category;
        }

        public Guid? Identifier { get; set; }
        public string? Name { get; set; }
        public string? Cpf { get; set; }
        public CustomerCategory? Category { get; set; }
    }
}
