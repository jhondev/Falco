﻿module Falco.Tests.Auth

open System.Security.Claims
open Falco
open Falco.Auth
open Falco.ViewEngine
open FSharp.Control.Tasks.V2.ContextInsensitive
open FsUnit.Xunit
open Xunit

[<Fact>] 
let ``ifNotAuthenticated calls authenticatedHandler if principal is authenticated`` () = 
    let ctx = getHttpContextWriteable true

    let expected = "hello"

    task {
        let! result = 
            (ifNotAuthenticated
                (textOut "hello")
                >=> textOut "world")
                shortCircuit ctx
        
        result.IsSome |> should equal true

        let! body = getBody result.Value
        
        body |> should equal expected        
    }

[<Fact>] 
let ``ifAuthenticated calls notAuthenticatedHandler if principal is not authenticated`` () = 
    let ctx = getHttpContextWriteable false

    let expected = "hello"

    task {
        let! result = 
            (ifAuthenticated
                (textOut "hello")
                >=> textOut "world")
                shortCircuit ctx
        
        result.IsSome |> should equal true

        let! body = getBody result.Value
        
        body |> should equal expected        
    }

[<Fact>]
let ``authHtmlOut properly threads through ClaimsPrincipal`` () =
    let ctx = getHttpContextWriteable true

    let expected = "<!DOCTYPE html><html><body><h1>hello</h1></body></html>"

    let doc (user : ClaimsPrincipal option) = 
        html [] [                        
                body [] [
                        if user.IsSome then yield h1 [] [ raw "hello" ]                
                    ]
        ]

    task {
        let! result = authHtmlOut doc shortCircuit ctx

        result.IsSome |> should equal true

        let! body = getBody result.Value
        
        body |> should equal expected        
    }