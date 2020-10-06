using System;
using FluentAssertions;
using NUnit.Framework;

namespace SpecificationFor.SpecificationForFixtures
{
  class SimpleTest : SpecFor<SimpleTest.SubjectUnderTest>
  {
    [SetUp]
    public void SetUp() => ArrangeAndAct();
    
    [Test]
    public void SubjectUnderTestIsCreated()
      => Subject.Should().NotBeNull();
    public class SubjectUnderTest {}
  }
}