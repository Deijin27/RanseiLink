using CliFx;
using CliFx.Infrastructure;
using Core.Services;
using System.Threading.Tasks;

namespace RanseiConsole
{
    public abstract class BaseCommand : ICommand
    {
        protected BaseCommand(IServiceContainer container)
        {
            Container = container;
        }

        protected BaseCommand() : this(ServiceContainer.Instance)
        {
        }

        protected readonly IServiceContainer Container;

        public abstract ValueTask ExecuteAsync(IConsole console);
    }
}
