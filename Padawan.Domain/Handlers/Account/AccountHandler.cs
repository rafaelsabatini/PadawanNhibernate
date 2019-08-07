using Flunt.Notifications;
using Padawan.Domain.Commands;
using Padawan.Domain.Entities;
using Padawan.Domain.Repositories;
using Padawan.Shared.Commands;
using Padawan.Shared.Messages;
using System.Threading.Tasks;

namespace Padawan.Domain.Handlers
{
    public class AccountHandler : Notifiable,
        ICommandHandler<CreateAccountCommand, AccountCommandResult>,
        ICommandHandler<EditAccountCommand, AccountCommandResult>,
        ICommandHandler<DeleteAccountCommand, AccountCommandResult>
    {
        private readonly IAccountRepository _accountRepository;
        public AccountHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public async Task<AccountCommandResult> Handle(CreateAccountCommand command)
        {
           
            Account account = new Account(command.Name,command.CreatedByUserId);

            AddNotifications(account.Notifications);

             await _accountRepository.Create(account);

            return account.ToDto();
           
        }

        public async Task<AccountCommandResult> Handle(EditAccountCommand command)
        {
            Account account = await _accountRepository.GetById(command.Id);
            if (account == null)
            {
                AddNotification("command", Messages.Account_NOT_FOUND);
                return null;
            }

            account.Update(command.Name);
            AddNotifications(account.Notifications);

            await _accountRepository.Update(account);

            return account.ToDto();
               
        }

        public async Task<AccountCommandResult> Handle(DeleteAccountCommand command)
        {
            Account account =  await _accountRepository.GetById(command.Id);
            if (account == null)
            {
                AddNotification("command", Messages.Account_NOT_FOUND);
                return null;
            }

            await _accountRepository.Delete(account);

            return account.ToDto();
        }
    }
}
