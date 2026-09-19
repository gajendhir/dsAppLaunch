# dsAppLaunch Programmer Reference

Technical reference for building, running, packaging and extending
dsAppLaunch. For a non-technical walkthrough of the app itself, see the
[User Guide](userguide.md).

## Overview

dsAppLaunch is a small WinForms desktop application. It reads a flat JSON
file describing a list of applications, shows them as a card grid grouped by
a `Group` label, and launches them individually or as a ticked set
("Launch Selected"). A modal settings dialog edits the same file. There is
no database, no server component and no network access of its own.

## Tech stack

| Layer | Choice |
|---|---|
| UI framework | Windows Forms (`UseWindowsForms=true`), designer-generated layouts |
| Target framework | `net10.0-windows8.0` (SDK-style `.vbproj`) |
| Language | VB.NET, `Option Strict On` |
| Configuration | `apps.json`, serialized with `System.Text.Json` |
| Config location | `%AppData%\Data Spec\dsAppLaunch\apps.json` |
| Installer | WiX Toolset v6 (`Installer/`), per-machine MSI |
| Third-party packages | None — the application uses only the .NET base class library and WinForms |

## Build & run

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download) on
Windows. Open `dsAppLaunch.slnx` in Visual Studio and run the `dsAppLaunch`
project, or from the command line:

```
dotnet build dsAppLaunch.vbproj
dotnet run --project dsAppLaunch.vbproj
```

