module HelloWorldApp 

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Falco

// ------------
// Logging
// ------------
let configureLogging (loggerBuilder : ILoggingBuilder) =
    loggerBuilder
        .AddFilter(fun l -> l.Equals LogLevel.Error)
        .AddConsole()
        .AddDebug() |> ignore

// ------------
// Services
// ------------
let configureServices (services : IServiceCollection) =
    services
        .AddResponseCaching()
        .AddResponseCompression()    
        .AddRouting()        
        |> ignore

// ------------
// Web App
// ------------

let helloHandler : HttpHandler =
    textOut "hello world"

let configureApp (app : IApplicationBuilder) =      
    let routes = [        
        get "/" helloHandler
    ]

    app.UseDeveloperExceptionPage()       
       .UseHttpEndPoints(routes)
       |> ignore

[<EntryPoint>]
let main _ =
    try
        WebHostBuilder()
            .UseKestrel()       
            .ConfigureLogging(configureLogging)
            .ConfigureServices(configureServices)
            .Configure(configureApp)          
            .Build()
            .Run()
        0
    with 
        | _ -> -1