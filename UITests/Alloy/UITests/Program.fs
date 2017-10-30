open canopy
open runner
open Common

rootUrl <- "http://localhost:50356"

start firefox

resize (1920, 1080)

//HomePageScenarios.all()
AlloyPlanScenarios.all()

run()

printfn "press [enter] to exit"
System.Console.ReadLine() |> ignore

quit()