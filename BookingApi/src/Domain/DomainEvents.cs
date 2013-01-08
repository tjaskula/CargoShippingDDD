using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;

namespace BookingApi.Domain
{
	/// <summary>
	/// Provides logic for raising and handling domain events.
	/// </summary>
	public static class DomainEvents
	{
		[ThreadStatic]
		private static List<Delegate> _actions;
		private static List<Delegate> Actions
		{
			get
			{
				if (_actions == null)
				{
					_actions = new List<Delegate>();
				}
				return _actions;
			}
		}

		/// <summary>
		/// Register the process for event handling.
		/// </summary>
		/// <param name="callback">Process for event handling.</param>
		/// <returns></returns>
		public static IDisposable Register<T>(Action<T> callback)
		{
			Actions.Add(callback);
			return new DomainEventRegistrationRemover(() => Actions.Remove(callback));
		}

		/// <summary>
		/// Notifies the event.
		/// </summary>
		public static void Raise<T>(T eventArgs)
		{
			// TODO : Refactor it, remove ServiceLocation
			//IEnumerable<IEventHandler<T>> registeredHandlers = ServiceLocator.Current.GetAllInstances<IEventHandler<T>>();
			//foreach (IEventHandler<T> handler in registeredHandlers)
			//{
			//    handler.Handle(eventArgs);
			//}

			//foreach (Action action in Actions)
			//{
			//    var typedAction = action as Action<T>;
			//    if (typedAction != null)
			//    {
			//        typedAction(eventArgs);
			//    }
			//}
		}

		/// <summary>
		/// Helper class.
		/// </summary>
		private sealed class DomainEventRegistrationRemover : IDisposable
		{
			private readonly Action _callOnDispose;

			public DomainEventRegistrationRemover(Action toCall)
			{
				_callOnDispose = toCall;
			}

			public void Dispose()
			{
				_callOnDispose();
			}
		}
	}
}