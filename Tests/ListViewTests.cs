﻿using System.Linq;
using DemoApp;
using NUnit.Framework;
using QuickTest;

namespace Tests
{
    public class ListViewTests : IntegrationTest<App>
    {
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();

            OpenMenu("ListViews");
            ShouldSee("ListView demo");
        }

        protected override void TearDown()
        {
            ShouldSee("ListView demo");

            base.TearDown();
        }

        [Test]
        public void TestTextCell()
        {
            Assert.That(FindFirst("Item A1"), Is.Not.Null);
            Tap("Item A1");
            ShouldSee("Item A1 tapped");
            Tap("Ok");
        }

        [Test]
        public void TestStringViewCell()
        {
            Assert.That(FindFirst("Item A2"), Is.Not.Null);
            Tap("Item A2");
            ShouldSee("Item A2 tapped");
            Tap("Ok");
        }

        [Test]
        public void TestItemViewCell()
        {
            Assert.That(FindFirst("Item A3"), Is.Not.Null);
            Tap("Item A3");
            ShouldSee("Item A3 tapped");
            Tap("Ok");
        }

        [Test]
        public void TestGroups()
        {
            Tap("A4");
            ShouldSee("A4 tapped");

            Tap("Ok");
            Tap("A5");
            ShouldSee("A5 tapped");
            Tap("Ok");
        }

        [Test]
        public void TestHeadersAndFooters()
        {
            ShouldSee("plain header");
            ShouldSee("plain footer");
            ShouldSee("header label");
            ShouldSee("footer label");
        }
    }
}
