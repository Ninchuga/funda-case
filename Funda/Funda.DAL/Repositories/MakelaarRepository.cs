using Funda.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Funda.DAL.Repositories
{
    internal class MakelaarRepository : IMakelaarRepository
    {
        public async Task<List<Makelaar>> GetTopTenMakelaarsFor(string city, bool propertiesWithGarden)
        {
            return [];
        }
    }
}
