using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BasicTest
{
    [Test]
    public void TwoAndTwo_Add_IsFour()
    {
        Assert.That(2+2, Is.EqualTo(4));
    }
}
