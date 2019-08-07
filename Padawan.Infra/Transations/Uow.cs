using NHibernate;
using Padawan.Infra.Context;
using System.Threading.Tasks;

namespace Padawan.Infra.Transations
{
    public class Uow : IUow
    {
        private readonly ISession _session;
        public Uow()
        {
            _session = PadawanNHibernateHelper.OpenSession();
        }

        public async Task Commit()
        {
          await _session.FlushAsync();

            if (_session.Transaction.IsActive)
                _session.Transaction.Commit();
        }

        public ISession GetSession()
        {
            return _session;
        }

        public void OpenTransaction()
        {
            if(!_session.Transaction.IsActive)
            _session.BeginTransaction();
        }

        public void Rollback()
        {
            if (_session.Transaction.IsActive)
                _session.Transaction.Rollback();
        }
    }
}
