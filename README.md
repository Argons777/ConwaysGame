# ConwaysGame
## Database
To create the database go to folder "./ConwayGame.Infrastructure/" and run the following instruction:
`dotnet ef database update --context ConwayGame.Infrastructure.Data.ConwayDbContext --startup-project ../ConwayGame.Api`

## Swagger
To access Swagger go to http://localhost:5096/swagger/index.html