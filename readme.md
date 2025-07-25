# Senior Engineer Take‑Home Exercise: People API

## Summary

Build and containerise a small .NET service, then spin up a self‑hosted TeamCity CI stack that builds it via Kotlin DSL.

**Environment**  
Assessed on Windows 11 with WSL 2 and Docker’s Linux backend. All images must be Linux (`linux/amd64`). Windows‑container images will not be accepted.

**Starter kit** – already provided

- Template solution with three projects: People.Api, People.Data, People.Tests
    
- EF Core In-Memory provider wired into People.Api
    
- Complete folder structure shown under Deliverables
    

Focus on implementing the logic, Docker image, and CI pipeline rather than scaffolding.

## Part 1 – API

- **Language**  C# (.NET 8 minimal API, target framework `net8.0`).
    
- **Endpoints** – implement endpoints (considering HTTP semantics) to perform the following functions:
    
    - Add
        
    - Update
        
    - Delete
        
    - List
        
- Model `{ Id: int, Name: string, DateOfBirth: DateOnly }` Use an in‑memory SQL database (e.g., EF Core InMemory provider - already setup).
    
- Add a self‑documenting endpoint (Swagger/OpenAPI).
    
- Implement a lightweight `/health` endpoint that returns HTTP 200 OK liveness checks.
    
- Provide unit tests in a dedicated People.Tests project under `src/` (xUnit + FluentAssertions or equivalent).
    

---

## Part 2 – Dockerfiles & Compose

### Runtime container

- Supply a multi‑stage `Dockerfile` that:
    
    1. Builds & self‑publishes for `linux‑x64` (trimmed/self‑contained if you target Alpine).
        
    2. Runs as non‑root in a lightweight base (e.g., `gcr.io/distroless/dotnet‑aspnet` or `mcr.microsoft.com/dotnet/aspnet:8.0‑alpine`).
        
- Container listens on 8080 internally and exposes the same port; honour `DOTNET_ENVIRONMENT` env var.
    
- Add a minimal `docker-compose.yml` that maps host 8080 to container 8080 (`8080:8080`) so `docker compose up` starts the API locally.
    

### Agent container

- Provide `Dockerfile.agent` based on `jetbrains/teamcity-agent:latest`; install the Docker CLI and ensure the agent mounts `/var/run/docker.sock` at runtime so build steps can invoke `docker build` and `docker push`. Your `compose.ci.yml` must reference this file via a `build:` section so the image is built automatically when reviewers run `docker compose -f compose.ci.yml up`.
    

---

## Part 3 – TeamCity CI stack (server + agent + pipeline)

- Provide a separate `compose.ci.yml` file that launches:
    
    - `teamcity-server` – based on `jetbrains/teamcity-server:latest`, listening on localhost:8111.
        
    - `teamcity-agent` – built automatically from `Dockerfile.agent` (which should start `FROM jetbrains/teamcity-agent:latest`). It must auto‑register with the server, be pre‑configured for .NET 8 builds, and include the Docker CLI while mounting `/var/run/docker.sock` so build steps can run `docker build` and `docker push`.
        
    - `registry` – lightweight local Docker registry (`registry`) listening on localhost:5000. Run in its default open (no‑auth) mode over HTTP; that’s acceptable for this local exercise. The pipeline must push the built image here.
        
- After the containers are up and the license is accepted, when we're assessing this we will:
    
    1. Add a VCS root pointing at the repository.
        
    2. Import the Kotlin DSL project (which you committed under `/.teamcity` - see next bullet).
        
- In `/.teamcity` create a single build configuration that:
    
    1. Restores, builds, and tests the solution.
        
    2. Builds the Docker image tagged `people-api:%build.number%`.
        
    3. Pushes the image (unauthenticated HTTP) to `localhost:5000/people-api:%build.number%` and stores `image.digest` as the build artifact.
        
- Provide a PowerShell script `build.ps1` containing a `ci-up` function/task that spins up the CI stack and waits until `http://localhost:8111` responds 200 OK.
    
- Document the one‑liner in the README:
    

```sh
Docker compose -f compose.ci.yml up -d && open http://localhost:8111
```

---

## Deliverables

```
├── .teamcity/
│   └── settings.kts         # Kotlin DSL build definition
├── src/
│   ├── People.Api/          # ASP.NET Core Minimal API project
│   ├── People.Data/         # EF Core in‑memory database layer
│   └── People.Tests/        # unit test project
├── build.ps1                # ci-up helper
├── compose.ci.yml           # spins up TeamCity server + agent + registry
├── docker-compose.yml       # runs the API locally
├── Dockerfile               # runtime image for the API
├── Dockerfile.agent         # custom agent with Docker CLI
└── README.md                # This document - replace with your own comments
```

`docker compose up` must start the API and `/health` should return 200 OK. Running `docker compose -f compose.ci.yml up -d` must bring up TeamCity so we can watch the pipeline execute.

---

## Evaluation criteria

|Area|What we look for|
|---|---|
|Code clarity & idiomatic C#|Separation of concerns, correct HTTP codes, validation & maintainable code|
|Tests & docs|Useful test cases, swagger docs, readme with clear instructions|
|Docker image hygiene|Small, non‑root, multi‑stage, no secrets|
|CI stack completeness|TeamCity server & agent self‑contained, ready for pipline import|
|Build automation (Kotlin DSL)|Kotlin DSL which pushes to registry|

---

**Time boxing** – Aim for a maximum of 4 hours. Document any trade‑offs if you can't complete everything.

**Submission** – Share the Git repo URL plus any noteworthy caveats in the README.

---

# My own comments

## Part 1

- I've deleted the Person.Tests project and re-created a new one using the MSTest framework.
  Unfortunately, I wasn't able to have a showcase for the usage of Mock package.


## Part 2

- Docker Build failed due some strange characters in some of the existing files
	Error response from daemon: dockerfile parse error on line 1: unknown instruction:   #
  So, I had to recreate these files with Notepad (or Notepad++)

- To allow building, when running "docker compose up --build" or "docker compose -f compose.ci.yml up --build", I had to previously run this:
	$env:DOCKER_BUILDKIT=0
  Once it is built, then I was able to turn the variable back to "1". The files were "Dockerfile" and "Dockerfile.agent"

- Part 3's description starts up saying "Provide a separate compose.ci.yml file".
  So for this Part 2, I've renamed the file name to "compose.ci.agent.yml" instead

to launch the website, run the script:
    docker compose up

to launch the teamcity's agent, run the script:
    docker compose -f compose.ci.agent.yml up


## Part 3

- At Docker Desktop (Windows), I've changed: Preferences → Docker Engine
  and I have:
  <pre> ```json {
    "builder": {
        "gc": {
        "defaultKeepStorage": "20GB",
        "enabled": true
        }
    },
    "experimental": false,
    "insecure-registries": ["localhost:5000"]
  } ``` </pre>


to launch the teamcity's agent, run the script:
    docker compose -f compose.ci.yml up

to access team-city, use the URL:
    http://localhost:8111/login.html

at team city:
- I've manually added a project, giving my public github URL, no credentials were given
  - selected "Import settings from .teamcity/settings.kts and enable synchronization with the VCS repository"
  - left default values (indicating "main" branch) and clicked "Proceed"

At the "People API Build", I had to accept my VCS username


Unfortunately, I went round and around the TeamCity, Step 2 process. I couldn't figure out the permissions problem to access "/var/run/docker.sock/" directory, in order to create the docker image. I unsuccessfully tried many different approaches, as you can see under my commits at GitHub.
The current configurations uses the user "dockerbuilder", group id "docker" (at my local, the id is 999, but some other can be 998).
