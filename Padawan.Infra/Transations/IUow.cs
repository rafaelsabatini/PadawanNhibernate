using NHibernate;
using System.Threading.Tasks;

namespace Padawan.Infra.Transations
{
    public interface IUow
    {
        Task Commit();
        void Rollback();

        void OpenTransaction();

        ISession GetSession();
    }
}
