# Database Setup Guide

Este documento explica como crear la base de datos local para el proyecto PokeGrading usando los scripts en la carpeta Scripts.

## Ubicacion de scripts

- Scripts/01_CreateDatabase.sql
- Scripts/02_CreateTables.sql
- Scripts/03_Constraints.sql
- Scripts/04_ForeignKeys.sql
- Scripts/05_Indexes.sql
- Scripts/06_SeedData.sql
- Scripts/07_TestData.sql

## Prerrequisitos

- SQL Server (Developer, Express, o instancia corporativa)
- SSMS o Azure Data Studio (recomendado), o sqlcmd
- Acceso para crear base de datos en la instancia local

## Flujo recomendado para cada contribuidor

1. Abrir una conexion a SQL Server.
2. Ejecutar los scripts en el orden 01 -> 07.
3. Verificar que no hubo errores en cada script antes de continuar.
4. Configurar la cadena de conexion en la API.
5. Levantar backend y frontend para validar flujo end-to-end.

## Orden de ejecucion (obligatorio)

1. 01_CreateDatabase.sql
2. 02_CreateTables.sql
3. 03_Constraints.sql
4. 04_ForeignKeys.sql
5. 05_Indexes.sql
6. 06_SeedData.sql
7. 07_TestData.sql

## Opcion A: Ejecutar con SSMS o Azure Data Studio

1. Abrir cada archivo en la carpeta Scripts.
2. Confirmar que el contexto de DB es correcto (los scripts usan USE PokeGrading).
3. Ejecutar uno por uno en el orden indicado.

## Opcion B: Ejecutar con sqlcmd

Ejemplo desde la raiz del repo:

```powershell
sqlcmd -S TU_SERVIDOR -E -i "PokeGrading/Database/Scripts/01_CreateDatabase.sql"
sqlcmd -S TU_SERVIDOR -E -i "PokeGrading/Database/Scripts/02_CreateTables.sql"
sqlcmd -S TU_SERVIDOR -E -i "PokeGrading/Database/Scripts/03_Constraints.sql"
sqlcmd -S TU_SERVIDOR -E -i "PokeGrading/Database/Scripts/04_ForeignKeys.sql"
sqlcmd -S TU_SERVIDOR -E -i "PokeGrading/Database/Scripts/05_Indexes.sql"
sqlcmd -S TU_SERVIDOR -E -i "PokeGrading/Database/Scripts/06_SeedData.sql"
sqlcmd -S TU_SERVIDOR -E -i "PokeGrading/Database/Scripts/07_TestData.sql"
```

Si usas usuario/password en lugar de autenticacion integrada:

```powershell
sqlcmd -S TU_SERVIDOR -U TU_USUARIO -P TU_PASSWORD -i "PokeGrading/Database/Scripts/01_CreateDatabase.sql"
```

## Configuracion de conexion en la API

Asegurate de definir ConnectionStrings:DefaultConnection en:

- PokeGrading/appsettings.Development.json
- PokeGrading/appsettings.json (solo si aplica para tu entorno)

Ejemplo:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SERVIDOR_SQL;Database=PokeGrading;User Id=TU_USUARIO;Password=TU_PASSWORD;TrustServerCertificate=True;"
  }
}
```

## Verificaciones minimas

- La base PokeGrading existe.
- Las tablas principales existen (USERS, ROLES, etc).
- El script 06 inserto roles.
- El script 07 inserto datos de prueba sin errores.
- El backend levanta y puede conectarse a SQL Server.

## Troubleshooting rapido

- Error de permisos: ejecutar con un usuario con permisos de CREATE DATABASE y DDL.
- Error por orden de scripts: volver a ejecutar en orden 01 -> 07.
- Error de conexion en API: revisar servidor, credenciales y firewall.
- Error SSL/TLS SQL: mantener TrustServerCertificate=True en local.

## Nota para el equipo

Cuando se agreguen cambios de esquema, crear un nuevo script numerado (por ejemplo 08_Algo.sql) en lugar de reescribir scripts historicos ya compartidos, salvo que el equipo acuerde reiniciar baseline.
