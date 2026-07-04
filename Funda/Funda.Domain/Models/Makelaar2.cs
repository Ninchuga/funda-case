using System;
using System.Collections.Generic;
using System.Text;

namespace Funda.Domain.Models
{
    public class Makelaar2
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumberOfPropertiesForSale { get; private set; }
    }
}
