using Microsoft.AspNetCore.Mvc;
using System.Xml.Serialization;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var list = new List<object>();

app.MapGet("/", () => Results.Redirect("/swagger"));


app.MapPost("/", ([FromHeader] bool xml = false) =>
{
    if (!xml)
        return Results.Ok(list);

    //CHATGPT
    var serializer = new XmlSerializer(typeof(List<object>), new XmlRootAttribute("Numbers"));
    using var stringWriter = new StringWriter();
    serializer.Serialize(stringWriter, list);

    return Results.Content(stringWriter.ToString(), "application/xml");
});

//CHATGPT
app.MapPut("/", ([FromForm] int quantity, [FromForm] string type) =>
{
    if (quantity <= 0)
        return Results.BadRequest(new { error = "'quantity' must be higher than zero" });

    if (type != "int" && type != "float")
        return Results.BadRequest(new { error = "'type' must be 'int' or 'float'" });

    var random = new Random();

    if (type == "int")
    {
        for (int i = 0; i < quantity; i++)
            list.Add(random.Next());
    }
    else
    {
        for (int i = 0; i < quantity; i++)
            list.Add(random.NextSingle());
    }

    return Results.Ok(list);
}).DisableAntiforgery();

app.MapDelete("/", ([FromForm] int quantity) =>
{
    if (quantity <= 0)
        return Results.BadRequest(new { error = "'quantity' must be higher than zero" });

    if (list.Count < quantity)
        return Results.BadRequest(new { error = "The list does not have enough elements" });

    for (int i = 0; i < quantity; i++)
        list.RemoveAt(0);

    return Results.Ok(list);
}).DisableAntiforgery();

app.MapPatch("/", () =>
{
    list.Clear();
    return Results.Ok(list);
}).DisableAntiforgery();

app.Run();
