using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public class TestEntity
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }
    
        public string Name { get; set; }

        public int Number { get; set; }

        public string NewField { get; set; }
    }
}
