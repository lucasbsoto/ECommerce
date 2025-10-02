using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities
{
    public class Customer
    {
        public Customer(Guid customerId, string name, string cpf, CustomerCategory category)
        {
            CustomerId = customerId;
            Name = name;
            Cpf = cpf;
            Category = category;
        }

        public Guid CustomerId { get; private set; }
        public string Name { get; private set; }
        public string Cpf { get; private set; }
        public CustomerCategory Category { get; private set; }
        public List<Sale> Sales { get; set; }
    }
}
