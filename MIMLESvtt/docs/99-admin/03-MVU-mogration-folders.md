Use this simple structure:

```text
/Models
  Board.cs
  Token.cs
  Player.cs
  Position.cs
  GameSession.cs

/Features
  /Tabletop
    TabletopPage.razor
    TabletopState.cs
    TabletopActions.cs
    TabletopUpdate.cs

    /Components
      BoardView.razor
      TokenView.razor
      ToolbarView.razor
      DicePanel.razor

  /GameSetup
    GameSetupPage.razor
    GameSetupState.cs
    GameSetupActions.cs
    GameSetupUpdate.cs

  /Settings
    SettingsPage.razor
    SettingsState.cs
    SettingsActions.cs
    SettingsUpdate.cs

/Services
  SaveGameService.cs
  LoadGameService.cs
  ModuleImportService.cs

/GameModules
  /Checkers
    CheckersRules.cs
    CheckersSetup.cs

  /Chess
    ChessRules.cs
    ChessSetup.cs

/Shared
  MainLayout.razor
  NavMenu.razor

/wwwroot
  app.css
```

## What each file means

```text
State.cs    = current data for that screen
Actions.cs  = things that can happen
Update.cs   = old state + action → new state
Page.razor  = screen and event dispatch
Components  = visual pieces
Services    = save/load/database/files
Models      = shared business objects
```

## The key rule

```text
MVU files stay in Features.
Persistence and database stay in Services.
Shared nouns stay in Models.
```

Do **not** start with:

```text
/Domain
/Application
/Infrastructure
/Repositories
/UseCases
```

Add those only if pain forces you.
