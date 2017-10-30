module AlloyPlanScenarios

open canopy
open Common
open Pages

let positive _ =
    context "Positive alloy plan page tests"

    "When on home page" &&& fun _ ->
        goto homePage.common.url
    
    "navigate to alloy plan" &&& fun _ ->
        Navigate.toAlloyPlan()
    
    "navigate to third navigation element" &&& fun _ ->
        Navigate.toNth 2

let all _ =
    positive()