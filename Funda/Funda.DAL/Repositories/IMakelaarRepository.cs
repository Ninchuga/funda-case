using Funda.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Funda.DAL.Repositories
{
    public interface IMakelaarRepository
    {
        Task<List<Makelaar>> GetTopTenMakelaarsFor(string city, bool propertiesWithGarden);
    }
}
