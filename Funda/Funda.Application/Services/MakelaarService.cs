using Funda.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Funda.Application.Services
{
    internal class MakelaarService : IMakelaarService
    {
        public async Task<List<Makelaar>> GetTopTenMakelaarsFor(string city, bool propertiesWithGarden)
        {
            return [];
        }
    }
}
