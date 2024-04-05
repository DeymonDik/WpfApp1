using System.Text;
using System.Text.Json;

namespace ClientApplication.Models.Dtos
{
    public class PersonDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Price {  get; set; }

        private readonly JsonSerializerOptions options = new()
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };

        public override string ToString()
        {
            return Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(this, options));
        }
    }
}