Output goes to the usual `bin\<Configuration>\` folder. (If a `d:\output`
folder exists on the build machine the project redirects output there
instead; see the conditional property group in `dsAppLaunch.vbproj`.)

No seed data or setup is needed. On first run the application creates its
configuration folder and an empty `apps.json`. To start with example data,
copy [apps.sample.json](../apps.sample.json) to
`%AppData%\Data Spec\dsAppLaunch\apps.json`.

### Version numbers

The assembly `Version` is computed at build time as
`Major.Minor.<days since 2000-01-01>.<seconds since midnight / 2>` (see the
`PropertyGroup` in `dsAppLaunch.vbproj`); bump `MajorVersion` /
`MinorVersion` by hand. The MSI carries its own three-part version in
`Installer/Package.wxs` because Windows Installer ignores the fourth field
when deciding whether a package is newer.

## Project structure

```
dsAppLaunch.vbproj        project file (WinForms, net10.0-windows8.0)
dsAppLaunch.slnx          single-project solution
My Project/               Application.myapp (startup form), generated designer file
frmMain.vb / .Designer.vb   main window: card grid, toolbar, Launch Selected
AppCard.vb / .Designer.vb   UserControl for one card
frmSettings.vb / .Designer.vb   modal editor for the app list
AppConfig.vb              one configured application (JSON record)
AppRepository.vb          the only code that reads/writes apps.json
AppLauncher.vb            starts a process, returns a result (no UI)
AppProbe.vb               background check: exe exists / date / icon
IconProvider.vb           icon resolution and caching
AppGrouping.vb            grouping and ordering rules (pure logic)
SelectionState.vb         session-only set of ticked cards
apps.sample.json          example configuration
Installer/                WiX v6 project for the MSI
docs/                     this documentation
```

## Architecture

### Layering

Forms contain UI code only (event handlers and control state). Logic lives in
`Shared` service classes that have no UI dependencies: `AppRepository`,
`AppLauncher`, `IconProvider`, `AppProbe`, `AppGrouping` and
`SelectionState`. There are no interfaces or dependency-injection container —
with three screens and one consumer per class the extra indirection would not
pay for itself. `AppGrouping` in particular is pure and straightforward to
unit test if a test project is added.

### Components

| Type | Responsibility |
|---|---|
| `frmMain` | Fills a `FlowLayoutPanel` with one `AppCard` per app; handles reload, settings, All/None, Launch Selected |
| `AppCard` | One card. `Bind(probe)` fills it from an `AppProbe`; raises `LaunchRequested`, `SettingsRequested`, `SelectionChanged`; `SetBusy` shows the starting state |
| `frmSettings` | Modal editor. Owns its own in-memory `BindingList(Of AppConfig)`; drag and drop adds entries or sets exe/icon paths |
| `AppConfig` | POCO serialized directly by `System.Text.Json` |
| `AppRepository` | `Load`, `Save`, `EnsureExists`; one cached serializer options set |
| `AppLauncher` | Starts a process and returns a `LaunchResult`; `LaunchManyAsync` for the ticked set |
| `AppProbe` | Per-app `Probe`/`Placeholder`: does the exe exist, its date, its icon. Safe to run off the UI thread |
| `IconProvider` | Custom icon, then the exe's icon, then a system icon; cached by path and timestamp |
| `AppGrouping` | Groups by `Group`; within a group sorts by `SortOrder` ascending, then `SubGroup` descending, then file order |

### Loading the card grid

`frmMain.LoadApplications`:

1. Reads the configuration (`AppRepository.Load`) and groups/sorts it
   (`AppGrouping`).
2. Builds every card immediately using `AppProbe.Placeholder` (generic icon,
   no date), so the window is usable at once.
3. For each app runs `AppProbe.Probe` on a background task; the result is
   marshalled back with `BeginInvoke` and re-binds that card. A load counter
   discards results from a superseded reload.
4. On reload the old cards are disposed. Leaving them alive leaks GDI/USER
   handles.

Application paths are usually network shares. Probing synchronously blocked
the UI thread for the full network timeout per unreachable server, which is
why probing is off-thread.

### Selection and multi-launch

`AppConfig.Selected` is the persistent *default*. On each (re)load `frmMain`
seeds `SelectionState` from it; ticking a card changes `SelectionState` only
and is never written to `apps.json`. Launch Selected runs the ticked apps in
on-screen order, one at a time with a one second stagger, and reports only
failures or missing files in a single summary. `SortOrder` therefore also
decides the launch order.

### Data flow

```
apps.json  --read-->  frmMain.LoadApplications()  --builds-->  card grid
apps.json  --read-->  frmSettings.LoadData()       --edits-->  BindingList (memory)
BindingList --write--> apps.json                    (Save & Apply Changes)
frmSettings returns DialogResult.OK --> frmMain re-reads apps.json
```

The two forms share no in-memory model: the settings dialog does its own
load/save against the file and the main window reloads from disk when it
closes. That is adequate for a single-window modal flow; a non-modal or
multi-window design would need a shared model or a file watcher.

## Configuration file

`apps.json` is a JSON array of records:

```json
[
  {
    "Id": "00000000-0000-0000-0000-000000000001",
    "Name": "Notepad",
    "ExecutablePath": "C:\\Windows\\System32\\notepad.exe",
    "IconPath": "",
    "Category": "Utilities",
    "Description": "Sample entry",
    "Group": "Sample",
    "SubGroup": "Accessories",
    "CardColor": "#FFFFFF",
    "SortOrder": 1,
    "Selected": false
  }
]
```

| Field | Type | Notes |
|---|---|---|
| `Id` | string | GUID, generated when an entry is created; stable across runs |
| `Name` | string | Card title |
| `ExecutablePath` | string | Local or UNC path. Any file type is accepted and started through its shell association |
| `IconPath` | string | Optional custom icon/image; empty means use the exe's icon |
| `Category`, `Description` | string | Shown on the card |
| `Group` | string | Heading; empty entries appear under `(Ungrouped)` |
| `SubGroup` | string | Label on the card; secondary sort key (descending) |
| `CardColor` | string | Hex colour, e.g. `#E3F2FD`. Invalid values fall back to the default |
| `SortOrder` | int | Position within the group, lower first; default 0 |
| `Selected` | bool | Default tick for Launch Selected |

