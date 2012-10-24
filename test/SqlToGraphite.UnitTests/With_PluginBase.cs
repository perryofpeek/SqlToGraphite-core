using System;
using System.Collections.Generic;

using NUnit.Framework;

using SqlToGraphiteInterfaces;

namespace SqlToGraphite.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_PluginBase
    {
        internal class TestPlugin : PluginBase
        {
            public string Secret
            {
                get
                {
                    return this.Decrypt(SecretValue);
                }

                set
                {
                    SecretValue = this.Encrypt(value);
                }
            }

            public string SecretValue { get; set; }

            public override IList<IResult> Get()
            {
                return new List<IResult>();
            }
        }

        [Test, Ignore()]
        public void Create_encrypted_string_for_testing()
        {
            var secret = "SomeCs";
            var testPlugin = new TestPlugin { Secret = secret };
            //Console.WriteLine(testPlugin.SecretValue);
            //Assert.That(testPlugin.Secret, Is.EqualTo(secret));            
            //Assert.That(testPlugin.SecretValue, Is.EqualTo(secretValue));           
        }

        [Test]
        public void Should_encrypt_string()
        {
            var secret = "abc123";
            string secretValue = "Myu0uW3E+PbIbaFkSIrLwA==";
            var testPlugin = new TestPlugin { Secret = secret };
            Assert.That(testPlugin.Secret, Is.EqualTo(secret));
            Assert.That(testPlugin.SecretValue, Is.EqualTo(secretValue));
        }
    }
}