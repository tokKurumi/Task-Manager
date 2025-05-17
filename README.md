## Migrations

### Prerequirements:
- Install the [dotnet EF CLI tool](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli#install-the-tools)

### Add a new migration:
To create a new EF Core migration, follow these steps:

1. Navigate to the API project directory so that the `.csproj` file is accessible.
2. Run the migration command, specifying the path to the infrastructure project:
   `dotnet ef migrations add <migration name> --project <relative path to infrastructure project>`

Example:
```bash
cd '.\Task Manager.Identity.IdentityAPI\'
dotnet ef migrations add 'Init Migration' --project '..\Task Manager.Identity.Infrastructure\Task Manager.Identity.Infrastructure.csproj'
```