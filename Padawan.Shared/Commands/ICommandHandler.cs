
using System.Threading.Tasks;

namespace Padawan.Shared.Commands
{
    public interface ICommandHandler<T, T1> where T : ICommand where  T1 : ICommandResult
    {
        Task<T1> Handle(T command);
    }
}
