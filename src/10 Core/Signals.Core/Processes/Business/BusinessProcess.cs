using Signals.Core.Processes.Base;
using Signals.Core.Processing.Results;

namespace Signals.Core.Processes.Business
{
    /// <summary>
    /// Represents a business process
    /// </summary>
    public interface IBusinessProcess { }

    /// <summary>
    /// Represents a business process
    /// </summary>
    public abstract class BusinessProcess<TResponse> : BaseProcess<TResponse>,
        IBusinessProcess
        where TResponse : VoidResult, new()
    {
        /// <summary>
        /// Business process context
        /// </summary>
        protected virtual BusinessProcessContext Context { get; set; }

        /// <summary>
        /// Base process context upcasted from Business process context
        /// </summary>
        internal override BaseProcessContext BaseContext => Context;

        /// <summary>
        /// CTOR
        /// </summary>
        protected BusinessProcess()
        {
            Context = new BusinessProcessContext();
        }

        /// <summary>
        /// Authentication and authorization layer
        /// </summary>
        /// <returns></returns>
        public abstract TResponse Auth();

        /// <summary>
        /// Validation layer
        /// </summary>
        /// <returns></returns>
        public abstract TResponse Validate();

        /// <summary>
        /// Execution layer
        /// </summary>
        /// <returns></returns>
        public abstract TResponse Handle();

        /// <summary>
        /// Execution using base strategy
        /// </summary>
        /// <returns></returns>
        internal virtual TResponse Execute()
        {
            var result = Auth();
            if (result.IsFaulted) return result;

            result = Validate();
            if (result.IsFaulted) return result;

            result = Handle();
            return result;
        }

        /// <summary>
        /// Entry point executed by the factory
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal override TResponse ExecuteProcess(params object[] args)
        {
            return Execute();
        }
    }

    /// <summary>
    /// Represents a business process
    /// </summary>
    public abstract class BusinessProcess<T1, TResponse> : BaseProcess<TResponse>,
        IBusinessProcess
        where TResponse : VoidResult, new()
    {
        /// <summary>
        /// Business process context
        /// </summary>
        protected virtual BusinessProcessContext Context { get; set; }

        /// <summary>
        /// Base process context upcasted from Business process context
        /// </summary>
        internal override BaseProcessContext BaseContext => Context;

        /// <summary>
        /// CTOR
        /// </summary>
        protected BusinessProcess()
        {
            Context = new BusinessProcessContext();
        }

        /// <summary>
        /// Authentication and authorization layer
        /// </summary>
        /// <param name="obj1"></param>
        /// <returns></returns>
        public abstract TResponse Auth(T1 obj1);

        /// <summary>
        /// Validation layer
        /// </summary>
        /// <param name="obj1"></param>
        /// <returns></returns>
        public abstract TResponse Validate(T1 obj1);

        /// <summary>
        /// Execution layer
        /// </summary>
        /// <param name="obj1"></param>
        /// <returns></returns>
        public abstract TResponse Handle(T1 obj1);

        /// <summary>
        /// Execution using base strategy
        /// </summary>
        /// <param name="obj1"></param>
        /// <returns></returns>
        internal virtual TResponse Execute(T1 obj1)
        {
            var result = Auth(obj1);
            if (result.IsFaulted) return result;

            result = Validate(obj1);
            if (result.IsFaulted) return result;

            result = Handle(obj1);
            return result;
        }

        /// <summary>
        /// Entry point executed by the factory
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal override TResponse ExecuteProcess(params object[] args)
        {
            var obj1 = (T1)args[0];
            return Execute(obj1);
        }
    }

    /// <summary>
    /// Represents a business process
    /// </summary>
    public abstract class BusinessProcess<T1, T2, TResponse> : BaseProcess<TResponse>,
        IBusinessProcess
        where TResponse : VoidResult, new()
    {
        /// <summary>
        /// Business process context
        /// </summary>
        protected virtual BusinessProcessContext Context { get; set; }

        /// <summary>
        /// Base process context upcasted from Business process context
        /// </summary>
        internal override BaseProcessContext BaseContext => Context;

        /// <summary>
        /// CTOR
        /// </summary>
        protected BusinessProcess()
        {
            Context = new BusinessProcessContext();
        }

        /// <summary>
        /// Authentication and authorization layer
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public abstract TResponse Auth(T1 obj1, T2 obj2);

        /// <summary>
        /// Validation layer
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public abstract TResponse Validate(T1 obj1, T2 obj2);

        /// <summary>
        /// Execution layer
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public abstract TResponse Handle(T1 obj1, T2 obj2);

        /// <summary>
        /// Execution using base strategy
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        internal virtual TResponse Execute(T1 obj1, T2 obj2)
        {
            var result = Auth(obj1, obj2);
            if (result.IsFaulted) return result;

            result = Validate(obj1, obj2);
            if (result.IsFaulted) return result;

            result = Handle(obj1, obj2);
            return result;
        }

        /// <summary>
        /// Entry point executed by the factory
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal override TResponse ExecuteProcess(params object[] args)
        {
            var obj1 = (T1)args[0];
            var obj2 = (T2)args[1];
            return Execute(obj1, obj2);
        }
    }

    /// <summary>
    /// Represents a business process
    /// </summary>
    public abstract class BusinessProcess<T1, T2, T3, TResponse> : BaseProcess<TResponse>,
        IBusinessProcess
        where TResponse : VoidResult, new()
    {
        /// <summary>
        /// Business process context
        /// </summary>
        protected virtual BusinessProcessContext Context { get; set; }

        /// <summary>
        /// Base process context upcasted from Business process context
        /// </summary>
        internal override BaseProcessContext BaseContext => Context;

        /// <summary>
        /// CTOR
        /// </summary>
        protected BusinessProcess()
        {
            Context = new BusinessProcessContext();
        }

        /// <summary>
        /// Authentication and authorization layer
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <param name="obj3"></param>
        /// <returns></returns>
        public abstract TResponse Auth(T1 obj1, T2 obj2, T3 obj3);

        /// <summary>
        /// Validation layer
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <param name="obj3"></param>
        /// <returns></returns>
        public abstract TResponse Validate(T1 obj1, T2 obj2, T3 obj3);

        /// <summary>
        /// Execution layer
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <param name="obj3"></param>
        /// <returns></returns>
        public abstract TResponse Handle(T1 obj1, T2 obj2, T3 obj3);

        /// <summary>
        /// Execution using base strategy
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <param name="obj3"></param>
        /// <returns></returns>
        internal virtual TResponse Execute(T1 obj1, T2 obj2, T3 obj3)
        {
            var result = Auth(obj1, obj2, obj3);
            if (result.IsFaulted) return result;

            result = Validate(obj1, obj2, obj3);
            if (result.IsFaulted) return result;

            result = Handle(obj1, obj2, obj3);
            return result;
        }

        /// <summary>
        /// Entry point executed by the factory
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal override TResponse ExecuteProcess(params object[] args)
        {
            var obj1 = (T1)args[0];
            var obj2 = (T2)args[1];
            var obj3 = (T3)args[2];
            return Execute(obj1, obj2, obj3);
        }
    }
}