# CoremakerChallenge

Can be run by:
- Installing .NET 6 and using the "dotnet run" CLI command while in the folder where ".csproj" file is located or by providing the location of the ".csproj" file
- As a docker container using the Dockerfile
- Using docker-compose

The Postman collection contains the callable routes and it also has several variables for easy of use:
- SERVICE_URL - should contain the URL of the service and is used by all the requests in the collection
- JWT_TOKEN - contains the JWT token used for the GET requests and is automatically populated after a successful "Login" request

The integration tests can be run by running "dotnet test" CLI command in the "Coremaker.IntegrationTests" folder or by providing the location of the ".csproj" file
