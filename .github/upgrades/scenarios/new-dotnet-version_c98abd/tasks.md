# SqlStudio .NET 10.0 Upgrade Tasks

## Overview

This document tracks the execution of the SqlStudio solution upgrade from .NET 8.0 to .NET 10.0 LTS. All projects will be upgraded simultaneously in a single atomic operation, followed by comprehensive testing and validation.

**Progress**: 2/4 tasks complete (50%) ![0%](https://progress-bar.xyz/50)

---

## Tasks

### [✓] TASK-001: Verify prerequisites *(Completed: 2026-02-17 10:48)*
**References**: Plan §Phase 0

- [✓] (1) Verify .NET 10 SDK installed per Plan §Prerequisites
- [✓] (2) SDK version meets minimum requirements (**Verify**)
- [✓] (3) Check global.json compatibility if present
- [✓] (4) Configuration file compatible with .NET 10 (**Verify**)

---

### [✓] TASK-002: Atomic framework and dependency upgrade *(Completed: 2026-02-17 10:54)*
**References**: Plan §Phase 1, Plan §Package Update Reference, Plan §Breaking Changes Catalog, Plan §Project-by-Project Migration Plans

- [✓] (1) Update TargetFramework to net10.0(-windows) in 10 projects per Plan §Migration Strategy (BinaryFiles and SqlParser remain on netstandard2.0 per Plan §BinaryFiles, §SqlParser)
- [✓] (2) All project files updated to target framework (**Verify**)
- [✓] (3) Update all package references to version 10.0.3 per Plan §Package Update Reference (12 packages including Microsoft.EntityFrameworkCore, Microsoft.Extensions.*, Microsoft.Windows.Compatibility, System.Text.Json, System.Configuration.ConfigurationManager)
- [✓] (4) All packages updated to specified versions (**Verify**)
- [✓] (5) Remove framework-included packages per Plan (System.ComponentModel.Annotations from SqlExecute/SqlStudio, System.Data.DataSetExtensions from SqlCommandCompleter/SqlParser, System.Memory from SqlExecute/SqlStudio)
- [✓] (6) Framework-included packages removed (**Verify**)
- [✓] (7) Address deprecated packages per Plan §Deprecated Package Risks (PoorMansTSqlFormatterRedux in SqlStudio, System.Data.SQLite in SqlStudio/SqlExecute)
- [✓] (8) Deprecated packages resolved (**Verify**)
- [✓] (9) Restore all dependencies
- [✓] (10) All dependencies restored successfully (**Verify**)
- [✓] (11) Build solution and fix all compilation errors per Plan §Breaking Changes Catalog (focus areas: Windows Forms API compatibility 10,283 issues in SqlStudio/CommandPrompt/FormatTextControl, GDI+ updates 278 issues, Windows Forms Legacy Controls 898 issues, Entity Framework Core 10.0 changes in CfgDataStore, package API changes)
- [✓] (12) Solution builds with 0 errors (**Verify**)

---

### [▶] TASK-003: Run full test suite and validate upgrade
**References**: Plan §Phase 2 Testing, Plan §Testing & Validation Strategy

- [✓] (1) Run tests in all test projects (FormatTextControlTests, SqlCommandCompleterTests, SqlStudioTests)
- [▶] (2) Fix any test failures (reference Plan §Breaking Changes Catalog for common issues: Windows Forms API changes, GDI+ compatibility, Entity Framework Core updates, package API changes)
- [ ] (3) Re-run all test projects after fixes
- [ ] (4) All tests pass with 0 failures (**Verify**)

---

### [ ] TASK-004: Final commit
**References**: Plan §Source Control Strategy

- [ ] (1) Commit all changes with message: "feat: Upgrade SqlStudio solution to .NET 10.0 LTS" (reference Plan §Source Control Strategy for comprehensive commit message template)

---





