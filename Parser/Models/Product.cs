using System.ComponentModel.DataAnnotations.Schema;

using System.Text.Json;
namespace Parser
{ 
     public class Product
    {
        public int Id { get; set; }
        
        public string Category { get; set; } //128
        public string Name { get; set; } // 128
        public string Description { get; set; } 
        public int Price { get; set; }
        public int Stock { get; set; }
        
        [NotMapped] 
        public bool Avaible { get; set; }
        
        [NotMapped]
        public Dictionary<string, string> Characteristics { get; set; }

        [Column(TypeName = "json")] 
        public string? CharacteristicsJson 
        {
            get
            {
                var options = new JsonSerializerOptions
                {
                    // Включаем поддержку юникода
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    // Устанавливаем свойство WriteIndented в true для удобочитаемого формата JSON
                    WriteIndented = true
                };
                return JsonSerializer.Serialize(Characteristics, options);
            }
            set
            {
                Characteristics = JsonSerializer.Deserialize(value, typeof(Dictionary<string, string>)) as Dictionary<string, string>;
            }
        } 
        
        public string OriginUrl { get; set; } //128
        
        // Конструкторы класса Product
        public Product()
        {
            Category = "-";
            Name = "-";
            Description = "-";
            Price = 0;
            Stock = 0;
            Characteristics = new Dictionary<string, string>();;
            OriginUrl = "test";
            Avaible = true;
        }
        public Product(string category, string name, string description, int price, int stock,
            string originUrl ,Dictionary<string, string>  characteristics)
        {
            Category = category;
            Name = name;
            Description = description;
            Price = price;
            Stock = stock;
            OriginUrl = originUrl;
            Characteristics = characteristics;
        }
    }
}
