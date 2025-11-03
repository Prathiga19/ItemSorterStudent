# ItemSorterStudent – Week 7

Dette er mit projekt fra **Industrial Programming – Week 7**, hvor jeg har lavet et simpelt system, der styrer en *Item Sorter Robot* via URScript.

Programmet viser to DataGrids:
- **Queued Orders** (ordrer der venter)
- **Processed Orders** (ordrer der er færdige)

Når jeg klikker på **“Process next order”**, sender programmet automatisk URScript til robotten, så den "henter" varer fra bins og flytter dem til en forsendelsesboks.  
Hver ordrelinje bliver udført én ad gangen med små pauser, og jeg kan følge alt i **log-boksen** nederst.

Jeg har også lavet en **“Seed demo data”**-knap, som opretter to testordrer, så man kan se systemet i aktion uden at indtaste noget manuelt.

Koden er bygget i **Avalonia (.NET)** og kommunikerer med robotten via TCP på port **30002** (URScript) og **29999** (dashboard).  
Hvis jeg kører med URSim på min egen computer, bruger jeg `127.0.0.1` som IP-adresse.

---

### Hvordan man kører programmet
1. Åbn projektet i **Rider**
2. Sørg for at NuGet-pakken `Avalonia.Controls.DataGrid` er installeret
3. Tryk **Run ▶**
4. Klik *“Seed demo data”*
5. Klik *“Process next order”* for at se robotten (eller simuleringen) arbejde

---


