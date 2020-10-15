using System;
using FluentAssertions;
using NUnit.Framework;

namespace SpecificationFor.SpecificationForFixtures
{
  class SimpleTest : SpecFor<SimpleTest.SubjectUnderTest>
  {
    [OneTimeSetUp]
    public void SetUp() => ArrangeAndAct();
    
    protected override void Act(SubjectUnderTest subject) => subject.MethodUnderTest();

    [Test]
    public void SubjectUnderTestIsCreated()
      => Subject.Should().NotBeNull();

    [Test]
    public void ActionOnSubjectIsInvoked()
      => Subject.WasCalled.Should().BeTrue();

    public class SubjectUnderTest
    {
      public bool WasCalled { get; private set; }
      public void MethodUnderTest() => WasCalled = true;
    }
  }
}