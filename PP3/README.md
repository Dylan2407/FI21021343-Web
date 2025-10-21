# Dylan Candia Navas FI21021343

# Comandos dotnet utilizados

- `dotnet new sln -n PP3`
- `dotnet new webapi -n WebApi`  
- `dotnet sln PP3.sln add WebApi`
- `cd WebApi` 

# Prompts

Tengo un proyecto en Minimal API y tengo que realizar un endpoint que se consume con HTTP 

para el endpoint de include se necesita controlar los errores  entonces el Response debe tener el Status Code 400 (Bad Request). Así mismo, todos los mensajes de error serán en formato JSON (independientemente del parámetro xml) y su llave/clave (Key) será error. Ejemplo: 

```json
{
  "error": "This is the error message"
}
```

y este replace tiene estas restricciones

position	La posición	int	Route	Igual o mayor que cero
value	La palabra a incluir	string	Query	Longitud mayor que cero
text	La oración	string	Form	Longitud mayor que cero
xml	El formato del resultado	bool	Headers	Ninguna ya que es opcional

esto se realiza con swagger y minimal api
ChatGPT dijo:
Perfecto. Para tu proyecto en Minimal API, puedes crear un endpoint include que cumpla exactamente con tus requerimientos: validar los parámetros, devolver errores en JSON con status 400, y opcionalmente responder en XML según el header xml. Aquí tienes un ejemplo funcional listo para usar con Swagger:


```csharp
using Microsoft.OpenApi.Models;
using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);

// Configurar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Text Processing API",
        Version = "v1",
        Description = "API con endpoint include"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ------------------------------------------------------------
// GET /
// Redirecciona a Swagger UI
// ------------------------------------------------------------
app.MapGet("/", () => Results.Redirect("/swagger"));

// ------------------------------------------------------------
// POST /include/{position:int}
// ------------------------------------------------------------
app.MapPost("/include/{position:int}", async (HttpContext context, int position) =>
{
    // Leer parámetros
    string? value = context.Request.Query["value"];
    string? text = context.Request.Form["text"];
    bool xml = context.Request.Headers.TryGetValue("xml", out var xmlHeader)
               && bool.TryParse(xmlHeader, out var xmlValue) && xmlValue;

    // ---------------------------
    // Validaciones (errores en JSON)
    // ---------------------------
    if (position < 0)
        return Results.BadRequest(new { error = "'position' must be 0 or higher" });

    if (string.IsNullOrWhiteSpace(value))
        return Results.BadRequest(new { error = "'value' cannot be empty" });

    if (string.IsNullOrWhiteSpace(text))
        return Results.BadRequest(new { error = "'text' cannot be empty" });

    // ---------------------------
    // Procesar texto (insertar palabra)
    // ---------------------------
    var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
    if (position > words.Count) position = words.Count;
    words.Insert(position, value);
    string newSentence = string.Join(' ', words);

    // ---------------------------
    // Generar respuesta
    // ---------------------------
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
        return Results.Json(new { ori = text, new_ = newSentence });
    }
})
.WithName("Include")
.WithSummary("Incluye una palabra en una posición específica de una oración.")
.WithDescription("Inserta una palabra en la posición indicada, según el texto recibido por Form, el valor por Query, y el formato opcional XML en el Header.");

app.Run();

```
---
🔹 Características
Parámetros:

position: desde la ruta, debe ser ≥ 0.

value: desde query, longitud > 0.

text: desde form, longitud > 0.

xml: opcional en headers, determina si la respuesta será en XML.

Errores:

Todos los errores devuelven JSON con clave "error" y status 400.

Procesamiento:

Inserta la palabra value en la posición indicada dentro de text.

Respuesta en JSON o XML según el header.

Si quieres, puedo hacer también un ejemplo con Postman mostrando cómo enviar text por el body y value por query para probarlo directamente. ¿Quieres que haga eso?

Dijiste:
al probar en postman aparece un \n porque pasa esto?
ChatGPT dijo:
Ah, eso sucede por cómo se está generando la respuesta XML. 😅

En el código que te di, la línea:

"<?xml version=\"1.0\" encoding=\"utf-16\"?>\n" + xmlResult.ToString()
incluye un salto de línea \n después de la declaración XML. Eso no afecta la validez del XML, pero si lo recibes en JSON o lo inspeccionas en Postman, verás ese \n como un carácter extra en la cadena.

Cómo evitarlo
Solo quita ese \n al concatenar la cadena:

return Results.Content("<?xml version=\"1.0\" encoding=\"utf-16\"?>" + xmlResult.ToString(), "application/xml");
O, si quieres que se vea más limpio con indentación, puedes usar:

return Results.Content(xmlResult.ToString(SaveOptions.None), "application/xml");
Así no tendrás el \n inesperado en la respuesta.

Si quieres, puedo reescribirte el endpoint completo con esta corrección aplicada. ¿Quieres que lo haga?

Dijiste:
igual aparece

ChatGPT dijo:
Ah, sí. Esto pasa porque el XElement.ToString() de .NET siempre agrega saltos de línea y sangrías internas para formatear el XML “bonito”. Por eso aunque quites el \n manual, todavía verás saltos de línea en la cadena resultante.

Vamos a limpiar tu endpoint para que eliminar cualquier salto de línea (\n, \r) y tabs de los valores recibidos, tanto de text como de value, de forma que la respuesta no aparezca con \n en Postman ni en JSON/XML.

Aquí te dejo la versión adaptada de tu /include:

