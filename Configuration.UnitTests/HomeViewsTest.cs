using NUnit.Framework;

[TestFixture]
public class DisplayClientsViewsTest
{
    [Test]
    public void Index()
    {
        // Instantiate the view directly. This is made possible by
        // the fact that we precompiled it
        //var view = new Index();

        // Set up the data that needs to be accessed by the view
        //view.ViewBag.Message = "Testing";

        // Render it in an HtmlAgilityPack HtmlDocument. Note that
        // you can pass a 'model' object here if your view needs one.
        // Generally, what you do here is similar to how a controller
        //action sets up data for its view.
        //HtmlDocument doc = view.RenderAsHtml();

        // Use the HtmlAgilityPack object model to verify the view.
        // Here, we simply check that the first <h2> tag contains
        // what we put in view.ViewBag.Message
        //HtmlNode node = doc.DocumentNode.Element("h2");
        //Assert.AreEqual("Testing", node.InnerHtml.Trim());
    }
}