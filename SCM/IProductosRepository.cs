using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCM
{
    public interface IProductosRepository
    {
        Task<List<Product>> GetProduct();
    }


}

