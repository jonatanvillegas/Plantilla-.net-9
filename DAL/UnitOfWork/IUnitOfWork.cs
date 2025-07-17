using DAL.Repositories.Repository.Academico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWork
{
    public interface IUnitOfWork:IDisposable
    {
        IAcademicoRepository Academico { get;}
    }
}
