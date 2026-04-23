# VTT Workspace Launch Flow Diagram


## 1. System Flow (Engine Perspective)

```mermaid
flowchart TD

    A[Start] --> B[Open VTT Application]
    B --> C[Show Launch / Home Screen]

    C --> D{Choose Game System}

    D --> D1[Checkers]
    D --> D2[Chess]
    D --> D3[Go]
    D --> D4[AD&D]

    D1 --> E
    D2 --> E
    D3 --> E
    D4 --> E

    E{New or Load Session?}

    E --> F[New Session]
    E --> G[Load Existing Session]

    F --> H[Choose Scenario / Setup]
    H --> I[Build Session Configuration]

    G --> J[Select Saved Session]
    J --> K[Load Session State]

    I --> L[Create VttSession]
    K --> M[Restore VttSession]

    L --> N[Initialize Workspace]
    M --> N

    N --> O[Load Game Module]
    O --> P[Render Board / UI]

    P --> Q[Workspace Ready]


```

## 1b. Launch + Game Selector Flow (UI System Perspective)

```mermaid
flowchart TD

    A[Start] --> B[Open VTT Application]
    B --> C[Show Launch/Home Screen]
    C --> D[Open Game Selection Dialog]

    D --> E[Load subscribed games list]
    E --> F[Render Saved Sessions list]

    F --> G{Choose action}
    G --> H[Open Saved Session]
    G --> I[Join Existing Game]
    G --> J[Start New Session]

    H --> H1[Load selected VttSession]
    H1 --> Z[Initialize Workspace]

    I --> I1[Enter Join Code / Select Hosted Game]
    I1 --> I2[Validate access + connect]
    I2 --> Z

    J --> K{User has admin rights?}
    K -->|Yes| K1[Create New Session]
    K -->|No| K2[Show access denied message]

    K1 --> K3[Optional scenario/setup selection]
    K3 --> Z
```

## 2. Workspace Right-Slider + Tabletop Edit Flow (UI Perspective)

```mermaid
flowchart TD

    A[Workspace Route Opened] --> B[Render Tabletop Layer]
    B --> C[Default Tabletop State: x-y grid]
    C --> D[Show Workspace Controls Button]

    D --> E{Open Right Slider?}
    E -->|Yes| F[Slide Panel In From Right]
    E -->|No| C

    F --> G[Choose Tabletop Surface State]
    G --> G1[Default x-y grid]
    G --> G2[Missing tabletop]
    G --> G3[Page not loading]

    G1 --> H[Render Grid Behind Controls]
    G2 --> I[Render Missing Tabletop Status]
    G3 --> J[Render Page-Load Error Status]

    F --> K[Use Workspace Controls]
    K --> K1[Create / Load / Save Session]
    K --> K2[Add Surface / Add Piece]
    K --> K3[Move / Rotate / Marker / State Updates]
    K --> K4[Stamp / Queue / Preset Actions]

    K1 --> L[Session State Updated]
    K2 --> L
    K3 --> L
    K4 --> L

    L --> M[Tabletop Re-renders]
    M --> N[Operation Feedback Message Shown]

    F --> O{Close Right Slider?}
    O -->|Yes| P[Hide Controls, Keep Tabletop Visible]
    O -->|No| K

    P --> Q[Continue Tabletop Interaction]
```

