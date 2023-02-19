using RestSharp;
using System.Net;
using System.Text.Json;

namespace RestSharpAPITests
{
    public class RestSharpAPI_Tests
    {
        private RestClient client; 
        private const string baseUrl = "https://TaskBoardJS.alexlb1.repl.co/api";
        
        [SetUp]
        public void Setup()
        {
            this.client = new RestClient(baseUrl);
        }

        [Test]
        public void Test_GetDoneTasks_CheckTitle()
        {
            //Arrange
            var request = new RestRequest("/tasks/board/done", Method.Get); 
            //Act
            var response = client.Execute(request);
            //Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var tasks = JsonSerializer.Deserialize<List<Task>>(response.Content);
            Assert.That(tasks[0].title, Is.EqualTo("Project skeleton"));

        }

        [Test]
        public void Test_SearchByKeyWord_ValidResults()
        {
            //Arrange
            var request = new RestRequest("/tasks/search/home", Method.Get);
            //Act
            var response = client.Execute(request);
            //Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var tasks = JsonSerializer.Deserialize<List<Task>>(response.Content);
            Assert.That(tasks[0].title, Is.EqualTo("Home page"));

        }

        [Test]
        public void Test_SearchByKeyWord_InvalidResults()
        {
            //Arrange
            var request = new RestRequest("/tasks/search/missing" + DateTime.Now.Ticks, Method.Get);
            //Act
            var response = client.Execute(request);
            //Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
           Assert.That(response.Content, Is.EqualTo("[]"));

        }

        [Test]
        public void Test_CreatedNewTask_InvalidResults()
        {
            //Arrange
            var request = new RestRequest("/tasks" , Method.Post);
            //Act
            var response = client.Execute(request);
            var reqBody = new
            {
                Description = "description",
                board = "Open"
            };

            request.AddBody(reqBody);
            //Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Is.EqualTo("{\"errMsg\":\"Title cannot be empty!\"}"));

        }

        [Test]
        public void Test_CreatedNewTask_ValidBody()
        {
            //Arrange
            var request = new RestRequest("tasks", Method.Post);
          
            var reqBody = new
            {
                title = "REstsharp: some title" + DateTime.Now.Ticks,
                Description = "description",
                board = "Open"
            };

            request.AddBody(reqBody);

            //Act
            var response = client.Execute(request);
            var tasksObject = JsonSerializer.Deserialize<taskObject>(response.Content);
            //Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(tasksObject.msg, Is.EqualTo("Task added."));
            Assert.That(tasksObject.task.id, Is.GreaterThan(0));
            Assert.That(tasksObject.task.title, Is.EqualTo(reqBody.title));
            Assert.That(tasksObject.task.description, Is.EqualTo(reqBody.Description));
            Assert.That(tasksObject.task.board.id, Is.EqualTo(1001));
            Assert.That(tasksObject.task.board.name, Is.EqualTo("Open"));
            Assert.That(tasksObject.task.dateCreated, Is.Not.Empty);
            Assert.That(tasksObject.task.dateModified, Is.Not.Empty);

          
            

        }
    }
}