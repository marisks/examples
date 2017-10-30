module Common

open canopy

let mutable rootUrl = ""

let goto path = url (rootUrl + path)