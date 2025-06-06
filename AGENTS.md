# Contribution Guidelines for ILDynamics

This repository contains multiple C# projects targeting .NET 8.0. When modifying any code under this repository, follow these rules:

1. **Testing and Formatting**
   - Run `dotnet format` at the repository root before committing changes.
   - Run `dotnet test` to ensure unit tests succeed.
   - If tests or format fail due to missing dependencies in the environment, note the failure in the PR description.
2. **Coding Style**
   - Use inline comments in English explaining complex logic.
   - End all files with a single newline character.
   - Prefer explicit access modifiers.
   - Begin each C# file with a short `summary` comment describing its purpose.
3. **Pull Requests**
   - Summarize changes briefly and mention if tests were executed.

These guidelines apply to the entire repository.
