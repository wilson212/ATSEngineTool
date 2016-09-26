// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
/*============================================================
**
** Class:  SyncProgress<T>
** 
** Modifed to use SynchronizationContext.Send instead of SynchronizationContext.Post
**
**
** Purpose: Event-based implementation of IProgress<T>.
**
**
===========================================================*/

using System;
using System.Threading;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace System
{
    class SyncProgress<T> : IProgress<T>
    {
        /// <summary>The synchronization context captured upon construction.  This will never be null.</summary>
        private readonly SynchronizationContext m_synchronizationContext;
        /// <summary>The handler specified to the constructor.  This may be null.</summary>
        private readonly Action<T> m_handler;
        /// <summary>A cached delegate used to post invocation to the synchronization context.</summary>
        private readonly SendOrPostCallback m_invokeHandlers;
 
        /// <summary>Initializes the <see cref="SyncProgress{T}"/>.</summary>
        public SyncProgress()
        {
            // Capture the current synchronization context.  "current" is determined by CurrentNoFlow,
            // which doesn't consider the [....] ctx flown with an ExecutionContext, avoiding
            // [....] ctx reference identity issues where the [....] ctx for one thread could be Current on another.
            // If there is no current context, we use a default instance targeting the ThreadPool.
            m_synchronizationContext = SynchronizationContext.Current ?? ProgressStatics.DefaultContext;
            Contract.Assert(m_synchronizationContext != null);
            m_invokeHandlers = new SendOrPostCallback(InvokeHandlers);
        }
 
        /// <summary>Initializes the <see cref="SyncProgress{T}"/> with the specified callback.</summary>
        /// <param name="handler">
        /// A handler to invoke for each reported progress value.  This handler will be invoked
        /// in addition to any delegates registered with the <see cref="ProgressChanged"/> event.
        /// Depending on the <see cref="System.Threading.SynchronizationContext"/> instance captured by 
        /// the <see cref="Progress"/> at construction, it's possible that this handler instance
        /// could be invoked concurrently with itself.
        /// </param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="handler"/> is null (Nothing in Visual Basic).</exception>
        public SyncProgress(Action<T> handler) : this()
        {
            if (handler == null) throw new ArgumentNullException("handler");
            m_handler = handler;
        }
 
        /// <summary>Raised for each reported progress value.</summary>
        /// <remarks>
        /// Handlers registered with this event will be invoked on the 
        /// <see cref="System.Threading.SynchronizationContext"/> captured when the instance was constructed.
        /// </remarks>
        public event EventHandler<T> ProgressChanged;
 
        /// <summary>Reports a progress change.</summary>
        /// <param name="value">The value of the updated progress.</param>
        protected virtual void OnReport(T value)
        {
            // If there's no handler, don't bother going through the [....] context.
            // Inside the callback, we'll need to check again, in case 
            // an event handler is removed between now and then.
            Action<T> handler = m_handler;
            EventHandler<T> changedEvent = ProgressChanged;
            if (handler != null || changedEvent != null)
            {
                // Post the processing to the [....] context.
                // (If T is a value type, it will get boxed here.)
                m_synchronizationContext.Send(m_invokeHandlers, value);
            }
        }
 
        /// <summary>Reports a progress change.</summary>
        /// <param name="value">The value of the updated progress.</param>
        void IProgress<T>.Report(T value) { OnReport(value); }
 
        /// <summary>Invokes the action and event callbacks.</summary>
        /// <param name="state">The progress value.</param>
        private void InvokeHandlers(object state)
        {
            T value = (T)state;
 
            Action<T> handler = m_handler;
            EventHandler<T> changedEvent = ProgressChanged;
 
            if (handler != null) handler(value);
            if (changedEvent != null) changedEvent(this, value);
        }
    }
 
    /// <summary>Holds static values for <see cref="Progress{T}"/>.</summary>
    /// <remarks>This avoids one static instance per type T.</remarks>
    internal static class ProgressStatics
    {
        /// <summary>A default synchronization context that targets the ThreadPool.</summary>
        internal static readonly SynchronizationContext DefaultContext = new SynchronizationContext();
    }
}
