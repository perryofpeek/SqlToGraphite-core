﻿using System.Collections.Generic;

using ConfigSpike.Config;

using NUnit.Framework;

using SqlToGraphite.Conf;

namespace SqlToGraphite.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_templates
    {
        [Test]
        public void Should_get_taskLists_from_templates_for_default()
        {
            var data = new List<WorkItems>();
            var workItem1 = new WorkItems { RoleName = "default", TaskSet = new List<ConfigSpike.Config.TaskSet>() };
            workItem1.TaskSet[0] = new ConfigSpike.Config.TaskSet { Frequency = 123 };
            data.Add(workItem1);
            //var templates = new Templates(data);
            var roles = new List<string> { "default" };
            //Test
            //var result = templates.GetTaskSetList(roles);
            //Assert
            //Assert.That(result.Count, Is.EqualTo(1));
            //Assert.That(result[0].frequency, Is.EqualTo("123"));
            Assert.Fail("fix this. ");
        }

        [Test]
        public void Should_get_taskLists_from_templates_for_default_and_hostname()
        {
            var data = new List<WorkItems>();
            var workItem1 = new WorkItems { RoleName = "default", TaskSet = new List<ConfigSpike.Config.TaskSet>() };
            workItem1.TaskSet[0] = new ConfigSpike.Config.TaskSet { Frequency = 123 };

            var workItem2 = new WorkItems { RoleName = "webserver", TaskSet = new List<ConfigSpike.Config.TaskSet>() };
            workItem2.TaskSet[0] = new ConfigSpike.Config.TaskSet { Frequency = 124 };

            data.Add(workItem1);
            data.Add(workItem2);

            var templates = new Templates1(data);
            var roles = new List<string> { "webserver", "default" };
            //Test
            var result = templates.GetTaskSetList(roles);
            //Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Frequency, Is.EqualTo(123));
            Assert.That(result[1].Frequency, Is.EqualTo(124));
        }

        [Test]
        public void Should_get_taskLists_from_templates_for_default_and_hostname_not_all()
        {
            var data = new List<WorkItems>();
            var workItem1 = new WorkItems { RoleName = "default", TaskSet = new List<ConfigSpike.Config.TaskSet>() };
            workItem1.TaskSet[0] = new ConfigSpike.Config.TaskSet { Frequency = 123 };

            var workItem2 = new WorkItems { RoleName = "webserver", TaskSet = new List<ConfigSpike.Config.TaskSet>() };
            workItem2.TaskSet[0] = new ConfigSpike.Config.TaskSet { Frequency = 124 };

            var workItem3 = new WorkItems { RoleName = "sqlserver", TaskSet = new List<ConfigSpike.Config.TaskSet>() };
            workItem3.TaskSet[0] = new ConfigSpike.Config.TaskSet { Frequency = 125 };

            data.Add(workItem1);
            data.Add(workItem2);
            data.Add(workItem3);

            var templates = new Templates1(data);
            var roles = new List<string> { "webserver", "default" };
            //Test
            var result = templates.GetTaskSetList(roles);
            //Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Frequency, Is.EqualTo(123));
            Assert.That(result[1].Frequency, Is.EqualTo("124"));
        }

        [Test]
        public void Should_not_merge_taskLists_from_templates_with_the_same_freq()
        {
            var data = new List<WorkItems>();
            var workItem1 = new WorkItems { RoleName = "default", TaskSet = new List<ConfigSpike.Config.TaskSet>() };
            var ts1 = new ConfigSpike.Config.TaskSet
                {
                    Frequency = 123,
                    Tasks = new List<ConfigSpike.Config.Task> { new ConfigSpike.Config.Task() { JobName = "a" } }
                };

            workItem1.TaskSet.Add(ts1);

            var workItem2 = new WorkItems { RoleName = "webserver", TaskSet = new List<ConfigSpike.Config.TaskSet>() };
            var ts2 = new ConfigSpike.Config.TaskSet
            {
                Frequency = 123,
                Tasks = new List<ConfigSpike.Config.Task> { new ConfigSpike.Config.Task() { JobName = "a" } }
            };

            workItem2.TaskSet.Add(ts2);
                     
            data.Add(workItem1);
            data.Add(workItem2);

            var templates = new Templates1(data);
            var roles = new List<string> { "webserver", "default" };
            //Test
            var result = templates.GetTaskSetList(roles);
            //Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.Fail("fix this");
            //Assert.That(result[0].frequency, Is.EqualTo("123"));
            //Assert.That(result[0].Task.Length, Is.EqualTo(1));
            //Assert.That(result[0].Task[0].name, Is.EqualTo("a"));
            //Assert.That(result[1].frequency, Is.EqualTo("123"));
            //Assert.That(result[1].Task.Length, Is.EqualTo(1));
            //Assert.That(result[1].Tasks[0].name, Is.EqualTo("b"));
        }
    }
}