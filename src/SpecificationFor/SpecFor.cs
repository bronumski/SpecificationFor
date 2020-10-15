using System;

namespace SpecificationFor
{
  public abstract class SpecFor<TSut>
  {
    protected void ArrangeAndAct()
    {
      Subject = Activator.CreateInstance<TSut>();

      Act(Subject);
    }

    protected abstract void Act(TSut subject);

    protected TSut Subject { get; private set; }
  }

  public abstract class SpecFor<TSut, TResult>
  {
    protected TResult Result { get; private set; }
    protected void ArrangeAndAct()
    {
      Subject = Activator.CreateInstance<TSut>();

      Result = Act(Subject);
    }

    protected abstract TResult Act(TSut subject);

    protected TSut Subject { get; private set; }
  }
}