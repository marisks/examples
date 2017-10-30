module Pages

type CommonData = { url: string; heading: string }
type BasicPage = { common: CommonData }

let articlePage = {
    common = { url = "/article"; heading = "Article" }
    }

type HomePage = { common: CommonData; jumbotron: string }
let homePage = {
    common = { url = "/"; heading = "Home" };
    jumbotron = ".jumbotronblock"
    }