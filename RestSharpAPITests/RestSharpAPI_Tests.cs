using RestSharp;
using System.Net;
using System.Text.Json;

//API Endpoints
//TaskBoard exposes a RESTful API, available at:
//https://taskboardjs.alexlb1.repl.co/api or in your case http://{yoursite}/api
//The following endpoints are supported:
//•	GET / api – list all API endpoints
//•	GET /api/tasks – list all tasks (returns JSON array of tasks)
//•	GET / api / tasks / id – returns a task by given id
//•	GET /api/tasks/search/keyword – list all tasks matching given keyword
//•	GET /api/tasks/board/boardName – list tasks by board name
//•	POST /api/tasks – create a new task(post a JSON object in the request body, e.g. {"title":"Add Tests", "description":"API + UI tests", "board":"Open"})
//•	PATCH / api / tasks / id – edit task by id (send a JSON object in the request body, holding the fields to modify, e.g. {"title":"changed title", "board":"Done"})
//•	DELETE / api / tasks / id – delete task by id
//•	GET /api/boards – list all boards

//RESTful API: RestSharp API Tests
//Your task is to write automated tests in C# for certain RESTful API endpoints. You should implement the following automated tests (35 points):
//•	List the tasks and assert that the first task from board "Done" has title "Project skeleton" (8 points).
//•	Find tasks by keyword "home" and assert that the first result has title "Home page" (5 points).
//•	Find tasks by keyword "missing{randnum}" and assert that the results are empty (5 points).
//•	Try to create a new task, holding invalid data, and assert an error is returned (5 points).
//•	Create a new task, holding valid data, and assert the new task is successfully created with respective properties (12 points).


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
