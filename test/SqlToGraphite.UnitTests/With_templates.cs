using System.Collections.Generic;

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
            var data = new List<SqlToGraphiteConfigTemplatesWorkItems>();
            var workItem1 = new SqlToGraphiteConfigTemplatesWorkItems { Role = "default", TaskSet = new SqlToGraphiteConfigTemplatesWorkItemsTaskSet[1] };
            workItem1.TaskSet[0] = new SqlToGraphiteConfigTemplatesWorkItemsTaskSet { frequency = "123" };
            data.Add(workItem1);
            var templates = new Templates(data);
            var roles = new List<string> { "default" };
            //Test
            var result = templates.GetTaskSetList(roles);
            //Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].frequency, Is.EqualTo("123"));
        }

        [Test]
        public void Should_get_taskLists_from_templates_for_default_and_hostname()
        {
            var data = new List<SqlToGraphiteConfigTemplatesWorkItems>();
            var workItem1 = new SqlToGraphiteConfigTemplatesWorkItems { Role = "default", TaskSet = new SqlToGraphiteConfigTemplatesWorkItemsTaskSet[1] };
            workItem1.TaskSet[0] = new SqlToGraphiteConfigTemplatesWorkItemsTaskSet { frequency = "123" };

            var workItem2 = new SqlToGraphiteConfigTemplatesWorkItems { Role = "webserver", TaskSet = new SqlToGraphiteConfigTemplatesWorkItemsTaskSet[1] };
            workItem2.TaskSet[0] = new SqlToGraphiteConfigTemplatesWorkItemsTaskSet { frequency = "124" };

            data.Add(workItem1);
            data.Add(workItem2);

            var templates = new Templates(data);
            var roles = new List<string> { "webserver", "default" };
            //Test
            var result = templates.GetTaskSetList(roles);
            //Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].frequency, Is.EqualTo("123"));
            Assert.That(result[1].frequency, Is.EqualTo("124"));
        }

        [Test]
        public void Should_get_taskLists_from_templates_for_default_and_hostname_not_all()
        {
            var data = new List<SqlToGraphiteConfigTemplatesWorkItems>();
            var workItem1 = new SqlToGraphiteConfigTemplatesWorkItems { Role = "default", TaskSet = new SqlToGraphiteConfigTemplatesWorkItemsTaskSet[1] };
            workItem1.TaskSet[0] = new SqlToGraphiteConfigTemplatesWorkItemsTaskSet { frequency = "123" };

            var workItem2 = new SqlToGraphiteConfigTemplatesWorkItems { Role = "webserver", TaskSet = new SqlToGraphiteConfigTemplatesWorkItemsTaskSet[1] };
            workItem2.TaskSet[0] = new SqlToGraphiteConfigTemplatesWorkItemsTaskSet { frequency = "124" };

            var workItem3 = new SqlToGraphiteConfigTemplatesWorkItems { Role = "sqlserver", TaskSet = new SqlToGraphiteConfigTemplatesWorkItemsTaskSet[1] };
            workItem3.TaskSet[0] = new SqlToGraphiteConfigTemplatesWorkItemsTaskSet { frequency = "125" };

            data.Add(workItem1);
            data.Add(workItem2);
            data.Add(workItem3);

            var templates = new Templates(data);
            var roles = new List<string> { "webserver", "default" };
            //Test
            var result = templates.GetTaskSetList(roles);
            //Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].frequency, Is.EqualTo("123"));
            Assert.That(result[1].frequency, Is.EqualTo("124"));
        }

        [Test]
        public void Should_not_merge_taskLists_from_templates_with_the_same_freq()
        {
            var data = new List<SqlToGraphiteConfigTemplatesWorkItems>();
            var workItem1 = new SqlToGraphiteConfigTemplatesWorkItems { Role = "default", TaskSet = new SqlToGraphiteConfigTemplatesWorkItemsTaskSet[1] };
            workItem1.TaskSet[0] = new SqlToGraphiteConfigTemplatesWorkItemsTaskSet { frequency = "123", Task = new SqlToGraphiteConfigTemplatesWorkItemsTaskSetTask[1] };
            workItem1.TaskSet[0].Task[0] = new SqlToGraphiteConfigTemplatesWorkItemsTaskSetTask { name = "a" };

            var workItem2 = new SqlToGraphiteConfigTemplatesWorkItems { Role = "webserver", TaskSet = new SqlToGraphiteConfigTemplatesWorkItemsTaskSet[1] };
            workItem2.TaskSet[0] = new SqlToGraphiteConfigTemplatesWorkItemsTaskSet { frequency = "123", Task = new SqlToGraphiteConfigTemplatesWorkItemsTaskSetTask[1] };
            workItem2.TaskSet[0].Task[0] = new SqlToGraphiteConfigTemplatesWorkItemsTaskSetTask { name = "b" };
            data.Add(workItem1);
            data.Add(workItem2);

            var templates = new Templates(data);
            var roles = new List<string> { "webserver", "default" };
            //Test
            var result = templates.GetTaskSetList(roles);
            //Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].frequency, Is.EqualTo("123"));
            Assert.That(result[0].Task.Length, Is.EqualTo(1));
            Assert.That(result[0].Task[0].name, Is.EqualTo("a"));
            Assert.That(result[1].frequency, Is.EqualTo("123"));
            Assert.That(result[1].Task.Length, Is.EqualTo(1));
            Assert.That(result[1].Task[0].name, Is.EqualTo("b"));
        }
    }
}