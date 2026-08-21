# PromptVault 0.2.0

Biblioteca local para organizar, buscar y reutilizar prompts sin depender de cuentas ni servicios externos.

## Funciones actuales

- Crear, editar y eliminar prompts.
- Categorías, etiquetas y modelo recomendado.
- Favoritos, más usados y recientes.
- Búsqueda por título, contenido, categoría, etiqueta o modelo.
- Variables reutilizables con formato `{{nombre_variable}}`.
- Contador de usos y fecha del último uso.
- Importación y exportación de copias JSON.
- Base SQLite local.
- Interfaz oscura moderna con biblioteca en tarjetas.

## Requisitos

- Windows 10 u 11.
- .NET 8 SDK.
- Visual Studio 2022 con **Desarrollo de escritorio con .NET**, o PowerShell.

## Ejecutar

Desde la raíz del proyecto:

```powershell
dotnet restore PromptVault.sln
dotnet run --project src\PromptVault.App\PromptVault.App.csproj
```

También puedes usar:

```powershell
.\scripts\run.ps1
```

## Base de datos

Se crea automáticamente en:

```text
%LOCALAPPDATA%\PromptVault\promptvault.db
```

La actualización visual no modifica el formato de la base de datos. Puedes sustituir la versión anterior conservando tus prompts.

## Estructura

```text
src/PromptVault.App/
├─ Data/
├─ Models/
├─ Services/
├─ Themes/
├─ ViewModels/
├─ Views/
├─ MainWindow.xaml
└─ App.xaml
```

## Estado de la versión

La versión 0.2.0 se centra en el rediseño visual. No añade dependencias externas ni cambia el modelo de datos de la versión 0.1.0.
