using FluentAssertions;
using NUnit.Framework;

namespace SpecificationFor.SpecificationForFixtures
{
  class SimpleTestWithReturn : SpecFor<SimpleTestWithReturn.SubjectUnderTest, int>
  {
    [OneTimeSetUp]
    public void SetUp() => ArrangeAndAct();
    
    protected override int Act(SubjectUnderTest subject) => subject.MethodUnderTest();

    [Test]
    public void SubjectUnderTestIsCreated()
      => Subject.Should().NotBeNull();

    [Test]
    public void ResultFromActionIsReturned()
      => Result.Should().Be(5);
    
    public class SubjectUnderTest
    {
      public int MethodUnderTest() => 5;
    }
  }
}