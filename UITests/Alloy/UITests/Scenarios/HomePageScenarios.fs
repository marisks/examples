module HomePageScenarios

open canopy
open Common
open Pages

let positive _ =
    context "Positive home page tests"

    "When on home page" &&& fun _ ->
        goto homePage.common.url
    
    "it contains jumbotron" &&& fun _ ->
        displayed homePage.jumbotron
    
    "it contains Alloy Plan" &&& fun _ ->
        "h2" *~ "Alloy Plan"

    "it contains Alloy Track" &&& fun _ ->
        "h2" *~ "Alloy Track"

    "it contains Alloy Meet" &&& fun _ ->
        "h2" *~ "Alloy Meet"

let all _ =
    positive()