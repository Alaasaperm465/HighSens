namespace Frozen_Warehouse.Domain.Entities
{
    using global::Domain;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Client: BaseEntity
    {
        public string Name { get; set; } = null!;

    }
}
