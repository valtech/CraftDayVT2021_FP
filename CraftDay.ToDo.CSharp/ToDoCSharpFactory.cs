using System;
using CraftDay.ToDo.Common.Services;

namespace CraftDay.ToDo.CSharp
{
    public class ToDoCSharpFactory : ToDoServiceFactory
    {
        private IToDoStore _store;
        public override ToDoServiceFactory WithStore(IToDoStore store)
        {
            _store = store;
            return this;
        }

        public override IToDoService Build()
        {
            return new ToDoCSharpService(_store);
        }
    }
}
