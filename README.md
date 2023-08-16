# Telemetry
Das ist mein Praktikumsprojekt

# Projektauftrag
## 1 Ausgangslage
Das Unternehmen  entwickelt Lösungen für Hausverwaltungen.
Die Lösungen bestehen sich aus verschieden Programmodulen, die für die unterschiedliche Aspekten die Beschäftigung dienen.
Um Benutzer zu unterstützen, führt das Unternehmen Schulungen und vorbereitet Personal für Help-Desk-Service.
Das bringt eine Kundenloyalität aber kostet Arbeitszeit und Geld.
Im Laufe der Zeit verwendet man einige Programmmodele öfter und andere seltener.
Das bedeutet Änderungen für die Schulungen und Help-Desk, einige Module brauchen mehr Aufmerksamkeit und Aufwand für andere soll reduziert.
Um zu bemerken, wie oft einzelne Modul aufgerufen worden, entwickelte das Unternehmen eine Telemetrie-System.
Ab 2020 generiert jedes abgerufene Modul Daten in JSON-Format, die wichtige Information über Programmzustand erhalten.
In Rahmen der Projektvorbereitung wurden die Daten bei mir in eine MariaDB -Datenbank übertragen und für das Projekt als Tabelle zur Verfügung bestellt.

## 2 Auftragstellung
Die Entwicklungsabteilung des Unternehmens braucht ein Werkzeug, das Aufwertung Telemetriedaten erlaubt. 
In Rahmen des Projekts soll nur ein Aspekt der Daten ausgewertet, nämlich Anzahl von Programmstarts.
Die Anwendung soll zehn oft benutzte und zehn am wenig benutzte Programm zeigen. 
Ein Benutzer soll die Anzahl der Programmstarts haben, sowohl für alle Kunden als auch für bestimmte Kunde. 
Die Daten sollen auch nach Jahren gruppiert werden.

## 3 Projektumfeld
Das Unternehmen verwendet C#-Framework ASP.NET Core als übliche Programmumgebung.
Als eine Datenbank soll MariaDB benutzt werden.
Für eine grafische Darstellung ist JavaScript-Framework Chart.js vorgeschlagen worden.
