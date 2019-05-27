using System.Text.Json.Serialization;
using Xunit;

namespace System.Text.Json.Tests.Imported
{
    public class Person
    {
        public string Name { get; set; }

        public DateTime BirthDate { get; set; }

        public DateTime LastModified { get; set; }

        [JsonIgnore]
        public string Department { get; set; }
    }


    public class Product
    {
        public string Name { get; set; }
        public DateTime ExpiryDate { get; set;}
        public decimal Price { get; set; }
        public string[] Sizes { get; set; }
    }

    public class CamelCaseTests
    {
        private JsonSerializerOptions camelCaseAndIndentedOption = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        [Fact]
        public void JsonSerializerCamelCaseSettings()
        {
            Person person = new Person();
            person.BirthDate = new DateTime(2000, 11, 20, 23, 55, 44, DateTimeKind.Utc);
            person.LastModified = new DateTime(2000, 11, 20, 23, 55, 44, DateTimeKind.Utc);
            person.Name = "Name!";

            string json = JsonSerializer.ToString(person, camelCaseAndIndentedOption);

            Assert.Equal(@"{
  ""name"": ""Name!"",
  ""birthDate"": ""2000-11-20T23:55:44Z"",
  ""lastModified"": ""2000-11-20T23:55:44Z""
}", json);

            Person deserializedPerson = JsonSerializer.Parse<Person>(json, camelCaseAndIndentedOption);

            Assert.Equal(person.BirthDate, deserializedPerson.BirthDate);
            Assert.Equal(person.LastModified, deserializedPerson.LastModified);
            Assert.Equal(person.Name, deserializedPerson.Name);

            json = JsonSerializer.ToString(person, new JsonSerializerOptions { WriteIndented = true });
            Assert.Equal(@"{
  ""Name"": ""Name!"",
  ""BirthDate"": ""2000-11-20T23:55:44Z"",
  ""LastModified"": ""2000-11-20T23:55:44Z""
}", json);
        }

        [Fact]
        public void BlogPostExample()
        {
            Product product = new Product
            {
                ExpiryDate = new DateTime(2010, 12, 20, 18, 1, 0, DateTimeKind.Utc),
                Name = "Widget",
                Price = 9.99m,
                Sizes = new[] { "Small", "Medium", "Large" }
            };

            string json = JsonSerializer.ToString(product, camelCaseAndIndentedOption);

            Assert.Equal(@"{
  ""name"": ""Widget"",
  ""expiryDate"": ""2010-12-20T18:01:00Z"",
  ""price"": 9.99,
  ""sizes"": [
    ""Small"",
    ""Medium"",
    ""Large""
  ]
}", json);
        }
    }
}
