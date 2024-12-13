# UITMANAGER - Setup Guide

## Prerequisites
Before you begin, ensure that the following are installed on your machine:
- [JetBrains Rider](https://www.jetbrains.com/Rider/) (latest stable version) or [Visual Studio](https://visualstudio.microsoft.com/fr/downloads/)
- .NET SDK version `net.08`
- Docker
    - On Windows, follow the installation guide [here](https://docs.docker.com/desktop/setup/install/windows-install/)
    - On Linux, follow the installation guide [here](https://docs.docker.com/desktop/setup/install/linux/)

## Download the Project
1. Open your terminal and run the following command to clone the GitLab project:
    ```bash
    $ git clone git@gitlab.com:infohers/2425/projet-grp1/projet.git ~/uitmanager
    ```

## Open in Rider/VS Studio
1. Launch JetBrains Rider or Visual Studio.
2. Click **Open** or navigate to **File > Open** and select the folder containing your `.sln` solution file. The path is: `~/uitmanager/Code/UITManagerAgent/UITManagerAgent.sln`.
3. Rider/VS Studio will load the solution, which includes all the associated projects (UITManagerAgent, UITManagerAgent.Tests, UITManagerWebServer, UITManagerApi).
4. Open your terminal and run the following commands to restore and build the projects:
    ```bash
    $ cd ~/uitmanager/Code/UITManagerAgent; dotnet restore; dotnet build
    $ cd ~/uitmanager/Code/UITManagerAgent.Tests; dotnet restore; dotnet build
    $ cd ~/uitmanager/Code/UITManagerWebServer; dotnet restore; dotnet build
    $ cd ~/uitmanager/Code/UITManagerApi; dotnet restore; dotnet build
    ```

## Database Setup
1. Create the `.env` file:
    ```bash
    $ touch ~/uitmanager/Database/.env
    ```
2. Copy the contents of the `.env` file from `credentials.md` and paste it into the `.env` file you just created.
3. Start the Docker container:
    ```bash
    $ cd ~/uitmanager/Database/
    $ docker-compose up -d
    ```
    If your have the new version of docker compose use:
    ```bash
    $ cd ~/uitmanager/Database/
    $ docker compose up -d
    ```
4. Your database will now be set up. To ensure everything is running smoothly without errors, execute the following checks:
    - To check the running container:
    ```bash
    $ docker container ls
    CONTAINER ID   IMAGE             COMMAND                  CREATED             STATUS             PORTS                                       NAMES
    d14fd5dc3274   postgres:latest   "docker-entrypoint.s…"   About an hour ago   Up About an hour   0.0.0.0:5432->5432/tcp, :::5432->5432/tcp   postgres_db
    ```
    Ensure the status is **Up**.
    
    - To check the Docker volume:
    ```bash
    $ docker volume ls
    DRIVER    VOLUME NAME
    local     database_postgres_data
    ```
    Ensure that the volume `database_postgres_data` exists.

## Running the Project
Once you have configured the projects to start, click the **Run** button in the top-right corner of Rider/VS Studio. This will launch all selected projects simultaneously, each in its own console window.

## Applying Migrations
To apply migrations to your PostgreSQL Docker container, run the following commands:
```bash
$ cd ~/uitmanager/Code/UITManagerWebServer;
$ dotnet ef database update
```
If you encounter any errors, try deleting the Migrations folder and re-initializing the migrations:
```bash
$ cd ~/uitmanager/Code/UITManagerWebServer;
$ rm -r Migrations
$ dotnet ef migrations add init
$ dotnet ef database update
```
## Stopping the Project
1. To stop all running projects in Rider/VS Studio, click the Stop button in the top-right corner. This will terminate all processes related to the running projects.
2. To stop the Docker container, run the following command in the terminal:
migrations:
    ```bash
    $ docker container stop postgres_db
    ```
3. If you wish to delete the database data associated with the project, run:
    ```bash
    $ docker container rm postgres_db
    $ docker volume rm database_postgres_data
    ```

