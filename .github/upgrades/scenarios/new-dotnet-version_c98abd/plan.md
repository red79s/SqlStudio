# .NET 10 Upgrade Plan

## Table of Contents
- [Executive Summary](#executive-summary)
- [Migration Strategy](#migration-strategy)
- [Detailed Dependency Analysis](#detailed-dependency-analysis)
- [Project-by-Project Migration Plans](#project-by-project-migration-plans)
- [Risk Management](#risk-management)
- [Testing & Validation Strategy](#testing--validation-strategy)
- [Complexity & Effort Assessment](#complexity--effort-assessment)
- [Source Control Strategy](#source-control-strategy)
- [Success Criteria](#success-criteria)

---

## Executive Summary

### Scenario Overview
Upgrade SqlStudio solution from .NET 8.0 to **.NET 10.0 (Long Term Support)** to benefit from the latest performance improvements, security enhancements, and long-term support lifecycle.

### Scope
- **12 projects** across the solution
- **Current Frameworks**: .NET 8.0 (10 projects), .NET Standard 2.0 (2 projects)
- **Target Framework**: .NET 10.0
- **Total Issues**: 10,624 (10,233 mandatory, 388 potential, 3 optional)
- **Affected Files**: 91 files

### Current State Analysis
**Projects by Dependency Level**:
- **Level 0 (Foundation)**: BinaryFiles, Common, FormatTextControl, SqlParser
- **Level 1**: FormatTextControlTests, SqlCommandCompleter, SqlExecute
- **Level 2**: CfgDataStore, SqlCommandCompleterTests
- **Level 3**: CommandPrompt
- **Level 4**: SqlStudio (main application)
- **Level 5**: SqlStudioTests

**Key Technology Areas**:
- **Windows Forms**: 10,283 issues (primary complexity driver)
- **GDI+ / System.Drawing**: 278 issues
- **Windows Forms Legacy Controls**: 898 issues
- **Legacy Configuration System**: 2 issues

### Target State
- All projects targeting **net10.0** (except BinaryFiles and SqlParser which may remain on netstandard2.0 for compatibility)
- Updated Microsoft.* packages to version 10.0.3
- Deprecated packages replaced or removed
- All API compatibility issues resolved

### Selected Strategy
**All-At-Once Strategy** - Upgrade all projects simultaneously in a single coordinated operation.

**Rationale**:
- All projects already on modern .NET (.NET 8.0 or netstandard2.0)
- Homogeneous framework upgrade path (8.0 → 10.0)
- Most issues (10K+) are Windows Forms compatibility warnings, not breaking changes
- Clear dependency structure enables atomic update
- Assessment shows all packages have net10.0 compatible versions

### Complexity Assessment
**Overall Complexity**: **High**

**Metrics**:
- Solution Size: 12 projects, 6 dependency levels
- Issue Count: 10,624 (extremely high, but mostly compatibility warnings)
- Risk Indicators: 2 deprecated packages, Windows Forms compatibility concerns
- Dependency Depth: 6 levels (deep but linear)

**Key Complexity Drivers**:
1. Windows Forms migration (10,283 issues) - primarily compatibility diagnostics
2. Deep dependency chain requiring coordinated updates
3. Deprecated packages: `PoorMansTSqlFormatterRedux`, `System.Data.SQLite`
4. GDI+ usage (278 issues)

### Critical Issues
1. **Deprecated NuGet Packages** (requires replacement):
   - `PoorMansTSqlFormatterRedux` → Needs alternative or removal
   - `System.Data.SQLite` → Replace with `Microsoft.Data.Sqlite` or update approach

2. **Package Upgrades Required** (12 packages):
   - Microsoft.EntityFrameworkCore: 9.0.1 → 10.0.3
   - Microsoft.Windows.Compatibility: 9.0.1 → 10.0.3
   - All Microsoft.Extensions.* packages: 9.0.1 → 10.0.3
   - System.Configuration.ConfigurationManager: 9.0.1 → 10.0.3
   - System.Text.Json: 9.0.1 → 10.0.3
   - System.Runtime.CompilerServices.Unsafe: 6.1.0 → 6.1.2

3. **Framework Packages to Remove** (functionality now in framework):
   - System.ComponentModel.Annotations
   - System.Data.DataSetExtensions
   - System.Memory

### Recommended Approach
Execute atomic upgrade across all projects with the following phases:

1. **Prerequisites**: Verify .NET 10 SDK installation, update global.json if present
2. **Atomic Upgrade**: Update all project files, package references, restore, build, fix compilation errors in single operation
3. **Testing**: Execute all test projects to validate functionality
4. **Validation**: Comprehensive smoke testing

### Iteration Strategy
Given high complexity but homogeneous upgrade path:
- **Phase 1**: Discovery & Classification (complete)
- **Phase 2**: Foundation sections (3 iterations)
- **Phase 3**: Detailed project specifications (3-4 iterations):
  - Iteration 1: High-risk projects (SqlStudio, CommandPrompt, FormatTextControl)
  - Iteration 2: Medium complexity batch (SqlExecute, CfgDataStore, SqlCommandCompleter)
  - Iteration 3: Low complexity batch (remaining libraries and test projects)
  - Iteration 4: Final sections (Success Criteria, Source Control)

**Estimated Total**: 8 iterations

---

## Migration Strategy

### Approach Selection: All-At-Once Strategy

**Decision**: Upgrade all projects simultaneously in a single coordinated operation.

**Justification**:

1. **Homogeneous Starting Point**
   - 10 projects on .NET 8.0 (modern framework)
   - 2 projects on .NET Standard 2.0 (already compatible)
   - No legacy .NET Framework projects requiring multi-targeting

2. **Clear Upgrade Path**
   - Direct .NET 8.0 → .NET 10.0 migration
   - All packages have net10.0 compatible versions
   - No intermediate framework versions needed

3. **Solution Characteristics**
   - Medium-sized solution (12 projects)
   - Clean dependency hierarchy (no cycles)
   - Homogeneous technology stack (Windows Forms + libraries)

4. **Issue Nature**
   - 99.9% of issues concentrated in 3 Windows Forms projects
   - Most issues are compatibility diagnostics, not breaking changes
   - Can be addressed systematically in single pass

5. **Risk Profile**
   - Known deprecated packages with clear replacements
   - Assessment confirms package compatibility
   - All projects can be built and tested together

### All-At-Once Strategy Implementation

**Core Principle**: All project files and package references updated simultaneously, then all compilation errors addressed in coordinated fashion.

**Atomic Operation Scope**:
- Update TargetFramework in all 12 projects
- Update all package references across all projects
- Restore dependencies
- Build entire solution
- Fix all compilation errors
- Rebuild and verify

**Why All-At-Once Works Here**:
- Projects already on compatible frameworks
- Package ecosystem fully supports net10.0
- Windows Forms issues are well-understood compatibility warnings
- Team can test entire solution together
- Single coherent set of changes for review

### Dependency-Based Ordering Considerations

While All-At-Once updates all projects simultaneously, understanding dependency order helps predict compilation error propagation:

**Error Propagation Pattern**:
1. **Foundation errors appear first**: Common, FormatTextControl issues will manifest immediately
2. **Cascading errors**: Fixes in foundation libraries may resolve downstream errors
3. **Windows Forms concentration**: Most compilation work will be in SqlStudio, CommandPrompt, FormatTextControl

**Practical Impact**: When fixing compilation errors during atomic upgrade, prioritize foundation libraries (Level 0) first as their fixes may resolve downstream issues automatically.

### Parallel vs Sequential Execution

**File Updates**: Can be parallelized (independent file modifications)

**Compilation**: Sequential by necessity
1. Build solution after all project file updates
2. Address compilation errors systematically
3. Prioritize foundation libraries when fixing
4. Rebuild incrementally to verify fixes

**Testing**: Can be parallelized after successful compilation
- Test projects are independent
- Can run unit tests concurrently

### Phase Definitions

**Phase 0: Prerequisites** (if needed)
- Verify .NET 10 SDK installed
- Update global.json if present
- Validate development environment

**Phase 1: Atomic Upgrade** (single coordinated operation)
- Update all project files to net10.0
- Update all package references
- Remove deprecated packages
- Remove framework-included packages
- Restore dependencies
- Build solution
- Fix all compilation errors
- Rebuild and verify 0 errors

**Phase 2: Test Validation**
- Execute all test projects
- Address test failures
- Verify all tests pass

**Phase 3: Final Validation**
- Manual smoke testing
- Performance validation
- Documentation updates

### Risk Mitigation During Atomic Upgrade

**High-Risk Projects Handling**:
1. **SqlStudio** (8,976 issues): Windows Forms compatibility warnings - address after build
2. **CommandPrompt** (844 issues): UI component compatibility - fix after SqlStudio patterns established
3. **FormatTextControl** (402 issues): Foundation UI library - fix early as affects downstream

**Deprecated Package Strategy**:
- `PoorMansTSqlFormatterRedux`: Evaluate if still needed, remove or find alternative
- `System.Data.SQLite`: Replace with Microsoft.Data.Sqlite or compatible alternative

**Rollback Strategy**:
- All changes in dedicated branch `upgrade-to-NET10`
- Single atomic commit enables clean revert if needed
- Can return to .NET 8 state with `git reset --hard`

### Success Criteria for Each Phase

**Phase 0 Complete**: .NET 10 SDK available, environment ready

**Phase 1 Complete**: 
- All projects target net10.0
- All packages updated
- Solution builds with 0 errors
- No deprecated packages remain

**Phase 2 Complete**:
- All unit tests pass
- No test project build errors

**Phase 3 Complete**:
- Application launches successfully
- Core functionality verified
- No runtime exceptions

---

## Detailed Dependency Analysis

### Dependency Graph Structure

The SqlStudio solution has a well-defined 6-level dependency hierarchy:

```
Level 0 (Foundation - No Dependencies):
├─ BinaryFiles.csproj          (0 issues, netstandard2.0)
├─ Common.csproj               (1 mandatory issue, net8.0)
├─ FormatTextControl.csproj    (402 mandatory issues, net8.0-windows7.0)
└─ SqlParser.csproj            (1 mandatory issue, netstandard2.0)

Level 1 (Depends on Level 0):
├─ FormatTextControlTests      → Common, FormatTextControl
├─ SqlCommandCompleter         → Common
└─ SqlExecute                  → Common

Level 2 (Depends on Level 0-1):
├─ CfgDataStore               → Common, SqlExecute
└─ SqlCommandCompleterTests   → Common, SqlCommandCompleter

Level 3 (Depends on Level 0-2):
└─ CommandPrompt              → Common, FormatTextControl, CfgDataStore

Level 4 (Depends on Level 0-3):
└─ SqlStudio (Main App)       → CommandPrompt, BinaryFiles, Common, 
                                 FormatTextControl, SqlCommandCompleter,
                                 SqlExecute, CfgDataStore

Level 5 (Depends on Level 0-4):
└─ SqlStudioTests             → SqlParser, CommandPrompt, SqlStudio
```

### Project Groupings for All-At-Once Migration

**All-At-Once Strategy**: All projects will be updated simultaneously in a single atomic operation. The dependency analysis informs understanding of project relationships and potential compilation error propagation, but does not dictate sequential migration.

**Grouping for Understanding** (not execution sequence):

**Group 1: Foundation Libraries** (Level 0)
- BinaryFiles (netstandard2.0 - may remain)
- Common (net8.0)
- FormatTextControl (net8.0-windows7.0) - HIGH RISK: 402 mandatory issues
- SqlParser (netstandard2.0 - may remain)

**Group 2: Core Services** (Level 1-2)
- SqlCommandCompleter
- SqlExecute
- CfgDataStore

**Group 3: UI Layer** (Level 3)
- CommandPrompt - HIGH RISK: 844 mandatory issues

**Group 4: Application & Tests** (Level 4-5)
- SqlStudio - HIGHEST RISK: 8,976 mandatory issues
- All test projects (FormatTextControlTests, SqlCommandCompleterTests, SqlStudioTests)

### Critical Path Identification

**Highest Risk Projects** (by mandatory issue count):
1. **SqlStudio** (8,976 mandatory issues) - Main WinForms application
2. **CommandPrompt** (844 mandatory issues) - UI component library
3. **FormatTextControl** (402 mandatory issues) - UI component library

These three projects account for **10,222 of 10,233** mandatory issues (99.9%). They are Windows Forms-heavy and will require the most attention during compilation error resolution.

**Foundation Projects** (minimal issues but critical):
- Common (1 issue) - Used by 7 projects
- SqlParser (1 issue) - Used by tests

### Circular Dependencies

**Analysis**: No circular dependencies detected. Clean hierarchical structure enables straightforward dependency resolution.

### Migration Considerations

1. **Atomic Update Scope**: All 12 projects updated simultaneously
2. **Windows Forms Concentration**: 99.9% of issues in 3 WinForms projects
3. **Test Project Strategy**: Test projects updated with their tested projects
4. **netstandard2.0 Projects**: BinaryFiles and SqlParser may remain on netstandard2.0 for broader compatibility, or upgrade to net10.0 if no cross-platform requirements exist

### Dependency Impact Matrix

| Project | Direct Dependencies | Downstream Consumers | Risk Level |
|---------|---------------------|----------------------|------------|
| BinaryFiles | 0 | 1 (SqlStudio) | Low |
| Common | 0 | 7 | Medium |
| FormatTextControl | 0 | 3 | High |
| SqlParser | 0 | 1 (tests) | Low |
| SqlCommandCompleter | 1 | 2 | Low |
| SqlExecute | 1 | 2 | Low |
| CfgDataStore | 2 | 2 | Medium |
| CommandPrompt | 3 | 2 | High |
| SqlStudio | 7 | 1 (tests) | Critical |
| Test Projects | varies | 0 | Low |

---

## Project-by-Project Migration Plans

### Overview
All projects will be migrated simultaneously using the All-At-Once strategy. The specifications below provide detailed information for each project to guide the atomic upgrade operation.

---

### Project: BinaryFiles

**Current State**: 
- TargetFramework: netstandard2.0
- Project Type: ClassLibrary
- Files: Unknown
- Issues: 0 (no issues detected)
- Dependencies: None (Level 0)

**Target State**: 
- TargetFramework: netstandard2.0 **or** net10.0
- Decision: Keep netstandard2.0 for maximum compatibility (used by single project)

**Migration Steps**:

1. **Prerequisites**
   - No dependencies
   - Used only by SqlStudio

2. **Target Framework Decision**
   - **Recommendation**: Keep as netstandard2.0
   - **Rationale**: 
     - No issues detected
     - netstandard2.0 is compatible with net10.0
     - Maintains broader compatibility
     - Only one consumer (SqlStudio)
   - **Alternative**: Upgrade to net10.0 if no cross-platform concerns

3. **Package Updates**
   - None

4. **Expected Breaking Changes**
   - None (if kept as netstandard2.0)
   - Minimal (if upgraded to net10.0)

5. **Code Modifications**
   - None required

6. **Testing Strategy**
   - Validation via SqlStudio

7. **Validation Checklist**
- [ ] Project builds without errors
- [ ] Project builds with no warnings
- [ ] SqlStudio consumes successfully
- [ ] Binary file handling works in SqlStudio

---

### Project: Common

**Current State**: 
- TargetFramework: net8.0
- Project Type: ClassLibrary (Foundation - Shared Utilities)
- Files: 20 total
- Issues: 1 total (1 mandatory - framework update only)
- Dependencies: None (Level 0)

**Target State**: 
- TargetFramework: net10.0
- No package changes

**Migration Steps**:

1. **Prerequisites**
   - No project dependencies (Level 0 - Foundation)
   - **Critical**: Used by 7 downstream projects
   - Must be fixed early as affects entire solution

2. **Target Framework Update**
   - Change `<TargetFramework>net8.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`

3. **Package Updates**
   - No packages in this project

4. **Expected Breaking Changes**
   - Framework update only, no other issues detected
   - Clean upgrade expected

5. **Code Modifications**
   - Minimal to none expected
   - Shared utility code should be compatible

6. **Testing Strategy**
   - Validation via consuming projects
   - All 7 downstream projects serve as integration tests

7. **Validation Checklist**
- [ ] Project builds without errors
- [ ] Project builds with no warnings
- [ ] All downstream projects build successfully
- [ ] No breaking changes introduced

---

### Project: FormatTextControl

**Current State**: 
- TargetFramework: net8.0-windows7.0
- Project Type: ClassLibrary (UI Component - Foundation)
- Files: 12 total
- Issues: 541 total (402 mandatory, 139 potential)
- Dependencies: None (Level 0 - Foundation)

**Target State**: 
- TargetFramework: net10.0-windows
- Package remains compatible

**Migration Steps**:

1. **Prerequisites**
   - No project dependencies (Level 0)
   - Used by SqlStudio, CommandPrompt, FormatTextControlTests
   - Critical foundation library - errors here cascade downstream

2. **Target Framework Update**
   - Change `<TargetFramework>net8.0-windows7.0</TargetFramework>` to `<TargetFramework>net10.0-windows</TargetFramework>`

3. **Package Updates**

**Packages to Keep (Compatible)**:
- Microsoft.DotNet.UpgradeAssistant.Extensions.Default.Analyzers: 0.4.421302
- Microsoft.Windows.Compatibility: 9.0.1 (already compatible)

4. **Expected Breaking Changes**

**Windows Forms (401 binary incompatibilities)**:
- Custom text formatting control implementation
- Control property changes
- Event handler updates
- Custom rendering

**GDI+ / System.Drawing (137 source incompatibilities)**:
- Text rendering operations
- Font handling
- Graphics object usage
- Drawing primitives
- Color and brush operations

**Windows Forms Legacy Controls (14 issues)**:
- Legacy control patterns

**Behavioral Changes (1 issue)**:
- Runtime behavior difference requiring validation

5. **Code Modifications**

**Expected Areas Requiring Changes**:
- Custom drawing code (GDI+ heavy usage)
- Text formatting and rendering
- Font management
- Control lifecycle and event handlers
- UserControl implementation patterns

**Approach**:
- **Priority: Fix this project early** - foundation library affects 3 downstream projects
- Focus on GDI+ compatibility (137 issues)
- Update Windows Forms control patterns
- Test rendering extensively
- Validate behavioral change

6. **Testing Strategy**

**Unit Tests**: Via FormatTextControlTests project (upgraded simultaneously)

**Integration Tests**: 
- Rendering validation
- Text formatting operations
- Font rendering
- Performance testing

**Manual Tests**:
- Control rendering in different scenarios
- Text formatting with various inputs
- Visual appearance validation
- Integration with SqlStudio and CommandPrompt

7. **Validation Checklist**
- [ ] Project builds without errors
- [ ] Project builds with minimal warnings
- [ ] FormatTextControlTests pass
- [ ] Text rendering works correctly
- [ ] Fonts render as expected
- [ ] Custom drawing operations functional
- [ ] No visual regressions
- [ ] Performance acceptable
- [ ] Downstream projects (SqlStudio, CommandPrompt) unaffected

---

### Project: SqlParser

**Current State**: 
- TargetFramework: netstandard2.0
- Project Type: ClassLibrary (SQL Parsing)
- Files: Unknown
- Issues: 1 total (1 mandatory - package removal)
- Dependencies: None (Level 0)

**Target State**: 
- TargetFramework: netstandard2.0 **or** net10.0
- Framework-included package removed

**Migration Steps**:

1. **Prerequisites**
   - No dependencies
   - Used by SqlStudioTests

2. **Target Framework Decision**
   - **Recommendation**: Keep as netstandard2.0
   - **Rationale**: 
     - Minimal issues (1 package removal)
     - netstandard2.0 compatible with net10.0
     - Parser library benefits from broad compatibility
   - **Alternative**: Upgrade to net10.0 for consistency

3. **Package Updates**

**Packages to Remove** (functionality in framework):
- System.Data.DataSetExtensions (if present)

4. **Expected Breaking Changes**
   - Package removal only
   - Namespace may need adjustment

5. **Code Modifications**
   - Remove package reference
   - Verify namespace imports
   - SQL parsing logic should be unaffected

6. **Testing Strategy**
   - Via SqlStudioTests
   - SQL parsing functionality validation

7. **Validation Checklist**
- [ ] Project builds without errors
- [ ] Project builds with no warnings
- [ ] SqlStudioTests pass
- [ ] SQL parsing works correctly

---

### Project: SqlCommandCompleter

**Current State**: 
- TargetFramework: net8.0
- Project Type: ClassLibrary (SQL Intellisense/Completion)
- Files: 3 total
- Issues: 2 total (2 mandatory)
- Dependencies: Common

**Target State**: 
- TargetFramework: net10.0
- Framework-included package removed

**Migration Steps**:

1. **Prerequisites**
   - Dependency (Common) upgraded in same atomic operation
   - Used by SqlStudio and SqlCommandCompleterTests

2. **Target Framework Update**
   - Change `<TargetFramework>net8.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`

3. **Package Updates**

**Packages to Remove** (functionality in framework):
- System.Data.DataSetExtensions: 4.5.0

**Packages to Keep (Compatible)**:
- Microsoft.CSharp: 4.7.0
- Microsoft.DotNet.UpgradeAssistant.Extensions.Default.Analyzers: 0.4.421302

4. **Expected Breaking Changes**

**Framework Package Removal**:
- System.Data.DataSetExtensions functionality now in framework
- May need namespace updates if directly referenced

5. **Code Modifications**

**Expected Areas Requiring Changes**:
- Namespace references (if System.Data.DataSetExtensions explicitly used)
- SQL command completion logic
- Intellisense provider implementation

**Approach**:
- Remove package reference
- Verify namespace imports
- Test SQL completion functionality

6. **Testing Strategy**

**Unit Tests**: Via SqlCommandCompleterTests project (upgraded simultaneously)

**Integration Tests**: 
- SQL command completion
- Keyword suggestions
- Table/column name completion
- Performance of completion engine

7. **Validation Checklist**
- [ ] Project builds without errors
- [ ] Project builds with no warnings
- [ ] SqlCommandCompleterTests pass
- [ ] SQL completion works correctly
- [ ] Keyword suggestions functional
- [ ] Table/column completion works
- [ ] No performance regressions

---

### Project: SqlExecute

**Current State**: 
- TargetFramework: net8.0-windows7.0
- Project Type: ClassLibrary (Database Execution Layer)
- Files: 15 total
- Issues: 22 total (3 mandatory, 18 potential, 1 optional)
- Dependencies: Common

**Target State**: 
- TargetFramework: net10.0-windows
- Package updated to 10.0.3
- Framework-included packages removed

**Migration Steps**:

1. **Prerequisites**
   - Dependency (Common) upgraded in same atomic operation
   - Used by SqlStudio and CfgDataStore

2. **Target Framework Update**
   - Change `<TargetFramework>net8.0-windows7.0</TargetFramework>` to `<TargetFramework>net10.0-windows</TargetFramework>`

3. **Package Updates**

| Package | Current Version | Target Version | Reason |
|---------|----------------|----------------|--------|
| System.Text.Json | 9.0.1 | 10.0.3 | Framework compatibility |

**Packages to Remove** (functionality in framework):
- System.ComponentModel.Annotations: 5.0.0
- System.Memory: 4.6.0

**Packages to Keep (Compatible)**:
- K4os.Compression.LZ4.Streams: 1.3.8
- K4os.Hash.xxHash: 1.0.8
- Microsoft.CSharp: 4.7.0
- Microsoft.Windows.Compatibility: 9.0.1
- MySql.Data: 9.2.0
- Npgsql: 9.0.2
- Oracle.ManagedDataAccess.Core: 23.7.0
- SQLitePCLRaw.bundle_e_sqlite3: 2.1.10
- Microsoft.DotNet.UpgradeAssistant.Extensions.Default.Analyzers: 0.4.421302

**Deprecated Package**:
- **System.Data.SQLite 1.0.119**: ⚠️ Deprecated
  - Action: Evaluate migration to Microsoft.Data.Sqlite or keep with monitoring
  - Impact: SQLite database access code

4. **Expected Breaking Changes**

**Source Incompatibilities (16 potential issues)**:
- API signature changes
- Database access pattern updates
- JSON serialization behavior (System.Text.Json update)

**Framework Package Removal**:
- Code using System.ComponentModel.Annotations may need namespace updates
- System.Memory usage should work seamlessly (in framework)

5. **Code Modifications**

**Expected Areas Requiring Changes**:
- Database connection handling (multiple providers: MySQL, PostgreSQL, Oracle, SQLite)
- SQLite access if deprecated package replaced
- JSON serialization/deserialization
- Data annotation attributes (namespace may change)

**Approach**:
- Verify database provider compatibility
- Test JSON serialization patterns
- Update namespace references if needed
- Validate multi-database support

6. **Testing Strategy**

**Unit Tests**: Test database execution logic

**Integration Tests**: 
- SQL Server connectivity
- MySQL connectivity
- PostgreSQL connectivity
- Oracle connectivity
- SQLite connectivity
- Query execution
- Result set handling
- Transaction management

7. **Validation Checklist**
- [ ] Project builds without errors
- [ ] Project builds with no warnings
- [ ] All database providers connect successfully
- [ ] Query execution works across all providers
- [ ] Result sets retrieved correctly
- [ ] Transactions work properly
- [ ] JSON serialization functional
- [ ] No performance regressions

---

### Project: CfgDataStore

**Current State**: 
- TargetFramework: net8.0-windows7.0
- Project Type: ClassLibrary (Configuration/Data Storage Layer)
- Files: 11 total
- Issues: 3 total (1 mandatory, 2 potential)
- Dependencies: Common, SqlExecute

**Target State**: 
- TargetFramework: net10.0-windows
- Packages updated to 10.0.3

**Migration Steps**:

1. **Prerequisites**
   - Dependencies (Common, SqlExecute) upgraded in same atomic operation
   - Used by SqlStudio and CommandPrompt

2. **Target Framework Update**
   - Change `<TargetFramework>net8.0-windows7.0</TargetFramework>` to `<TargetFramework>net10.0-windows</TargetFramework>`

3. **Package Updates**

| Package | Current Version | Target Version | Reason |
|---------|----------------|----------------|--------|
| Microsoft.EntityFrameworkCore | 9.0.1 | 10.0.3 | Framework compatibility |
| Microsoft.EntityFrameworkCore.Sqlite.Core | 9.0.1 | 10.0.3 | Framework compatibility |

4. **Expected Breaking Changes**

**Entity Framework Core 9.0 → 10.0**:
- Minor API changes in EF Core
- Query generation differences
- Migration compatibility
- SQLite provider updates

5. **Code Modifications**

**Expected Areas Requiring Changes**:
- DbContext configuration
- Entity configurations
- LINQ queries (potential behavior changes)
- Migration scripts (may need regeneration)
- Database initialization

**Approach**:
- Update EF Core usage patterns
- Test database operations thoroughly
- Validate migrations
- Check for query behavior changes

6. **Testing Strategy**

**Unit Tests**: Configuration persistence, entity operations

**Integration Tests**: 
- DbContext creation
- Entity CRUD operations
- Query execution
- Migration application
- Configuration loading/saving
- Data integrity

7. **Validation Checklist**
- [ ] Project builds without errors
- [ ] Project builds with no warnings
- [ ] DbContext initializes correctly
- [ ] Entities can be queried
- [ ] CRUD operations work
- [ ] Migrations apply successfully
- [ ] Configuration persists correctly
- [ ] Data integrity maintained
- [ ] No EF Core performance regressions

---

### Project: CommandPrompt

**Current State**: 
- TargetFramework: net8.0-windows7.0
- Project Type: ClassLibrary (UI Component)
- Files: 17 total
- Issues: 854 total (844 mandatory, 10 potential)
- Dependencies: Common, FormatTextControl, CfgDataStore

**Target State**: 
- TargetFramework: net10.0-windows
- Package updated to 10.0.3

**Migration Steps**:

1. **Prerequisites**
   - Dependencies (Common, FormatTextControl, CfgDataStore) upgraded in same atomic operation
   - Used by SqlStudio and SqlStudioTests

2. **Target Framework Update**
   - Change `<TargetFramework>net8.0-windows7.0</TargetFramework>` to `<TargetFramework>net10.0-windows</TargetFramework>`

3. **Package Updates**

| Package | Current Version | Target Version | Reason |
|---------|----------------|----------------|--------|
| Microsoft.Windows.Compatibility | 9.0.1 | 10.0.3 | Framework compatibility |

**Packages to Keep (Compatible)**:
- Microsoft.DotNet.UpgradeAssistant.Extensions.Default.Analyzers: 0.4.421302

4. **Expected Breaking Changes**

**Windows Forms (843 binary incompatibilities)**:
- Control property changes in command prompt UI components
- Event handler updates
- Custom control rendering

**GDI+ / System.Drawing (9 issues)**:
- Custom drawing for command prompt display
- Text rendering and syntax highlighting

**Windows Forms Legacy Controls (4 issues)**:
- Legacy control compatibility

**Source Incompatibilities (9 issues)**:
- API signature changes

5. **Code Modifications**

**Expected Areas Requiring Changes**:
- CmdLineControl.cs (Component): Command line control implementation
- SQLScript.cs (UserControl): SQL script display control
- Custom drawing operations
- Event handlers
- Control initialization

**Approach**:
- Follow patterns established in FormatTextControl (dependency)
- Fix compilation errors systematically
- Test control rendering and behavior
- Validate integration with SqlStudio

6. **Testing Strategy**

**Unit Tests**: Component-level testing

**Integration Tests**: 
- Used by SqlStudio and SqlStudioTests
- Validation happens through consumer testing

**Manual Tests**:
- Command prompt control rendering
- Text input and display
- SQL script formatting
- Syntax highlighting (if applicable)
- Control interaction

7. **Validation Checklist**
- [ ] Project builds without errors
- [ ] Project builds with minimal warnings
- [ ] CmdLineControl renders correctly
- [ ] SQLScript control displays properly
- [ ] Custom drawing operations work
- [ ] Integration with SqlStudio validated
- [ ] No visual regressions

---

### Project: SqlStudio

**Current State**: 
- TargetFramework: net8.0-windows7.0
- Project Type: WinForms (Main Application)
- Files: 127 total
- Issues: 9,191 total (8,976 mandatory, 213 potential, 2 optional)
- Dependencies: CommandPrompt, BinaryFiles, Common, FormatTextControl, SqlCommandCompleter, SqlExecute, CfgDataStore

**Target State**: 
- TargetFramework: net10.0-windows
- All packages updated to 10.0.3
- Deprecated packages resolved

**Migration Steps**:

1. **Prerequisites**
   - Verify project builds on .NET 8.0 baseline
   - Document current application behavior
   - Ensure all dependencies upgraded first (part of atomic operation)

2. **Target Framework Update**
   - Change `<TargetFramework>net8.0-windows7.0</TargetFramework>` to `<TargetFramework>net10.0-windows</TargetFramework>`
   - Windows 7.0 target removed in .NET 10, use net10.0-windows

3. **Package Updates**

| Package | Current Version | Target Version | Reason |
|---------|----------------|----------------|--------|
| Microsoft.EntityFrameworkCore.Sqlite | 9.0.1 | 10.0.3 | Framework compatibility |
| Microsoft.Extensions.Caching.Memory | 9.0.1 | 10.0.3 | Framework compatibility |
| Microsoft.Extensions.Configuration.Binder | 9.0.1 | 10.0.3 | Framework compatibility |
| Microsoft.Extensions.Hosting | 9.0.1 | 10.0.3 | Framework compatibility |
| Microsoft.Extensions.Identity.Core | 9.0.1 | 10.0.3 | Framework compatibility |
| Microsoft.Extensions.Logging | 9.0.1 | 10.0.3 | Framework compatibility |

**Packages to Keep (Compatible)**:
- Microsoft.DotNet.UpgradeAssistant.Extensions.Default.Analyzers: 0.4.421302
- SQLitePCLRaw.bundle_e_sqlite3: 2.1.10
- System.ComponentModel.Annotations: 5.0.0
- System.Linq.Dynamic.Core: 1.6.0
- System.Memory: 4.6.0
- System.Windows.Forms.DataVisualization: 1.0.0-prerelease.20110.1

**Deprecated Packages (Requires Action)**:
- **PoorMansTSqlFormatterRedux 1.0.3**: ⚠️ Deprecated
  - Action: Investigate usage, consider removal or replacement
  - Alternative: Find maintained T-SQL formatter or implement custom
- **System.Data.SQLite 1.0.119**: ⚠️ Deprecated
  - Action: Evaluate migration to Microsoft.Data.Sqlite
  - Impact: May require SQLite access code changes

4. **Expected Breaking Changes**

**Windows Forms (8,973 binary incompatibilities)**:
- Control property changes
- Event handler updates
- Layout engine differences
- Visual appearance validation required

**GDI+ / System.Drawing (132 issues)**:
- Graphics object disposal patterns
- Bitmap handling
- Font rendering

**Windows Forms Legacy Controls (880 issues)**:
- Legacy control compatibility warnings
- May require control replacement or compatibility shims

**Legacy Configuration System (2 issues)**:
- ConfigurationManager API changes
- Validate configuration loading

**Source Incompatibilities (201 issues)**:
- API signature changes
- Method parameter updates
- Property accessor changes

**Behavioral Changes (6 issues)**:
- Runtime behavior differences
- Requires functional testing

5. **Code Modifications**

**Expected Areas Requiring Changes**:
- Windows Forms control initialization
- Event handler signatures
- Configuration loading code
- SQLite database access (if deprecated package replaced)
- T-SQL formatting (if deprecated package replaced)
- GDI+ drawing operations
- Legacy control usage

**Approach**:
- Fix compilation errors by pattern, not individually
- Use IDE quick fixes where applicable
- Consult .NET 10 migration documentation for Windows Forms
- Test visual appearance after each major fix category

6. **Testing Strategy**

**Unit Tests**: Via SqlStudioTests project (upgraded simultaneously)

**Integration Tests**: 
- Database connectivity (SQLite, SQL Server, PostgreSQL, MySQL)
- Configuration loading
- Entity Framework operations
- T-SQL formatting

**Manual Tests**:
- Application launch
- Main window rendering
- All menu functions
- Database connections
- Query execution
- Results display
- Settings persistence

**Performance Tests**:
- Query execution time
- Large result set rendering
- Application startup time

7. **Validation Checklist**
- [ ] Project builds without errors
- [ ] Project builds with < 50 warnings (Windows Forms compatibility warnings expected)
- [ ] SqlStudioTests pass
- [ ] Application launches successfully
- [ ] Main UI renders correctly
- [ ] Database connections work (all supported databases)
- [ ] Query execution functional
- [ ] Results display correctly
- [ ] Configuration saves/loads
- [ ] No runtime exceptions in common scenarios
- [ ] Performance within 10% of baseline

---

### Project: FormatTextControlTests

**Current State**: 
- TargetFramework: net8.0-windows7.0
- Project Type: Test Project
- Files: Unknown
- Issues: 1 total (1 mandatory - framework update)
- Dependencies: Common, FormatTextControl

**Target State**: 
- TargetFramework: net10.0-windows
- MSTest packages remain compatible

**Migration Steps**:

1. **Prerequisites**
   - Dependencies (Common, FormatTextControl) upgraded in same atomic operation

2. **Target Framework Update**
   - Change `<TargetFramework>net8.0-windows7.0</TargetFramework>` to `<TargetFramework>net10.0-windows</TargetFramework>`

3. **Package Updates**
   - Test framework packages remain compatible

4. **Expected Breaking Changes**
   - Framework update only
   - Test framework compatibility expected

5. **Code Modifications**
   - Update test expectations if FormatTextControl behavior changed
   - Minimal changes expected

6. **Testing Strategy**
   - Execute all tests after upgrade
   - Validate FormatTextControl functionality

7. **Validation Checklist**
- [ ] Project builds without errors
- [ ] All tests execute
- [ ] All tests pass
- [ ] Test coverage maintained

---

### Project: SqlCommandCompleterTests

**Current State**: 
- TargetFramework: net8.0
- Project Type: Test Project
- Files: Unknown
- Issues: 3 total (1 mandatory, 2 potential)
- Dependencies: Common, SqlCommandCompleter

**Target State**: 
- TargetFramework: net10.0
- Packages updated to latest

**Migration Steps**:

1. **Prerequisites**
   - Dependencies (Common, SqlCommandCompleter) upgraded in same atomic operation

2. **Target Framework Update**
   - Change `<TargetFramework>net8.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`

3. **Package Updates**
   - Test framework packages remain compatible

4. **Expected Breaking Changes**
   - Framework update only
   - Test infrastructure should remain compatible

5. **Code Modifications**
   - Update test expectations if SqlCommandCompleter behavior changed
   - Minimal changes expected

6. **Testing Strategy**
   - Execute all tests after upgrade
   - Validate SQL command completion functionality

7. **Validation Checklist**
- [ ] Project builds without errors
- [ ] All tests execute
- [ ] All tests pass
- [ ] Test coverage maintained

---

### Project: SqlStudioTests

**Current State**: 
- TargetFramework: net8.0-windows7.0
- Project Type: Test Project
- Files: Unknown
- Issues: 5 total (1 mandatory, 4 potential)
- Dependencies: SqlParser, CommandPrompt, SqlStudio

**Target State**: 
- TargetFramework: net10.0-windows
- Test packages updated

**Migration Steps**:

1. **Prerequisites**
   - Dependencies (SqlParser, CommandPrompt, SqlStudio) upgraded in same atomic operation
   - Tests entire application stack

2. **Target Framework Update**
   - Change `<TargetFramework>net8.0-windows7.0</TargetFramework>` to `<TargetFramework>net10.0-windows</TargetFramework>`

3. **Package Updates**
   - Test framework packages remain compatible

4. **Expected Breaking Changes**
   - Framework update
   - May need test expectation updates based on SqlStudio changes

5. **Code Modifications**
   - Update test expectations for behavioral changes in SqlStudio
   - Update mocks if API signatures changed
   - Adjust assertions if needed

6. **Testing Strategy**
   - Execute all tests after upgrade
   - Critical validation for main application
   - Investigate any failures thoroughly

7. **Validation Checklist**
- [ ] Project builds without errors
- [ ] All tests execute
- [ ] All tests pass (or failures understood and addressed)
- [ ] Test coverage maintained
- [ ] Integration scenarios validated

---

## Risk Management

### High-Risk Changes Summary

| Project | Risk Level | Issue Count | Primary Concerns | Mitigation |
|---------|------------|-------------|------------------|------------|
| SqlStudio | Critical | 8,976 mandatory | Windows Forms API compatibility, main application stability | Extensive testing, Windows Forms compatibility layer, incremental error fixing |
| CommandPrompt | High | 844 mandatory | UI component compatibility, used by main app and tests | Fix after establishing patterns in SqlStudio |
| FormatTextControl | High | 402 mandatory | Foundation UI library, affects 3 downstream projects | Fix early, validate downstream impact |
| Common | Medium | 1 mandatory | Widely used (7 consumers), potential cascade impact | Fix first, simple scope |
| CfgDataStore | Medium | 1 mandatory | Configuration layer, affects multiple projects | Validate configuration loading |

### Security Vulnerabilities

**Status**: No security vulnerabilities reported in assessment.

**Package Security Validation**: All Microsoft.* packages being updated to latest stable versions (10.0.3), ensuring latest security patches.

### Deprecated Package Risks

**Critical: Two deprecated packages require attention**

1. **PoorMansTSqlFormatterRedux** (version 1.0.3)
   - **Status**: ⚠️ NuGet package is deprecated
   - **Used By**: SqlStudio project
   - **Risk**: No security updates, potential future incompatibility
   - **Mitigation Options**:
     - Option A: Remove if not actively used in codebase
     - Option B: Replace with maintained alternative (research required)
     - Option C: Fork and maintain internally if critical
   - **Action**: Investigate actual usage in SqlStudio codebase before migration

2. **System.Data.SQLite** (version 1.0.119)
   - **Status**: ⚠️ NuGet package is deprecated
   - **Used By**: SqlStudio, SqlExecute
   - **Risk**: No .NET 10 specific support, potential compatibility issues
   - **Mitigation Options**:
     - Option A: Replace with `Microsoft.Data.Sqlite` (recommended by Microsoft)
     - Option B: Update to latest community-maintained `System.Data.SQLite` fork
     - Option C: Migrate to Entity Framework Core with SQLite provider
   - **Action**: Assess SQLite usage patterns, plan migration to Microsoft.Data.Sqlite

### Breaking Changes Catalog

**Expected Breaking Changes by Category**:

1. **Windows Forms API Changes**
   - **Scope**: 10,283 issues across SqlStudio, CommandPrompt, FormatTextControl
   - **Nature**: Mostly compatibility warnings, not hard breaks
   - **Examples**: 
     - Control property changes
     - Event handler signature updates
     - Layout engine differences
   - **Mitigation**: Most will compile with warnings, validate runtime behavior

2. **GDI+ / System.Drawing Changes**
   - **Scope**: 278 issues
   - **Nature**: Drawing API compatibility warnings
   - **Examples**:
     - Graphics object disposal patterns
     - Bitmap handling changes
     - Font rendering differences
   - **Mitigation**: Review custom drawing code, test visual appearance

3. **Legacy Configuration System**
   - **Scope**: 2 issues
   - **Nature**: ConfigurationManager API changes
   - **Mitigation**: Validate configuration loading, update to modern patterns if needed

4. **Package API Changes**
   - **Entity Framework Core 9.0.1 → 10.0.3**: Minor API refinements
   - **Microsoft.Extensions.* 9.0.1 → 10.0.3**: Dependency injection and logging updates
   - **System.Text.Json 9.0.1 → 10.0.3**: JSON serialization behavior changes

### Contingency Plans

**Scenario 1: Compilation Errors Exceed Expected Scope**
- **Trigger**: > 500 unique compilation errors after atomic upgrade
- **Action**: 
  1. Categorize errors by type and frequency
  2. Fix foundation libraries first (Common, FormatTextControl)
  3. Rebuild iteratively to reduce error count
  4. If > 1,000 errors persist after foundation fixes, reassess approach

**Scenario 2: Deprecated Package Replacement Blocked**
- **Trigger**: Cannot find suitable replacement for deprecated packages
- **Action**:
  1. PoorMansTSqlFormatterRedux: Remove dependency if unused, or accept deprecated package temporarily
  2. System.Data.SQLite: Keep existing version with explicit dependency resolution, plan separate migration

**Scenario 3: Windows Forms Runtime Issues**
- **Trigger**: Application builds but crashes or renders incorrectly at runtime
- **Action**:
  1. Enable detailed Windows Forms compatibility logging
  2. Test on multiple Windows versions (if applicable)
  3. Review .NET 10 Windows Forms known issues documentation
  4. Apply compatibility shims if available

**Scenario 4: Test Failures After Migration**
- **Trigger**: > 20% of tests fail after upgrade
- **Action**:
  1. Categorize failures (setup, assertion, runtime)
  2. Fix test infrastructure issues first
  3. Update test expectations for behavioral changes
  4. Investigate actual regressions vs test brittleness

**Scenario 5: Performance Degradation**
- **Trigger**: > 30% performance regression in key scenarios
- **Action**:
  1. Profile application to identify hotspots
  2. Review .NET 10 performance known issues
  3. Optimize affected code paths
  4. Consider targeted rollback of specific changes if critical

### Rollback Strategy

**Branch-Based Rollback**:
- All changes in dedicated `upgrade-to-NET10` branch
- Can discard branch entirely: `git checkout main; git branch -D upgrade-to-NET10`
- Source branch `main` remains unchanged

**Partial Rollback** (if needed):
- Commit structure allows selective revert of specific changes
- Can isolate package updates from framework updates if needed

**Rollback Decision Criteria**:
- Blocking issues preventing compilation after reasonable effort (> 8 hours)
- Critical functionality broken with no clear fix
- Performance degradation > 50% with no mitigation
- Security issues introduced by upgrade

### Risk Mitigation Timeline

**Before Starting**:
- [ ] Backup current main branch state
- [ ] Document current application behavior and performance baselines
- [ ] Ensure all tests pass on .NET 8.0 baseline
- [ ] Verify .NET 10 SDK installation

**During Atomic Upgrade**:
- [ ] Commit after project file updates (before compilation fixes)
- [ ] Track compilation error types and patterns
- [ ] Fix foundation libraries before dependent projects
- [ ] Rebuild incrementally to validate fixes

**After Compilation Success**:
- [ ] Run full test suite immediately
- [ ] Smoke test main application functionality
- [ ] Compare performance against baseline
- [ ] Review all compiler warnings

**Before Merging**:
- [ ] All tests passing
- [ ] No critical warnings
- [ ] Application functionality validated
- [ ] Performance acceptable
- [ ] Code review completed

---

## Testing & Validation Strategy

### Multi-Level Testing Approach

The All-At-Once strategy requires comprehensive testing after the atomic upgrade completes.

### Phase 1: Build Validation (During Atomic Upgrade)

**Objective**: Achieve clean compilation

**Activities**:
1. Build entire solution after project/package updates
2. Categorize compilation errors by type
3. Fix errors systematically (foundation libraries first)
4. Rebuild incrementally to verify fixes
5. Continue until 0 compilation errors

**Success Criteria**:
- Solution builds successfully
- 0 compilation errors
- < 50 Windows Forms compatibility warnings (acceptable)

### Phase 2: Unit Test Execution

**Test Projects** (3 total):
1. **FormatTextControlTests**
   - Tests: Text formatting, custom drawing, control behavior
   - Priority: High (foundation library)

2. **SqlCommandCompleterTests**
   - Tests: SQL command completion, keyword suggestions
   - Priority: Medium

3. **SqlStudioTests**
   - Tests: Main application integration scenarios
   - Priority: Critical (end-to-end validation)

**Execution Approach**:
- Run all test projects after successful compilation
- Can execute in parallel for efficiency
- Categorize failures: setup issues, assertion failures, runtime errors

**Success Criteria**:
- All test projects build
- > 95% test pass rate initially
- 100% test pass rate after investigation and fixes
- No new test infrastructure issues

### Phase 3: Integration Testing

**Database Connectivity** (Critical for SqlStudio):
- SQL Server connection and query execution
- MySQL connection and query execution
- PostgreSQL connection and query execution
- Oracle connection and query execution
- SQLite connection and query execution

**Entity Framework Operations**:
- DbContext initialization
- CRUD operations
- LINQ queries
- Migration application

**Configuration Management**:
- Configuration loading
- Configuration saving
- Settings persistence

**Success Criteria**:
- All database providers connect successfully
- Query execution works across all databases
- EF Core operations functional
- Configuration persists correctly

### Phase 4: Functional Validation

**Main Application Testing**:

**Startup & Initialization**:
- [ ] Application launches without errors
- [ ] Main window displays correctly
- [ ] Menu bar renders properly
- [ ] Toolbars functional
- [ ] Status bar displays

**Core Functionality**:
- [ ] New query window creation
- [ ] Database connection dialog
- [ ] Connect to database (all providers)
- [ ] Execute simple SELECT query
- [ ] Display query results
- [ ] Execute UPDATE/INSERT/DELETE queries
- [ ] Transaction support
- [ ] Query cancellation

**UI Components**:
- [ ] FormatTextControl renders correctly
- [ ] CommandPrompt control functional
- [ ] Syntax highlighting works
- [ ] SQL command completion functional
- [ ] Results grid displays properly
- [ ] Custom drawing/GDI+ operations work

**Data Operations**:
- [ ] Large result sets display
- [ ] Result set scrolling
- [ ] Data export functionality
- [ ] Binary data handling

**Advanced Features**:
- [ ] T-SQL formatting (if PoorMansTSqlFormatterRedux kept)
- [ ] Multiple query windows
- [ ] Split view functionality
- [ ] Settings dialog
- [ ] Theme/appearance settings

**Success Criteria**:
- All core functionality works
- UI renders correctly (no visual regressions)
- No runtime exceptions in common scenarios
- User workflows complete successfully

### Phase 5: Performance Validation

**Baseline Comparison**:
- Measure on .NET 8.0 before upgrade (baseline)
- Measure on .NET 10.0 after upgrade
- Compare key metrics

**Key Performance Metrics**:

1. **Application Startup Time**
   - Target: Within 10% of baseline
   - Measure: Time from launch to main window displayed

2. **Query Execution Time**
   - Target: Within 5% of baseline
   - Measure: Time to execute standard test query

3. **Large Result Set Rendering**
   - Target: Within 10% of baseline
   - Measure: Time to display 10,000 row result set

4. **Memory Usage**
   - Target: No significant increase (< 20%)
   - Measure: Working set after standard operations

**Success Criteria**:
- No performance metric degrades > 30%
- Most metrics within 10% of baseline
- .NET 10 improvements may show gains in some areas

### Phase 6: Compatibility & Visual Testing

**Windows Forms Visual Validation**:
- Compare screenshots of key UI elements before/after
- Validate control rendering across different DPI settings
- Test on different Windows versions (if applicable)
- Verify custom drawing operations

**Cross-Provider Database Testing**:
- Validate behavior consistency across all database providers
- Test edge cases (empty results, large results, special characters)

**Success Criteria**:
- Visual appearance matches baseline
- No rendering artifacts
- Consistent behavior across database providers

### Test Failure Response Strategy

**Categorize Failures**:

1. **Test Infrastructure Failures**
   - **Symptoms**: Tests don't run, framework errors
   - **Response**: Fix test project setup first
   - **Priority**: Critical

2. **Broken Test Expectations**
   - **Symptoms**: Tests fail due to behavioral changes
   - **Response**: Validate change is expected, update test
   - **Priority**: Medium

3. **Actual Regressions**
   - **Symptoms**: Functionality broken, exceptions
   - **Response**: Fix code, may require rollback if severe
   - **Priority**: Critical

**Escalation Criteria**:
- > 20% unit test failures → Stop and investigate
- Any critical functionality broken → Stop and assess
- Performance degradation > 50% → Stop and profile

### Test Documentation

**Test Results Template**:
```
## Test Execution Results - [Date]

### Build Validation
- Compilation: [Pass/Fail]
- Errors: [Count]
- Warnings: [Count]

### Unit Tests
- FormatTextControlTests: [X/Y passed]
- SqlCommandCompleterTests: [X/Y passed]
- SqlStudioTests: [X/Y passed]
- Overall Pass Rate: [%]

### Integration Tests
- SQL Server: [Pass/Fail]
- MySQL: [Pass/Fail]
- PostgreSQL: [Pass/Fail]
- Oracle: [Pass/Fail]
- SQLite: [Pass/Fail]

### Functional Validation
- Core Features: [Pass/Fail] - [Details]
- UI Rendering: [Pass/Fail] - [Details]

### Performance Metrics
- Startup Time: [Baseline] → [New] ([% change])
- Query Execution: [Baseline] → [New] ([% change])
- Large Result Rendering: [Baseline] → [New] ([% change])

### Issues Found
1. [Issue description] - [Severity] - [Status]
2. ...

### Recommendation
[Proceed / Investigate / Rollback]
```

### Validation Timeline

**Estimated Testing Effort** (relative):
- Build validation: 10% (part of atomic upgrade)
- Unit tests: 15%
- Integration tests: 25%
- Functional validation: 35%
- Performance validation: 10%
- Visual/compatibility testing: 5%

**Critical Path**: Functional validation of main SqlStudio application

---

## Complexity & Effort Assessment

### Overall Complexity Rating: **High**

**Primary Complexity Drivers**:
1. Large issue count (10,624 total, 10,233 mandatory)
2. Windows Forms migration complexity (10,283 issues)
3. Deep dependency chain (6 levels)
4. Deprecated package replacements required

### Per-Project Complexity Analysis

| Project | Complexity | Issues | Risk Factors | Dependency Impact |
|---------|------------|--------|--------------|-------------------|
| **SqlStudio** | **Critical** | 8,976 | Main WinForms app, 7 dependencies, largest issue count | Affects SqlStudioTests |
| **CommandPrompt** | **High** | 844 | UI component library, 3 dependencies, Windows Forms heavy | Used by SqlStudio, SqlStudioTests |
| **FormatTextControl** | **High** | 402 | Foundation UI library, Windows Forms controls | Used by 3 projects |
| **CfgDataStore** | Medium | 1 | Configuration layer, 2 dependencies | Used by 2 projects |
| **Common** | Medium | 1 | Widely used foundation (7 consumers) | Affects 7 projects |
| **SqlExecute** | Low | 3 | Service layer, 1 dependency, deprecated package | Used by 2 projects |
| **SqlCommandCompleter** | Low | 2 | Service layer, 1 dependency | Used by 2 projects |
| **BinaryFiles** | Low | 0 | No issues, simple library | Used by 1 project |
| **SqlParser** | Low | 1 | No dependencies, minimal issues | Used by tests |
| **FormatTextControlTests** | Low | 1 | Test project, simple upgrade | No consumers |
| **SqlCommandCompleterTests** | Low | 1 | Test project, simple upgrade | No consumers |
| **SqlStudioTests** | Low | 1 | Test project, simple upgrade | No consumers |

### Phase Complexity Assessment

**Phase 0: Prerequisites**
- **Complexity**: Low
- **Effort**: Minimal
- **Activities**: SDK verification, environment setup

**Phase 1: Atomic Upgrade**
- **Complexity**: High
- **Effort**: High
- **Activities**: 
  - Project file updates (Low complexity)
  - Package reference updates (Medium complexity)
  - Deprecated package handling (High complexity)
  - Compilation error fixing (High complexity - 10K+ issues)
  - Windows Forms compatibility (Critical complexity)

**Phase 2: Test Validation**
- **Complexity**: Medium
- **Effort**: Medium
- **Activities**:
  - Test execution (Low complexity)
  - Test failure investigation (Medium complexity)
  - Test updates for behavioral changes (Medium complexity)

**Phase 3: Final Validation**
- **Complexity**: Medium
- **Effort**: Medium
- **Activities**:
  - Manual smoke testing (Medium complexity)
  - Performance validation (Medium complexity)
  - Documentation updates (Low complexity)

### Relative Complexity by Category

**Framework Updates** (Low-Medium):
- Straightforward TargetFramework changes
- Well-defined from .NET 8 to .NET 10
- No multi-targeting complexity

**Package Updates** (Medium):
- 12 packages requiring updates
- All have clear version paths
- Microsoft.* packages move in lockstep
- 3 packages to remove (framework-included)

**Deprecated Package Resolution** (High):
- PoorMansTSqlFormatterRedux: Unknown replacement complexity
- System.Data.SQLite: Requires migration strategy decision
- Potential code changes depending on approach

**Compilation Error Resolution** (Critical):
- 10,233 mandatory issues
- 99.9% in 3 Windows Forms projects
- Most are compatibility warnings, not hard breaks
- May require API usage pattern changes
- Unknowns in exact error types until compilation

**Testing & Validation** (Medium):
- Well-structured test projects exist
- Unknown test failure rate post-upgrade
- Manual testing scope for WinForms application

### Resource Requirements

**Skills Required**:
- **Essential**: 
  - C# and .NET framework knowledge
  - Windows Forms development experience
  - NuGet package management
  - Build system troubleshooting

- **Helpful**:
  - Entity Framework Core experience
  - SQL Server / database knowledge
  - Windows Forms migration patterns
  - Performance profiling

**Team Capacity Considerations**:
- Single developer can execute (atomic upgrade)
- Parallel testing possible (3 test projects)
- Code review recommended for high-risk projects
- QA involvement for final validation

**Tool Requirements**:
- Visual Studio 2022 (17.12 or later) or VS Code
- .NET 10 SDK
- Git for version control
- Database tools for testing (SQL Server, SQLite, PostgreSQL, MySQL support)

### Complexity Mitigation Strategies

**For High Issue Count**:
- Leverage compiler diagnostics to categorize errors
- Fix by pattern, not individually
- Use IDE refactoring tools where applicable
- Prioritize foundation libraries to reduce cascade

**For Windows Forms Migration**:
- Consult .NET 10 Windows Forms migration documentation
- Test visual appearance early and often
- Use Windows Forms compatibility analyzers
- Review control-by-control if needed

**For Deprecated Packages**:
- Research replacement options before starting
- Test deprecated package compatibility on .NET 10 first
- Consider phased replacement if immediate replacement risky

**For Testing Complexity**:
- Automate test execution
- Categorize failures for efficient resolution
- Update test expectations systematically
- Involve QA for exploratory testing

### Effort Distribution Estimate

**Note**: Absolute time estimates avoided due to variability, but relative effort distribution:

- **Phase 0 (Prerequisites)**: 5% of total effort
- **Phase 1 (Atomic Upgrade)**: 70% of total effort
  - Project/package updates: 10%
  - Compilation error fixing: 50%
  - Deprecated package handling: 10%
- **Phase 2 (Test Validation)**: 15% of total effort
- **Phase 3 (Final Validation)**: 10% of total effort

**Critical Path**: Compilation error resolution in Windows Forms projects (SqlStudio, CommandPrompt, FormatTextControl)

---

## Source Control Strategy

### Branch Strategy

**Primary Branches**:
- **Source Branch**: `main` - Current stable .NET 8.0 version
- **Upgrade Branch**: `upgrade-to-NET10` - All upgrade work (already created)
- **Main Branch**: `main` - Target for final merge

### Commit Strategy

**All-At-Once Approach**: Single comprehensive commit preferred

**Recommended Commit Structure** (Single Atomic Commit):

```
feat: Upgrade solution to .NET 10.0

- Update all 12 projects from .NET 8.0 to .NET 10.0
- Update Microsoft.* packages to 10.0.3
- Update System.Text.Json to 10.0.3
- Remove framework-included packages (System.ComponentModel.Annotations, System.Memory, System.Data.DataSetExtensions)
- Resolve deprecated package issues (PoorMansTSqlFormatterRedux, System.Data.SQLite)
- Fix Windows Forms API compatibility issues (10,233 fixes)
- Fix GDI+ compatibility issues
- Update Entity Framework Core to 10.0.3
- All tests passing
- Performance validated

Projects upgraded:
- SqlStudio (main app)
- CommandPrompt, FormatTextControl (UI components)
- Common, SqlExecute, CfgDataStore, SqlCommandCompleter (libraries)
- BinaryFiles, SqlParser (utilities - kept on netstandard2.0)
- All test projects

Breaking changes addressed:
- Windows Forms API updates
- GDI+ drawing operations
- Entity Framework Core 10.0 compatibility
- Package API changes

BREAKING CHANGE: Requires .NET 10.0 SDK
```

**Alternative: Staged Commits** (if atomic commit too large):

1. **Commit 1: Project Files & Package Updates**
   ```
   feat: Update project files and packages for .NET 10.0

   - Update all TargetFramework properties to net10.0(-windows)
   - Update Microsoft.* packages to 10.0.3
   - Update System.Text.Json to 10.0.3
   - Remove framework-included packages

   Note: Solution will not compile after this commit
   ```

2. **Commit 2: Compilation Fixes**
   ```
   fix: Resolve .NET 10.0 compilation errors

   - Fix Windows Forms API compatibility (10,233 issues)
   - Fix GDI+ compatibility issues
   - Update API usage patterns
   - Resolve deprecated package issues

   Solution now builds successfully
   ```

3. **Commit 3: Test Updates**
   ```
   test: Update tests for .NET 10.0 compatibility

   - Update test expectations for behavioral changes
   - Fix test project compatibility
   - All tests passing
   ```

**Recommendation**: **Single atomic commit** - aligns with All-At-Once strategy, easier to review and rollback if needed.

### Commit Timing

**During Atomic Upgrade**:
- No commits until upgrade complete and compiling
- All changes accumulated in working directory
- Single commit after successful compilation

**After Testing**:
- Commit test fixes separately if needed
- Tag commit: `net10.0-upgrade-complete`

### Review and Merge Process

**Pull Request Requirements**:

**PR Title**: `Upgrade solution to .NET 10.0 LTS`

**PR Description Template**:
```markdown
## Overview
Upgrades entire SqlStudio solution from .NET 8.0 to .NET 10.0 LTS.

## Changes Summary
- **Projects Upgraded**: 12 (10 to net10.0, 2 kept on netstandard2.0)
- **Packages Updated**: 12 packages to version 10.0.3
- **Packages Removed**: 3 framework-included packages
- **Deprecated Packages**: 2 addressed (details below)
- **Compilation Issues Fixed**: 10,233
- **Tests Status**: All passing

## Detailed Changes

### Project Upgrades
- SqlStudio: net8.0-windows7.0 → net10.0-windows
- CommandPrompt: net8.0-windows7.0 → net10.0-windows
- FormatTextControl: net8.0-windows7.0 → net10.0-windows
- Common: net8.0 → net10.0
- SqlExecute: net8.0-windows7.0 → net10.0-windows
- CfgDataStore: net8.0-windows7.0 → net10.0-windows
- SqlCommandCompleter: net8.0 → net10.0
- Test Projects: net8.0(-windows7.0) → net10.0(-windows)
- BinaryFiles: netstandard2.0 (no change)
- SqlParser: netstandard2.0 (no change)

### Package Updates
| Package | Old Version | New Version |
|---------|-------------|-------------|
| Microsoft.EntityFrameworkCore | 9.0.1 | 10.0.3 |
| Microsoft.EntityFrameworkCore.Sqlite | 9.0.1 | 10.0.3 |
| Microsoft.EntityFrameworkCore.Sqlite.Core | 9.0.1 | 10.0.3 |
| Microsoft.Extensions.* (5 packages) | 9.0.1 | 10.0.3 |
| Microsoft.Windows.Compatibility | 9.0.1 | 10.0.3 |
| System.Configuration.ConfigurationManager | 9.0.1 | 10.0.3 |
| System.Text.Json | 9.0.1 | 10.0.3 |
| System.Runtime.CompilerServices.Unsafe | 6.1.0 | 6.1.2 |

### Packages Removed (in framework)
- System.ComponentModel.Annotations
- System.Data.DataSetExtensions
- System.Memory

### Deprecated Packages
- **PoorMansTSqlFormatterRedux**: [Kept/Removed/Replaced] - [Details]
- **System.Data.SQLite**: [Kept/Migrated to Microsoft.Data.Sqlite] - [Details]

## Breaking Changes
- Windows Forms API compatibility updates (10,233 fixes)
- GDI+ / System.Drawing updates
- Entity Framework Core 10.0 API changes
- Requires .NET 10.0 SDK for building

## Testing
- [x] All projects build successfully
- [x] Unit tests: [X/Y] passing
- [x] Integration tests: All database providers tested
- [x] Functional validation: Core features verified
- [x] Performance validation: Within acceptable thresholds
- [x] Visual testing: UI rendering validated

## Performance Impact
- Startup time: [baseline] → [new] ([X%] change)
- Query execution: [baseline] → [new] ([X%] change)
- Memory usage: [baseline] → [new] ([X%] change)

## Rollback Plan
If issues discovered post-merge:
1. Revert this PR
2. Branch remains available: `upgrade-to-NET10`
3. Can investigate issues and re-submit

## Checklist
- [ ] All projects build without errors
- [ ] Compiler warnings reviewed and acceptable
- [ ] All tests passing
- [ ] Integration testing complete
- [ ] Performance validated
- [ ] Documentation updated (if needed)
- [ ] Breaking changes documented
```

**Review Checklist**:
- [ ] All project files updated correctly
- [ ] Package versions consistent
- [ ] No unintended changes
- [ ] Compilation successful
- [ ] Tests passing
- [ ] No security vulnerabilities introduced
- [ ] Performance acceptable
- [ ] Breaking changes documented

**Merge Criteria**:
1. All CI/CD checks pass (if applicable)
2. Code review approved
3. All tests passing
4. Performance validated
5. No blocking issues identified

### Post-Merge Process

**After Successful Merge**:
1. Tag main branch: `git tag -a v1.0.0-net10.0 -m ".NET 10.0 upgrade complete"`
2. Push tag: `git push origin v1.0.0-net10.0`
3. Delete upgrade branch (optional): `git branch -d upgrade-to-NET10`
4. Update documentation with .NET 10.0 requirements
5. Notify team of upgrade completion

**If Rollback Needed**:
1. Identify specific issue causing rollback
2. Document issue for future reference
3. Revert merge commit: `git revert <merge-commit-sha>`
4. Push revert to main
5. Branch `upgrade-to-NET10` remains for future fixes
6. Address issues in upgrade branch
7. Re-submit when resolved

### Branch Lifecycle

```
main (NET 8.0)
  │
  └─── upgrade-to-NET10 (created at start)
         │
         ├─ Atomic upgrade work
         ├─ Compilation fixes
         ├─ Testing
         ├─ Validation
         │
         └─ Merge → main (NET 10.0)
               │
               └─ Tag: v1.0.0-net10.0
```

### Git Workflow Commands

**Current Status** (already executed):
```bash
git checkout -b upgrade-to-NET10  # Already done
```

**After Upgrade Complete** (single commit approach):
```bash
# Stage all changes
git add .

# Commit with comprehensive message
git commit -m "feat: Upgrade solution to .NET 10.0

[Use template above]"

# Push to remote
git push -u origin upgrade-to-NET10

# Create PR (via GitHub/Azure DevOps UI)
```

**After PR Approved**:
```bash
# Merge to main (via UI or command line)
git checkout main
git merge upgrade-to-NET10

# Tag the release
git tag -a v1.0.0-net10.0 -m ".NET 10.0 upgrade complete"
git push origin main --tags
```

### Version Control Best Practices

1. **Commit frequently during development** but squash for final PR
2. **Write descriptive commit messages** following conventional commits
3. **Reference issues/tickets** if applicable
4. **Include test results** in commit messages or PR description
5. **Document breaking changes** clearly
6. **Tag significant milestones** for easy reference
7. **Keep upgrade branch** until confident in production stability

---

## Success Criteria

The .NET 10.0 upgrade is considered successful when ALL of the following criteria are met:

### 1. Technical Criteria

#### Build & Compilation
- [ ] All 12 projects build successfully without errors
- [ ] Compiler warnings < 50 (Windows Forms compatibility warnings acceptable)
- [ ] No blocking compilation errors remain
- [ ] Solution configuration valid for Debug and Release

#### Target Framework Migration
- [ ] 10 projects successfully targeting net10.0 or net10.0-windows
- [ ] 2 projects on netstandard2.0 (BinaryFiles, SqlParser) if decision made to keep them
- [ ] No projects remaining on net8.0
- [ ] TargetFramework properties correctly set in all .csproj files

#### Package Updates
- [ ] All 12 required packages updated to version 10.0.3:
  - [ ] Microsoft.EntityFrameworkCore
  - [ ] Microsoft.EntityFrameworkCore.Sqlite
  - [ ] Microsoft.EntityFrameworkCore.Sqlite.Core
  - [ ] Microsoft.Extensions.Caching.Memory
  - [ ] Microsoft.Extensions.Configuration.Binder
  - [ ] Microsoft.Extensions.Hosting
  - [ ] Microsoft.Extensions.Identity.Core
  - [ ] Microsoft.Extensions.Logging
  - [ ] Microsoft.Windows.Compatibility
  - [ ] System.Configuration.ConfigurationManager
  - [ ] System.Text.Json
  - [ ] System.Runtime.CompilerServices.Unsafe (to 6.1.2)
- [ ] 3 framework-included packages removed:
  - [ ] System.ComponentModel.Annotations
  - [ ] System.Data.DataSetExtensions
  - [ ] System.Memory
- [ ] 2 deprecated packages addressed:
  - [ ] PoorMansTSqlFormatterRedux (removed, replaced, or explicitly acknowledged)
  - [ ] System.Data.SQLite (migrated or explicitly acknowledged)

#### API Compatibility
- [ ] All 10,233 mandatory compatibility issues resolved
- [ ] Windows Forms API usage updated (8,973 issues)
- [ ] GDI+ / System.Drawing issues resolved (278 issues)
- [ ] Windows Forms Legacy Controls updated (898 issues)
- [ ] Legacy Configuration System updated (2 issues)
- [ ] Source incompatibilities addressed (201+ issues)
- [ ] Behavioral changes validated (6+ issues)

#### Testing Results
- [ ] **Unit Tests**: All test projects build and execute
  - [ ] FormatTextControlTests: 100% passing
  - [ ] SqlCommandCompleterTests: 100% passing
  - [ ] SqlStudioTests: 100% passing
- [ ] **Integration Tests**: All database providers functional
  - [ ] SQL Server connectivity and query execution
  - [ ] MySQL connectivity and query execution
  - [ ] PostgreSQL connectivity and query execution
  - [ ] Oracle connectivity and query execution
  - [ ] SQLite connectivity and query execution
- [ ] **Entity Framework**: All EF Core operations working
  - [ ] DbContext initialization
  - [ ] CRUD operations
  - [ ] LINQ queries
  - [ ] Migrations (if applicable)

### 2. Quality Criteria

#### Code Quality
- [ ] No introduction of code smells or anti-patterns
- [ ] Code follows existing project conventions
- [ ] No unnecessary changes beyond upgrade scope
- [ ] Comments updated where API changes occurred
- [ ] No `// TODO` or `// HACK` comments introduced without tracking

#### Test Coverage
- [ ] Test coverage maintained at pre-upgrade levels
- [ ] No tests disabled without justification
- [ ] New tests added if behavioral changes require validation
- [ ] Test quality maintained (no brittle tests)

#### Security
- [ ] No new security vulnerabilities introduced
- [ ] All security-updated packages applied
- [ ] Deprecated packages with security concerns addressed
- [ ] No sensitive data exposed in commits

#### Performance
- [ ] Application startup time within 10% of baseline
- [ ] Query execution time within 5% of baseline
- [ ] Large result set rendering within 10% of baseline
- [ ] Memory usage increase < 20%
- [ ] No performance regressions > 30% in any scenario

#### Visual Quality
- [ ] UI rendering identical or improved compared to .NET 8.0
- [ ] No visual artifacts or layout issues
- [ ] Custom drawing operations render correctly
- [ ] Syntax highlighting functional
- [ ] All controls display properly

### 3. Functional Criteria

#### Core Application Functionality
- [ ] **Startup**: Application launches without errors
- [ ] **Database Connection**: Can connect to all supported databases
- [ ] **Query Execution**: Can execute SELECT, INSERT, UPDATE, DELETE queries
- [ ] **Results Display**: Query results display correctly
- [ ] **Data Operations**: Large result sets, scrolling, export functional
- [ ] **UI Components**: All custom controls render and function correctly
  - [ ] FormatTextControl
  - [ ] CommandPrompt
  - [ ] SQL Script display
  - [ ] Results grid
- [ ] **Configuration**: Settings load, save, and persist correctly
- [ ] **Advanced Features**: Multi-window, split view, formatting functional

#### No Regressions
- [ ] All features working in .NET 8.0 still work in .NET 10.0
- [ ] No unexpected behavioral changes
- [ ] No new runtime exceptions
- [ ] No data corruption or loss

### 4. Process Criteria

#### All-At-Once Strategy Compliance
- [ ] All projects updated simultaneously in atomic operation
- [ ] No intermediate multi-targeting states
- [ ] Single coordinated upgrade completed
- [ ] Strategy principles followed throughout

#### Documentation
- [ ] README updated with .NET 10.0 requirements
- [ ] Build instructions updated
- [ ] Breaking changes documented
- [ ] Migration notes created (if applicable)
- [ ] Developer setup guide updated

#### Source Control
- [ ] All changes committed to `upgrade-to-NET10` branch
- [ ] Commit messages clear and descriptive
- [ ] No unintended files committed
- [ ] .gitignore properly configured
- [ ] Branch ready for PR

#### Team Readiness
- [ ] Team notified of upgrade
- [ ] .NET 10.0 SDK installation instructions provided
- [ ] Known issues documented
- [ ] Support plan in place for post-merge issues

### 5. Deployment Readiness

#### Environment Requirements
- [ ] .NET 10.0 SDK version documented
- [ ] Runtime requirements specified
- [ ] Dependencies documented
- [ ] Deployment process validated (if applicable)

#### Rollback Plan
- [ ] Rollback procedure documented
- [ ] Branch preserved for potential fixes
- [ ] Known rollback risks identified
- [ ] Rollback criteria defined

### Acceptance Gate

**The upgrade can proceed to merge when**:
- ✅ All Technical Criteria met (100%)
- ✅ All Quality Criteria met (100%)
- ✅ All Functional Criteria met (100%)
- ✅ Process Criteria substantially met (> 90%)
- ✅ Deployment Readiness criteria met

**The upgrade should NOT merge if**:
- ❌ Any critical functionality broken
- ❌ Test pass rate < 95%
- ❌ Performance degradation > 30% in any area
- ❌ Security vulnerabilities introduced
- ❌ Build errors remain

### Post-Merge Success Criteria

**Within 1 week of merge**:
- [ ] No critical bugs reported
- [ ] No rollback required
- [ ] Team successfully building with .NET 10.0
- [ ] Production deployment successful (if applicable)

**Within 1 month of merge**:
- [ ] No major issues discovered
- [ ] Performance metrics stable
- [ ] Team fully transitioned to .NET 10.0 development
- [ ] Upgrade considered stable

### Metrics Dashboard

Track these metrics before and after upgrade:

| Metric | Baseline (.NET 8.0) | Target (.NET 10.0) | Actual | Status |
|--------|---------------------|---------------------|--------|--------|
| Build Success | 100% | 100% | | |
| Test Pass Rate | [%] | 100% | | |
| Startup Time (ms) | [X] | < X * 1.1 | | |
| Query Time (ms) | [Y] | < Y * 1.05 | | |
| Memory (MB) | [Z] | < Z * 1.2 | | |
| Warnings | [W] | < 50 | | |
| Critical Bugs | 0 | 0 | | |

### Sign-Off Checklist

Before declaring upgrade complete:

- [ ] **Technical Lead**: All technical criteria verified
- [ ] **QA/Tester**: All testing complete and passing
- [ ] **Developer(s)**: Code quality and implementation verified
- [ ] **Product Owner**: Functionality validated, no regressions
- [ ] **DevOps**: Build and deployment validated (if applicable)

**Upgrade Completion Date**: __________

**Signed Off By**: __________

---

## Final Notes

This upgrade to .NET 10.0 LTS provides:
- ✅ **Long-term support** until November 2027
- ✅ **Performance improvements** in runtime and libraries
- ✅ **Security enhancements** with latest patches
- ✅ **Modern tooling** and development experience
- ✅ **Future-proofing** for continued .NET evolution

The All-At-Once strategy enables rapid, coordinated upgrade with clear validation at each step. Success requires disciplined execution, comprehensive testing, and team coordination.
