using NHibernate;
using NHibernate.Linq;
using Padawan.Domain.Entities;
using Padawan.Domain.Repositories;
using Padawan.Infra.Transations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Padawan.Infra.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IUow _uow;

        public AccountRepository(IUow uow)
        {
            _uow = uow;
        }

        public async Task Create(Account account)
        {
            await _uow.GetSession().SaveAsync(account);
            await _uow.GetSession().FlushAsync();
        }

        public async Task Update(Account account)
        {
            await _uow.GetSession().UpdateAsync(account);
            await _uow.GetSession().FlushAsync();
        }

        public async Task Delete(Account account)
        {
            await _uow.GetSession().DeleteAsync(account);
            await _uow.GetSession().FlushAsync();
        }

        public async Task<List<Account>> GetListBy(Expression<Func<Account, bool>> expression, int page, int pageSize)
        {
            return await _uow.GetSession().Query<Account>().Skip(page).Take(pageSize).Where(expression).ToListAsync();
        }

        public async Task<List<Account>> GetList(int page, int pageSize)
        {
            return await _uow.GetSession().Query<Account>().Skip(page).Take(pageSize).ToListAsync(); ;
        }

        public async Task<Account> GetById(long id)
        {
            _uow.GetSession().CacheMode = CacheMode.Normal;
            return await _uow.GetSession().GetAsync<Account>(id);
        }

        public Task<Account> GetBy(Expression<Func<Account, bool>> expression)
        {
            return _uow.GetSession().Query<Account>().Where(expression).AsQueryable().FirstOrDefaultAsync();
        }
    }
}