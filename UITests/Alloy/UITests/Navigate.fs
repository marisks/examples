module Navigate

open canopy

let toAlloyPlan () =
    element ".nav"
    |> elementWithin "Alloy Plan"
    |> click

let toNth index =
    nth index ".nav > li a"
    |> click