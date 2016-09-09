using System.Collections.Generic;

namespace Dapper.Demo.Entites
{
    public class Categorys
    {
        public int CategoryId { get; set; }

        public int GetCategoryId()
        {
            return CategoryId;
        }
        
        public string CategoryName { get; set; }

        public List<Products> Products { get; set; }
    }
}
