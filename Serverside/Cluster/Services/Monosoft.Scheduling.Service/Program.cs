using System;

namespace Monosoft.Service.Scheduling
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            /*
Topic: scheduler.create.[ogranistation]
{
“id”: “729a0ec4-4a85-4933-809b-e0b9d452d1fe”
"name": "test schedule",
"description": "bla….",
"periode": { "startDate" : "NULL", ”endDate” : ”NULL” },
"Settings": { "every" : "1 week" , [ {"day" : "monday" } { ”day” : ”wednesday” }] },
"advSettings": { "repeatEvery" : "15 minutes", ”duration” : ”1 hour” },
"action": { "topic" : "database.insert.nst", "body" : "[bson]" }]
}
Topic: scheduler.read.[ogranistation]
{
“schedules” : [
“id”: “729a0ec4-4a85-4933-809b-e0b9d452d1fe”
"name": "test schedule",
"description": "bla….",
"periode": { "startDate" : "NULL", ”endDate” : ”NULL” },
"Settings": { "every" : "1 week" , [ {"day" : "monday" } { ”day” : ”wednesday” }] },
"advSettings": { "repeatEvery" : "15 minutes", ”duration” : ”1 hour” },
"action": { "topic" : "database.insert.nst", "body" : "[bson]" }]
]
}
Topic: scheduler.update.[ogranistation]
{
“id”: “729a0ec4-4a85-4933-809b-e0b9d452d1fe”
"name": "test schedule",
"description": "bla….",
"periode": { "startDate" : "NULL", ”endDate” : ”NULL” },
"Settings": { "every" : "1 week" , [ {"day" : "monday" } { ”day” : ”wednesday” }] },
"advSettings": { "repeatEvery" : "15 minutes", ”duration” : ”1 hour” },
"action": { "topic" : "database.insert.nst", "body" : "[bson]" }]
}
Topic: scheduler.[create / read / update / delete /].[ogranistation]
{
“id”: “729a0ec4-4a85-4933-809b-e0b9d452d1fe”
}

Der er et par ”intelligente” tekster i denne opsætning:
”settings.every”, Start interval, kan være: week, month, year
”settings.day”, Hvilke dage scheduleren må starte: monday to saturday
”advSettings.repeatevery”: indikere hvor ofte, på dagen.Beholdes blank hvis den kun skal kører én gang per dag. Eller er det hvert X second/minute/hour
”advSettings.duration”: Værdien sættes i enten: minute/hour
            */
        }
    }
}
