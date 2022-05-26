using System;
using System.Collections.Generic;

namespace MonogameTextEditor.TextEditor.UndoSystem
{
	public class UndoItemPool
	{
		private readonly Dictionary<Type, object> pools = new Dictionary<Type, object>();

		public T Acquire<T>()
			where T : IUndoItem, new()
		{
			var undoItem = GetPool<T>().Acquire();
			undoItem.PoolCreator = this;
			return undoItem;
		}

		public void Release<T>(T item)
			where T : IUndoItem, new()
		{
			GetPool<T>().Release(item);
		}

		private Pool<T> GetPool<T>()
			where T : IUndoItem, new()
		{
			var type = typeof(T);
			if (!pools.ContainsKey(type)) {
				pools.Add(type, new Pool<T>());
			}
			return (Pool<T>)pools[type];
		}

		private class Pool<T>
			where T : IUndoItem, new()
		{
			private readonly Stack<T> freeItems = new Stack<T>();

			public T Acquire()
			{
				if (freeItems.Count > 0) {
					return freeItems.Pop();
				}
				return new T();
			}

			public void Release(T command)
			{
				System.Diagnostics.Debug.Assert(command.GetType() == typeof(T));
				freeItems.Push(command);
			}
		}
	}
}
