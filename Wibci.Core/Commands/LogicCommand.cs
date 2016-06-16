using System.Threading.Tasks;
using Wibci.Core.Commands;

namespace Wibci.Core
{
    public interface IAsyncLogicCommand<TIn, TOut> : ILogicCommand<TIn, TOut> where TOut : CommandResult
    {
        Task<TOut> ExecuteAsync(TIn request);
    }

    public interface ILogicCommand<in TIn, out TOut>
    {
        TOut Execute(TIn request);
    }

    /// <summary>
    /// Base class for which async commands can inherit.
    /// Created to enforce single responsibility design principal in application logic implementation
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    public abstract class AsyncLogicCommand<TIn, TOut> : LogicCommand<TIn, TOut>, IAsyncLogicCommand<TIn, TOut> where TOut : CommandResult
    {
        public override TOut Execute(TIn request)
        {
            TOut result = AsyncHelper.RunSync(() => ExecuteAsync(request));
            return result;
        }

        public abstract Task<TOut> ExecuteAsync(TIn request);
    }

    /// <summary>
    /// Base class for which commands can inherit.
    /// Created to enforce single responsibility design principal in application logic implementation
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    public abstract class LogicCommand<TIn, TOut> : ILogicCommand<TIn, TOut> where TOut : CommandResult
    {
        public abstract TOut Execute(TIn request);
    }
}