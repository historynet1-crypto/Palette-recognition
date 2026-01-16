# PALETTE RECOGNITION - WIREFRAME & FLOW DIAGRAM

## Project Overzicht
Een interactief kunstwerk quiz-spel waarbij gebruikers schilderijen moeten herkennen met een hint-systeem en authenticatie.

---

## AUTHENTICATION FLOW

```
┌─────────────────┐      ┌──────────────────┐      ┌─────────────────────┐
│  1. Login       │──→   │  2. Klik         │──→   │  3. Register        │
│  Pagina         │      │  "Registreer     │      │  Laadt              │
│                 │      │   hier"          │      │  • Vul email in     │
└─────────────────┘      └──────────────────┘      │  • Vul password in  │
                                                    │  • Dictionary       │
                                                    │    validatie        │
                                                    └──────────┬──────────┘
                                                               │
                                                               ▼
┌─────────────────┐      ┌──────────────────┐      ┌─────────────────────┐
│  6. Terug naar  │◄──   │  5. JSON request │◄──   │  4. Submit form     │
│  Login pagina   │      │  naar AuthAPI    │      │  • Stuur data       │
│                 │      │  • Hash password │      │  • Validatie check  │
│                 │      │  • Sla op in DB  │      │                     │
└─────────┬───────┘      └──────────────────┘      └─────────────────────┘
          │
          ▼
┌─────────────────┐      ┌──────────────────┐      ┌─────────────────────┐
│  7. Voer email  │──→   │  8. JSON request │──→   │  9. Wachtwoord      │
│  + password in  │      │  naar AuthAPI    │      │  verificatie        │
│                 │      │  • Query         │      │  • VerifyPassword() │
│                 │      │    database      │      │                     │
└─────────────────┘      └──────────────────┘      └──────────┬──────────┘
                                                               │
                                                      ┌────────┴────────┐
                                                      │                 │
                                                      ▼                 ▼
                                            ┌─────────────┐   ┌─────────────┐
                                            │ ✅ Match OK │   │ ❌ Verkeerd │
                                            └──────┬──────┘   └──────┬──────┘
                                                   │                 │
                                                   ▼                 ▼
                                            ┌─────────────┐   ┌─────────────┐
                                            │ Index       │   │ Error msg   │
                                            │ (ingelogd)  │   │ blijf login │
                                            └─────────────┘   └─────────────┘
```

---

## GAME FLOW

```
┌─────────────────┐      ┌──────────────────┐      ┌─────────────────────┐
│  1. Index       │──→   │  2. Klik PLAY    │──→   │  3. JSON request    │
│  Pagina         │      │  button          │      │  naar DataAPI       │
│  • Logo         │      │                  │      │  • GET /data        │
│  • Instructies  │      │                  │      │  • Ontvang lijst    │
│  • PLAY         │      │                  │      │    van IDs          │
└─────────────────┘      └──────────────────┘      └──────────┬──────────┘
                                                               │
                                                               ▼
┌─────────────────┐      ┌──────────────────┐      ┌─────────────────────┐
│  6. Toon quiz   │◄──   │  5. Initialiseer │◄──   │  4. Random          │
│  pagina         │      │  game state      │      │  selectie           │
│  • Afbeelding   │      │  • Pogingen = 0  │      │  • Kies random ID   │
│  • Input veld   │      │  • Hints = []    │      │  • Laad data        │
│  • CHECK/SKIP   │      │                  │      │                     │
└─────────┬───────┘      └──────────────────┘      └─────────────────────┘
          │
          │
    ┌─────┴──────┐
    │            │
    ▼            ▼
┌─────────┐  ┌─────────┐
│  CHECK  │  │  SKIP   │
│  button │  │  button │
└────┬────┘  └────┬────┘
     │            │
     │            └──────────────────────────────────────────┐
     │                                                       │
     ▼                                                       │
┌─────────────────┐                                         │
│  7. Vergelijk   │                                         │
│  antwoord       │                                         │
│  • GOED/FOUT    │                                         │
└────────┬────────┘                                         │
         │                                                  │
    ┌────┴─────┐                                            │
    │          │                                            │
    ▼          ▼                                            │
┌──────┐  ┌──────────────────┐                             │
│ GOED │  │  8. Check        │                             │
│      │  │  poging count    │                             │
└───┬──┘  │  • < 3 of >= 3   │                             │
    │     └────────┬─────────┘                             │
    │              │                                        │
    │     ┌────────┴────────┐                              │
    │     │                 │                              │
    │     ▼                 ▼                              │
    │ ┌──────────┐      ┌──────────┐                      │
    │ │ Poging   │      │ 3 pogingen│                      │
    │ │ < 3      │      │ bereikt   │                      │
    │ └────┬─────┘      └─────┬─────┘                      │
    │      │                  │                            │
    │      ▼                  │                            │
    │ ┌──────────┐            │                            │
    │ │ Toon hint│            │                            │
    │ │ 1: Artist│            │                            │
    │ │ 2: Jaar  │            │                            │
    │ │ 3: Genre │            │                            │
    │ │          │            │                            │
    │ │ Blijf op │            │                            │
    │ │ pagina   │            │                            │
    │ └──────────┘            │                            │
    │                         │                            │
    └─────────────────────────┴────────────────────────────┘
                              │
                              ▼
                        ┌─────────────────────────┐
                        │  9. Ga naar Endscreen   │
                        │  • Route juiste pagina  │
                        │  • MonaLisa / Starry    │
                        │    Night / NightWatch / │
                        │    Sunflowers           │
                        └────────────┬────────────┘
                                     │
                                     ▼
                        ┌─────────────────────────┐
                        │  10. Endscreen          │
                        │  • Toon details         │
                        │  • TERUG → Index        │
                        │  • NOG KEER → Nieuw spel│
                        └─────────────────────────┘
```

---

## API OVERZICHT

### DataAPI (localhost:8001)
- **Endpoint:** GET /data
- **Functie:** Retourneert lijst van schilderijen met unieke IDs

### AuthAPI (localhost:8002)
- **Endpoint:** POST /register
  - Ontvang email + password
  - Valideer en hash password
  - Sla op in database
  
- **Endpoint:** POST /login
  - Ontvang email + password
  - Verify credentials
  - Genereer en return access token

