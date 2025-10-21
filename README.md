.NET Obligatory assignment ITPE3200

This project is our implementation of basic project task 3. Note that the game is not finished. 


## How to run:
In terminal write dotnet run. The program will be run at local host 5169. link : http://localhost:5169 NB! Check that you are in the DOTNET_PROJECT folder.

## Excpected game flow
1. You should be sent to "Auth/Login" when opening the local host link. Here you can either register your own account or use the admin account with following credentials : username = admin, password = admin.
2. After login you should be sent to "Home/index". Here you are given a home page. From here you can either play the game or use other features such as logout.
3. After pressing play you should be sent to "GameMvc/Play?nodeId=1". From here you can press choices buttons to progress futher into the story.
4. Since this is just a proof of concept there is no ending screen. To inform user the choice button will have the label "Game is over".

## Usage of AI : 

### General usage
Used Chatgpt5, Chatgpt4, visual Studio Copilot (Chatgpt 4o), Gemini 2.5 throuout the project to explain errors code and messages.   

### Services:
Used AI(GPT-5) to find errors and logic mistakes in the gameService to improve it.
### Repositories : 
Used the built in copilot (GPT-4) in vscode to reason if the repository methods that I created was necessary. This resulted in some repository features being removed due to being useless. \
Used ai (Chatgpt-5 Thinking) for logic check while designing the generic repository. Was given comments that changed the implementation of the update method.  
Used AI(Chatgpt-5 Thinking) on how to implement the health feature on player character. The ai reasoned that the health feature could be saved on a character if saved of type player character. The project implementation used this as the general idea when implementing the feature.   

Used AI(GPT-5) to write an generic method that fetches properties for the different entities, so we can remove and delete the duplicated getName or getDescription etc for all the repositories, and instead opt to use a generic method where we pass in the entity, and the property value we want.   
Used AI/GPT-5) for checking the logic and finding errors in the gameRepositories, for imporvment  

### Auth controller : 
Used AI (ChatGPT-5 Thinking) to help find a solution to retrieving a user id when logged in. The ai suggested implementing a solution with a “httpcontext session id”, set by the id of the user logged in. The project implementation was based on the ai suggestion, but changed to fit this project.   
Used AI (Chatgpt-5 thinking) for ideas to convert our logic from React based to .Net based. The feedback lead to a change with the controller methods having a viewmodel as a parameter instead of a dto.  The dto are still being used inside the method.    

### GameMvc controller : 
At first the game was supposed to be connected to a playercharacter, but was later on changed to the story node. Used AI(Chatgpt-5-thinking) to reason if our thought out logical change was beneficial, and if it would work. Our implementation was to adjust our previous implementation with the new story node logic.   

### GameController
Since the gamecontroller relies on logic from the gameservice and Repositories I used AI to find error mistakes that AI could more easily find. 
Frontend and images.  
ChatGPT-5 (image generation) was used to create all background and character-card artwork. All in-game images are AI-generated assets derived from our prompts, lightly edited and cropped. The images are as follows: Backgrounds : Airlock.png, awakening.png, corridor.png, encounter.png, exposedWires.png, hallway.png, HomePageImage.png, LogInImage.png, medkit.png, outsideCryopod.png, releaseHatch.png. Characters : darius.png, irene.png, narrator.png, protagonist.png and systemAI.png.


### play.cshtml layout
GPT-5 Thinking was used to troubleshoot and refine the Razor/CSS layout for the gameplay view (play.cshtml). Assistance included diagnosing grid sizing issues and scaling.


### Database creation and handling of seed data (AppDbConext.cs, DbSeeder.cs, Models.cs)
Used ChatGPT version 5 and 4 for initial creation of AppDbContext and DbSeeder files. Later on the versions were used to debug and diagnose problem that occurred with the database, seeder and the functions related.  


## Authors:
386100,
385564,
385411,
385539,
385542


