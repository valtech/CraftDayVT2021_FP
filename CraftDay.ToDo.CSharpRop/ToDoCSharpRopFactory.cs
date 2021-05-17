using CraftDay.ToDo.Common.Services;

namespace CraftDay.ToDo.CSharpRop
{
    public class ToDoCSharpRopFactory : ToDoServiceFactory
    {
        private IToDoStore _store;

        public override ToDoServiceFactory WithStore(IToDoStore store)
        {
            _store = store;
            return this;
        }

        public override IToDoService Build()
        {
            return new ToDoCSharpRopService(_store);
        }
    }
}
