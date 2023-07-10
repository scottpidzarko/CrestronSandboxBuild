using System;
using Microsoft.Build
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SignForCrestronSandboxTask;

namespace TestSignForCrestronSandboxTask
{
    [TestClass]
    public class GenerateNetCfCompatibleCsprojTaskTests
    {
        [TestMethod]
        public void FullTest()
        {
            GenerateNetCfCompatibleCsprojTask task = new GenerateNetCfCompatibleCsprojTask();

            //task.Execute();
        }
    }
}
