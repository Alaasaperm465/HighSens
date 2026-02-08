namespace HighSens.Domain
{
    using global::Domain;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text.Json.Serialization;

    public class Product: BaseEntity
    {
        public string Name { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        // Navigation: stocks referencing this product
        [JsonIgnore]
        public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
    }
}
