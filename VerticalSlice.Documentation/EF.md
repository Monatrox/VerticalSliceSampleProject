# Entity Framework

This document will include a bunch of scripts to automatically create migrations for various modules 

It's not entirely necessary, but convenient

### Create Initial Migration for Weather App
```bash
dotnet ef migrations add InitialCreate --project ../Weather.Infrastructure --startup-project ../Weather.Infrastructure --output-dir ../Weather.Infrastructure/Persistence/Migrations --context WeatherDbContext
dotnet ef migrations add InitialCreate --project ../Todo.Infrastructure --startup-project ../Todo.Infrastructure --output-dir ../Todo.Infrastructure/Persistence/Migrations --context TodoDbContext
```
### Create Initial Migration for Todo App
```bash
dotnet ef migrations add InitialCreate --project ../Order.Infrastructure --startup-project ../Order.Infrastructure --output-dir ../Order.Infrastructure/Persistence/Migrations --context OrderDbContext
```



### Clear Migrations for Weather App (Windows
```bash
Remove-Item -Recurse -Force ../Weather.Infrastructure/Persistence/Migrations/*
Remove-Item -Recurse -Force ../Todo.Infrastructure/Persistence/Migrations/*
```

