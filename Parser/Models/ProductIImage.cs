using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Parser;

public class ProductIImage
{
    public int Id { get; set; }
    
    [NotMapped]
    public Dictionary<string, string> Image { get; set; }

    [Column(TypeName = "json")] 
    public string? ImageJson 
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
            return JsonSerializer.Serialize(Image, options);
        }
        set
        {
            Image = JsonSerializer.Deserialize(value, typeof(Dictionary<string, string>)) as Dictionary<string, string>;
        }
    } 

    public int ProductId { get; set; } // Внешний ключ
    public Product Product { get; set; } 
}