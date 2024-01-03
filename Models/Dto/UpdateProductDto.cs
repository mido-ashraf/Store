namespace Store.Models.Dto
{
    public class UpdateProductDto
    {
        public int Id { get; set; }
        public int Price { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
    }
}
