using DataAccessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.IRepo
{
    public interface IBoothRepo:IGenericRepo<Booth>
    {
        Task CreateBooth(Booth booth);
    }
}
