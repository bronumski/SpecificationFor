using System;

namespace SpecificationFor
{
  public class SpecFor<TSut>
  {
    protected void ArrangeAndAct()
    {
      Subject = (TSut) Activator.CreateInstance<TSut>();
    }

    protected TSut Subject { get; private set; }
  }
}