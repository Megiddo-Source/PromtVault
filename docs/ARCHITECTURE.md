# Arquitectura base

## Capas actuales

### Models

Entidades y contratos de datos:

- `PromptItem`
- `Category`
- `Tag`
- `PromptTag`
- DTO de edición y exportación

### Data

- `AppDbContext`: configuración de SQLite y relaciones.
- `DbInitializer`: creación inicial y datos de ejemplo.

### Services

- `PromptService`: consultas y operaciones CRUD.
- `VariableResolver`: extracción y sustitución de `{{variables}}`.
- `PromptBackupService`: importación y exportación JSON.
- `DialogService` y `ClipboardService`: integración con Windows.

### ViewModels

- `MainViewModel`: biblioteca, filtros y acciones.
- `PromptEditorViewModel`: edición de prompts.
- `VariableDialogViewModel`: valores temporales de variables.

### Views

- `MainWindow`
- `PromptEditorWindow`
- `VariableDialogWindow`

## Reglas de evolución

1. La interfaz no debe acceder directamente a SQLite.
2. Toda lógica de prompts debe vivir en servicios o ViewModels.
3. Las integraciones externas deben entrar como servicios independientes.
4. No guardar claves de API sin cifrado.
5. Los formatos de exportación deben estar versionados antes de publicarse.
6. Sustituir `EnsureCreated` por migraciones antes de distribuir actualizaciones con cambios de esquema.

## Evolución futura sugerida

Cuando crezca el proyecto, separar en tres proyectos:

```text
PromptVault.Core
PromptVault.Infrastructure
PromptVault.App
```

La base actual permanece en un único proyecto para facilitar el primer arranque y evitar complejidad prematura.
