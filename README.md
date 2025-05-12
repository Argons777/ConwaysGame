# ConwaysGame
## 1. Database

The Api now uses PostgreSQL as its database management system (DBMS). This change was implemented due to the limitations of SQLite in terms of concurrency and other essential features for application scaling and performance.

### 1.1. Connection String
The PostgreSQL connection string is now configured through the appsettings.json file.

    "ConnectionStrings": {
        "Postgresql": "Host={Host};Port={Port};Database={Database};Username={Username};Password={Password}"
    }

### 1.2. Local development
For local development, a `docek-compose.dev.yml` file was added. This file runs the application along with a container running Postgresql server on port `5432`

### 1.3. Migrations
The application now automatically applies Entity Framework Core migrations on startup, but to apply them manually you need to go to folder `./ConwayGame.Infrastructure/` and run the following instruction:

`dotnet ef database update --context ConwayGame.Infrastructure.Data.ConwayDbContext --startup-project ../ConwayGame.Api`

## 2. Docker Support
Docker has been integrated to ease deployment and development. The `Dockerfile` is optimized for production releases using a multi-stage build, resulting in smaller, more efficient images.

### 2.1. Docker Compose (docker-compose.dev.yml)

A docker compose file was added to manage the Api and the database for local development
- Host access (local development)
  - **Api**: Accessible at `http://localhost:5000`
  - **PostgreSQL**: Accessible at `localhost:5432`

## 3. CI/CD Workflow
Now the project uses GitHub Actions to automate the build and test process. This ensures code quality and consistency.
The following scenarios will trigger the pipeline:
- Everytime a pull request is created targeting `main` branch
- Everytime a push to `main` is done

## 4. Swagger
To access Swagger go to http://localhost:5000/swagger/index.html