```csharp
// ------------------------------------------------------------
// POST /include/{position:int}
// ------------------------------------------------------------
app.MapPost("/include/{position:int}", async (HttpContext context, int position) =>
{
    // Leer parámetros desde las ubicaciones especificadas
    string? value = context.Request.Query["value"];
    string? text = context.Request.Form["text"];
    bool xml = context.Request.Headers.TryGetValue("xml", out var xmlHeader)
        && bool.TryParse(xmlHeader, out var xmlValue)
        && xmlValue;

    // ---------------------------
    // Limpiar saltos de línea y tabs
    // ---------------------------
    if (!string.IsNullOrWhiteSpace(value))
        value = value.Replace("\r", "").Replace("\n", "").Replace("\t", "").Trim();

    if (!string.IsNullOrWhiteSpace(text))
        text = text.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim();

    // ---------------------------
    // Validaciones
    // ---------------------------
    if (position < 0)
    {
        var error = new { error = "'position' must be 0 or higher" };
        return xml
            ? Results.Content(ToXmlError(error.error), "application/xml")
            : Results.BadRequest(error);
    }

    if (string.IsNullOrWhiteSpace(value))
    {
        var error = new { error = "'value' cannot be empty" };
        return xml
            ? Results.Content(ToXmlError(error.error), "application/xml")
            : Results.BadRequest(error);
    }

    if (string.IsNullOrWhiteSpace(text))
    {
        var error = new { error = "'text' cannot be empty" };
        return xml
            ? Results.Content(ToXmlError(error.error), "application/xml")
            : Results.BadRequest(error);
    }

    // ---------------------------
    // Procesar texto
    // ---------------------------
    var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

    if (position > words.Count)
        position = words.Count;

    words.Insert(position, value);
    string newSentence = string.Join(' ', words);

    // ---------------------------
    // Generar respuesta
    // ---------------------------
    if (xml)
    {
        var xmlResult = new XElement("Result",
            new XElement("Ori", text),
            new XElement("New", newSentence)
        );

        return Results.Content(
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>\n" + xmlResult.ToString(),
            "application/xml"
        );
    }
    else
    {
        return Results.Json(new
        {
            ori = text,
            new_ = newSentence
        });
    }
})
.WithName("Include")
.WithSummary("Incluye una palabra en una posicion especifica de una oracion.")
.WithDescription("Inserta una palabra en la posicion indicada, segun el texto recibido por Form, el valor por Query, y el formato opcional XML en el Header.");

```

---

✅ Qué cambia:

text.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ") → elimina saltos de línea y tabs, reemplazándolos por espacios donde corresponda.

value.Replace("\r", "").Replace("\n", "").Replace("\t", "").Trim() → elimina todos los caracteres no deseados de la palabra a insertar.

Así, en Postman ya no aparecerá el \n en la respuesta JSON o XML.

Dijiste:
Si alguna validación falla, entonces el Response debe tener el Status Code 400 (Bad Request). Así mismo, todos los mensajes de error serán en formato JSON (independientemente del parámetro xml) y su llave/clave (Key) será error. Ejemplo:

{
  "error": "This is the error message"
}
ChatGPT dijo:
Perfecto, esto significa que el header xml solo aplica para respuestas exitosas, pero todos los errores deben devolverse siempre en JSON con 400 Bad Request y la clave "error".

Vamos a adaptar tus endpoints para cumplir con esto:

 /include actualizado:


```csharp
app.MapPost("/include/{position:int}", async (HttpContext context, int position) =>
{
    string? value = context.Request.Query["value"];
    string? text = context.Request.Form["text"];
    bool xml = context.Request.Headers.TryGetValue("xml", out var xmlHeader)
               && bool.TryParse(xmlHeader, out var xmlValue) && xmlValue;

    // Limpiar saltos de línea y tabs
    if (!string.IsNullOrWhiteSpace(value))
        value = value.Replace("\r", "").Replace("\n", "").Replace("\t", "").Trim();

    if (!string.IsNullOrWhiteSpace(text))
        text = text.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim();

    // Validaciones (si falla, siempre JSON)
    if (position < 0)
        return Results.BadRequest(new { error = "'position' must be 0 or higher" });

    if (string.IsNullOrWhiteSpace(value))
        return Results.BadRequest(new { error = "'value' cannot be empty" });

    if (string.IsNullOrWhiteSpace(text))
        return Results.BadRequest(new { error = "'text' cannot be empty" });

    // Procesar texto
    var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
    if (position > words.Count) position = words.Count;
    words.Insert(position, value);
    string newSentence = string.Join(' ', words);

    // Respuesta exitosa (JSON o XML según xml)
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
        return Results.Json(new { ori = text, new_ = newSentence });
    }
})
.WithName("Include")
.WithSummary("Incluye una palabra en una posicion especifica de una oracion.");

```

---

✅ Cambios importantes
Todos los errores devuelven JSON con status 400 y clave "error", sin importar el header xml.

Solo las respuestas exitosas pueden ser XML o JSON, según xml.

Limpieza de saltos de línea y tabs en todos los endpoints.


---


# Preguntas



¿Es posible enviar valores en el _Body_ (por ejemplo, en el _Form_) del _Request_ de tipo **GET**?

No, ya que un GET se encarga de recuperar los datos, no de enviarlos en el body para esto se utiliza el POST o PUT.


<br />


¿Qué ventajas y desventajas observa con el _Minimal API_ si se compara con la opción de utilizar _Controllers_?

Puede ser útil para proyectos pequeños en los que no se requiera mucho pero ya en un entorno con muchos más datos se dificulta el poder manejar muchos endpoints ya que no están tan estructurados como los controladores.

<br />

---