Property names are read case-insensitively. Missing fields take their
defaults, so files written by older versions keep loading.

### Where the file lives

`AppRepository.FilePath` is
`%AppData%\Data Spec\dsAppLaunch\apps.json` — per-user and writable, so an
installation under Program Files can still save. `AppRepository.EnsureExists`
creates the folder and, when no file exists yet, copies an `apps.json` found
next to the executable once (versions before 2.3 kept it there), otherwise
writes `[]`. If the file exists but cannot be parsed, the main window shows
the error and the settings dialog refuses to open so a Save cannot overwrite
the damaged file.

### Legacy keys

Earlier versions used `FirmName` and `FinancialYear` for what are now `Group`
and `SubGroup`. `AppConfig` keeps two set-only alias properties bound to the
old JSON names, so old files load; because they are set-only they are never
serialized and the next Save migrates the file. When a file with old keys is
loaded, `AppRepository` copies it once to `apps.json.bak`. The aliases can be
deleted once no old configuration files remain.

## Coding conventions

- **Layout in the designer, logic elsewhere.** Add controls in the
  `*.Designer.vb` files (keep them in the format Visual Studio generates), not
  in hand-written `New Button With {...}` blocks. Only data-driven UI — group
  headings and one card per app — is created at runtime.
- **Forms with constructor arguments** (`frmSettings`) also need a
  parameterless `New()` that calls `InitializeComponent()`, and must not load
  data in the constructor; use the `Load` event.
- **Public properties on controls** need `Browsable(False)` and
  `DesignerSerializationVisibility.Hidden`, or the build fails with WFO1000.
- **Label heights must fit one line of their font.** A single-line label with
  `AutoEllipsis` that is shorter than the font's line height renders nothing
  when the text is too long, instead of `abc...`.
- **Buttons whose caption contains `&`** need `UseMnemonic = False`.
- **`Option Strict On`** applies project-wide.
- **Events:** use `Handles` for designer controls; `AddHandler` only for
  controls created at runtime.
- **Errors** from file I/O and process launch are caught at the UI boundary
  and shown with `MessageBox`; there is no logging framework.
- **Colours** are hex strings converted with `ColorTranslator`, always inside
  `Try/Catch` with a default fallback, because the file can be hand-edited.
- **Configuration compatibility:** new `AppConfig` fields need safe defaults;
  renamed fields need a legacy-key alias.

## Extending

- **New card field:** add the property to `AppConfig` (with a default), a
  control in `frmSettings.Designer.vb` plus load/save handling in
  `frmSettings.vb`, and display it in `AppCard`.
- **Different ordering or grouping:** edit `AppGrouping.vb`; both the card
  grid and Launch Selected use its result.
- **Different config location:** change `AppRepository.FilePath`; nothing else
  reads or writes the file.

## Packaging (MSI installer)

The installer is a [WiX Toolset v6](https://wixtoolset.org/) project in
`Installer/`. It installs a self-contained x64 build (no .NET runtime
required on the target machine) to `Program Files\Data Spec Application
Launcher`, adds Start menu and desktop shortcuts, shows the licence, and uses
`MajorUpgrade` so a newer MSI replaces an older one. User configuration in
`%AppData%` is never touched by install, upgrade or uninstall.

Prerequisites: .NET 10 SDK and the WiX CLI
(`dotnet tool install --global wix --version 6.0.2`).

```
dotnet publish dsAppLaunch.vbproj -c Release -r win-x64 --self-contained true -o Installer/publish
dotnet build Installer/dsAppLaunch.wixproj -c Release
```

The MSI is written to `Installer/bin/x64/Release/dsAppLaunch.msi`. Before
releasing, raise `Version` in `Installer/Package.wxs`; leave `UpgradeCode`
unchanged so upgrades keep working. Attach the MSI to a GitHub Release rather
than committing it to the repository.

## License

MIT — see [LICENSE.txt](../LICENSE.txt).
