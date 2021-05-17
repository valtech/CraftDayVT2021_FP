namespace CraftDay.ToDo.Common.Services
{
  public abstract class ToDoServiceFactory
  {
    public abstract ToDoServiceFactory WithStore(IToDoStore store);
    public abstract IToDoService Build();
  }
}