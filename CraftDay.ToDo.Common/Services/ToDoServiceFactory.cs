namespace CraftDay.ToDo.Common.Services
{
  public abstract class ToDoServiceFactory
  {
    public ToDoServiceFactory()
    {
      throw new System.NotImplementedException();
    }

    public ToDoServiceFactory withStore(IToDoStore store)
    {
      throw new System.NotImplementedException();
    }
    
    public IToDoService Build() {
      throw new System.NotImplementedException();
    }
  }
}