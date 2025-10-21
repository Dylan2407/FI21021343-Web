using Microsoft.OpenApi.Models;
using System.Xml.Linq;


//Builder para la aplicacion web
var builder = WebApplication.CreateBuilder(args);

//services para Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{

    //Configuracion de Swagger/OpenAPI
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Text Processing API",
        Version = "v1",
    });
});


//se inicializa la aplicacion
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//se redirige la raiz al swagger
app.MapGet("/", () => Results.Redirect("/swagger"))
   .WithName("Root");


//Endpoint para incluir una palabra en una posicion dada
app.MapPost("/include/{position:int}", (HttpContext context, int position) =>


    {//Se llenan las variables con los datos recibidos
        string? value = context.Request.Query["value"];
        string? text = context.Request.Form["text"];
        bool xml = context.Request.Headers.TryGetValue("xml", out var xmlHeader)
                   && bool.TryParse(xmlHeader, out var xmlValue) && xmlValue;

        //se quitan saltos de linea y tabulaciones
        if (!string.IsNullOrWhiteSpace(value))
            value = value.Replace("\r", "").Replace("\n", "").Replace("\t", "").Trim();

        if (!string.IsNullOrWhiteSpace(text))
            text = text.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim();


        //si la posicion es negativa, se retorna un error
        if (position < 0)
            return Results.BadRequest(new { error = "'position' Tiene que ser 0 o mas" });

        //si el valor a incluir esta vacio, se retorna un error
        if (string.IsNullOrWhiteSpace(value))
            return Results.BadRequest(new { error = "'value' No puede estar vacio" });


        //si el texto esta vacio, se retorna un error
        if (string.IsNullOrWhiteSpace(text))
            return Results.BadRequest(new { error = "'text' No puede estar vacio" });

        //se divide el texto en palabras y se inserta la nueva palabra en la posicion dada
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
        if (position > words.Count) position = words.Count;
        words.Insert(position, value);
        string newSentence = string.Join(' ', words);

        //Si el elemento xml es true, se retorna el resultado en formato XML
        if (xml)
        {
            var xmlResult = new XElement("Result",
                new XElement("Ori", text),
                new XElement("New", newSentence)
            );
            //el resultado en formato XML se retorna aqui
            return Results.Content("<?xml version=\"1.0\" encoding=\"utf-16\"?>\n" + xmlResult.ToString(), "application/xml");
        }
        else
        {   //se muestra el resultado en formato JSON
            return Results.Json(new { ori = text, new_ = newSentence });
        }
    })

//name pare el swagger
.WithName("Include");

//Endpoint para reemplazar palabras de una longitud dada
app.MapPut("/replace/{length:int}", (HttpContext context, int length) =>
{

    //se llenan las variables con los datos recibidos
    string? value = context.Request.Query["value"];
    string? text = context.Request.Form["text"];
    bool xml = context.Request.Headers.TryGetValue("xml", out var xmlHeader)
               && bool.TryParse(xmlHeader, out var xmlValue) && xmlValue;


    //se quitan saltos de linea y tabulaciones
    if (!string.IsNullOrWhiteSpace(value))
        value = value.Replace("\r", "").Replace("\n", "").Replace("\t", "").Trim();

    if (!string.IsNullOrWhiteSpace(text))
        text = text.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim();

    //si la longitud es menor o igual a 0, se retorna un error
    if (length <= 0)
        return Results.BadRequest(new { error = "'length' Tiene que ser mas que 0" });

    //si el valor a incluir esta vacio, se retorna un error
    if (string.IsNullOrWhiteSpace(value))
        return Results.BadRequest(new { error = "'value' No puede estar vacio" });

    //si el texto esta vacio, se retorna un error
    if (string.IsNullOrWhiteSpace(text))
        return Results.BadRequest(new { error = "'text' No puede estar vacio" });


    //se divide el texto en palabras y se reemplazan las palabras de la longitud dada
    var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
    for (int i = 0; i < words.Count; i++)
        //si la palabra tiene la longitud dada, se reemplaza
        if (words[i].Length == length)
            words[i] = value;

    //se une la lista de palabras en una nueva oracion
    string newSentence = string.Join(' ', words);


    //Si el elemento xml es true, se retorna el resultado en formato XML
    if (xml)
    {
        var xmlResult = new XElement("Result",
            new XElement("Ori", text),
            new XElement("New", newSentence)
        );
        return Results.Content("<?xml version=\"1.0\" encoding=\"utf-16\"?>\n" + xmlResult.ToString(), "application/xml");
    }
    else
    {
        //si no, se retorna en formato JSON
        return Results.Json(new { ori = text, new_ = newSentence });
    }

})
//name dentro de swagger
.WithName("Replace");


//Endpoint para borrar palabras de una longitud dada
app.MapDelete("/erase/{length:int}", (HttpContext context, int length) =>
{

    //se llenan las variables con los datos recibidos
    string? text = context.Request.Form["text"];
    bool xml = context.Request.Headers.TryGetValue("xml", out var xmlHeader)
               && bool.TryParse(xmlHeader, out var xmlValue) && xmlValue;


    //se quitan saltos de linea y tabulaciones
    if (!string.IsNullOrWhiteSpace(text))
        text = text.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim();

    //si la longitud es menor o igual a 0, se retorna un error
    if (length <= 0)
        return Results.BadRequest(new { error = "'length' Tiene que ser mas que 0" });

    //si el texto esta vacio, se retorna un error
    if (string.IsNullOrWhiteSpace(text))
        return Results.BadRequest(new { error = "'text' No puede estar vacio" });


    //se divide el texto en palabras y se eliminan las palabras de la longitud dada
    var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Where(w => w.Length != length)
                    .ToList();

    //se une la lista de palabras en una nueva oracion
    string newSentence = string.Join(' ', words);

    //Si el elemento xml es true, se retorna el resultado en formato XML
    if (xml)
    {
        var xmlResult = new XElement("Result",
            new XElement("Ori", text),
            new XElement("New", newSentence)
        );
        return Results.Content("<?xml version=\"1.0\" encoding=\"utf-16\"?>\n" + xmlResult.ToString(), "application/xml");
    }
    else
    {   //si no, se retorna en formato JSON
        return Results.Json(new { ori = text, new_ = newSentence });
    }
})

//name dentro de swagger
.WithName("Erase");

//se inicia la aplicacion
app.Run();
