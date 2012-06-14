using System;
using System.Threading;

using NUnit.Framework;

namespace SqlToGraphite.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_ThreadImpl
    {
        [Test]
        public void Should_create_thread_start_and_stop()
        {
            var thing = new Thing();
            var threadImpl = new ThreadImpl(thing.Sleep);
            threadImpl.Start();
            Thread.Sleep(200);
            threadImpl.Abort();           
        }

        public class Thing
        {
            public void Sleep()
            {
                try
                {
                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    var x = ex.Message;
                    Console.WriteLine(ex.Message);
                    throw;
                }                
            }
        }
    }
}