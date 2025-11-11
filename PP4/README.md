# Dylan Candia Navas FI21021343

# Comandos dotnet utilizados


- `dotnet new sln -n  PP4`
- `dotnet new console -o PP4CodeFirst -f net8.0`
- `dotnet sln PP4.sln add PP4CodeFirst`
- `cd PP4CodeFirst`
- `dotnet add package Microsoft.EntityFrameworkCore`
- `dotnet add package Microsoft.EntityFrameworkCore.Sqlite`
- `dotnet add package Microsoft.EntityFrameworkCore.Design`
- `mkdir data`
- `mkdir models`
- `dotnet add package CsvHelper`

# Prompts

Tengo un proyecto en Code first y tengo que realizarlo utilizando entity framework que sea capaz de generar una base de datos sqlite llamada books.db dentro de una carpeta data, ademas de esto al agregar la base se debe poder leer un archivo csv proporcionado dentro del proyecto y ya al tener estos datos dentro de la bd guardarlos en formato tsv y Agregar Data Annotations en cada una de las tablas para indicar que todas las propiedades serán requeridas Utilizar la estrategia de Fluent API para que el nombre de la tabla TitleTag sea TitlesTags en la base de datos. Con la estrategia de Fluent API, definir el orden de las columnas de la tabla Title: TitleId AuthorId TitleName

Perfecto 👍
Vamos a hacerlo paso a paso, desde cero, para que tu proyecto de consola con .NET 8 + Entity Framework Core (Code First) y SQLite funcione perfectamente.

🧩 1. Crear el proyecto

Abre tu terminal (PowerShell o CMD) en la carpeta donde quieres trabajar y ejecuta:
```csharp
dotnet new console -n PP4CodeFirst
```

Luego entra en la carpeta del proyecto:

```csharp
cd PP4CodeFirst
```
📦 2. Instalar los paquetes necesarios

Entity Framework + SQLite necesitan estos paquetes:

```csharp
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

Estos te permitirán usar EF Core, conectarte a SQLite y ejecutar migraciones.

🗃️ 3. Crear la carpeta para la base de datos

Crea dentro del proyecto una carpeta llamada data:
```csharp
mkdir data
```

La base de datos books.db se generará ahí cuando ejecutes dotnet ef database update.

🧠 4. Crear las entidades (modelos)

Dentro de tu proyecto, crea una carpeta llamada Models y agrega los archivos que representen tus entidades.
Según las instrucciones, tendremos tres tablas: Author, Title y TitleTag (que se mapeará como TitlesTags).

📄 Models/Author.cs
```csharp
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PP4CodeFirst.Models
{
    public class Author
    {
        [Key]
        public int AuthorId { get; set; }

        [Required]
        public string AuthorName { get; set; } = string.Empty;

        public ICollection<Title> Titles { get; set; } = new List<Title>();
    }
}


📄 Models/Title.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace PP4CodeFirst.Models
{
    public class Title
    {
        [Key]
        public int TitleId { get; set; }

        [Required]
        [ForeignKey("Author")]
        public int AuthorId { get; set; }

        [Required]
        public string TitleName { get; set; } = string.Empty;

        public Author Author { get; set; }
        public ICollection<TitleTag> TitleTags { get; set; } = new List<TitleTag>();
    }
}


📄 Models/TitleTag.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PP4CodeFirst.Models
{
    public class TitleTag
    {
        [Key]
        public int TitleTagId { get; set; }

        [Required]
        [ForeignKey("Title")]
        public int TitleId { get; set; }

        [Required]
        public string Tag { get; set; } = string.Empty;

        public Title Title { get; set; }
    }
}

🧩 5. Crear el contexto (DbContext)

📄 Data/AppDbContext.cs

using Microsoft.EntityFrameworkCore;
using PP4CodeFirst.Models;
using System.IO;

namespace PP4CodeFirst.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<TitleTag> TitleTags { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dataPath = Path.Combine(Directory.GetCurrentDirectory(), "data", "books.db");
            optionsBuilder.UseSqlite($"Data Source={dataPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cambiar el nombre de la tabla TitleTag → TitlesTags
            modelBuilder.Entity<TitleTag>().ToTable("TitlesTags");

            // Definir el orden de columnas en Title
            modelBuilder.Entity<Title>()
                .Property(t => t.TitleId).HasColumnOrder(0);

            modelBuilder.Entity<Title>()
                .Property(t => t.AuthorId).HasColumnOrder(1);

            modelBuilder.Entity<Title>()
                .Property(t => t.TitleName).HasColumnOrder(2);
        }
    }
}

⚙️ 6. Preparar el entorno de migraciones

Instala las herramientas de EF (solo una vez en tu máquina, si no lo hiciste antes):

dotnet tool install --global dotnet-ef


Luego ejecuta dentro del proyecto:

dotnet build
dotnet ef migrations add InitialCreate
dotnet ef database update

```
Esto creará automáticamente la base de datos data/books.db.

✅ 7. Verificar que todo funciona

Puedes abrir el archivo books.db con DB Browser for SQLite o Visual Studio Code (con extensión SQLite Viewer) y comprobar que existen las tablas:

Authors

Titles

TitlesTags

Cuando tengas eso listo, seguimos con la siguiente parte del objetivo:
📂 Leer CSV y escribir TSV usando .NET 8.

¿Quieres que pasemos a esa parte (la lectura/escritura de archivos)?

Dijiste:
Como seria la parte del csv?

Perfecto 😎
Ya tienes tu base lista, así que ahora vamos con la segunda parte: leer el archivo CSV books.csv y cargar los datos en la base SQLite (books.db) con Entity Framework Code First.

🧩 1. Estructura esperada

Tu carpeta del proyecto debe verse así:
```csharp
PP4CodeFirst/
 ┣ data/
 ┃ ┣ books.db
 ┃ ┗ books.csv
 ┣ Models/
 ┣ Data/
 ┣ Program.cs
 ┗ PP4CodeFirst.csproj
```
📄 2. Ejemplo de contenido de books.csv

Debe estar en data/books.csv con este formato:

Author,Title,Tags
"Acosta, Soledad",La mujer en la sociedad moderna,Historia y Novela histórica|Ensayo|Autoayuda
Aeschylus,The persians,Literatura Universal
Aeschylus,Prometheus bound,Literatura Universal
"Aguinaldo, Emilio",Reseña verídica de la Revolución Filipina,Literatura Universal

⚙️ 3. Instalar paquete para leer CSV (opcional pero recomendado)

Vamos a usar CsvHelper, una librería potente y muy usada para leer archivos CSV de forma segura:

dotnet add package CsvHelper

📘 4. Código para leer el CSV y guardar en la base de datos

📄 Program.cs
```csharp
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using PP4CodeFirst.Data;
using PP4CodeFirst.Models;

class Program
{
    static void Main()
    {
        string dataPath = Path.Combine(Directory.GetCurrentDirectory(), "data");
        string csvPath = Path.Combine(dataPath, "books.csv");

        if (!File.Exists(csvPath))
        {
            Console.WriteLine("❌ No se encontró el archivo books.csv en la carpeta data.");
            return;
        }

        Console.WriteLine("📖 Leyendo archivo CSV...");

        // Configuración para leer correctamente comillas y delimitadores
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ",",
            IgnoreBlankLines = true,
            TrimOptions = TrimOptions.Trim
        };

        using var reader = new StreamReader(csvPath);
        using var csv = new CsvReader(reader, config);

        var records = csv.GetRecords<BookCsv>().ToList();

        using var context = new AppDbContext();

        foreach (var record in records)
        {
            // Buscar si el autor ya existe
            var author = context.Authors.FirstOrDefault(a => a.AuthorName == record.Author);
            if (author == null)
            {
                author = new Author { AuthorName = record.Author };
                context.Authors.Add(author);
                context.SaveChanges();
            }

            // Crear el título
            var title = new Title
            {
                TitleName = record.Title,
                AuthorId = author.AuthorId
            };
            context.Titles.Add(title);
            context.SaveChanges();

            // Crear las etiquetas (tags)
            var tags = record.Tags.Split('|', StringSplitOptions.RemoveEmptyEntries);
            foreach (var tag in tags)
            {
                var titleTag = new TitleTag
                {
                    TitleId = title.TitleId,
                    Tag = tag.Trim()
                };
                context.TitleTags.Add(titleTag);
            }

            context.SaveChanges();
        }

        Console.WriteLine("✅ Datos cargados correctamente en books.db");
    }
}

// Clase auxiliar para leer el CSV
public class BookCsv
{
    public string Author { get; set; }
    public string Title { get; set; }
    public string Tags { get; set; }
}
```
🧠 5. Qué hace este código

✔ Lee el archivo books.csv dentro de data/
✔ Usa CsvHelper para manejar correctamente comillas y separadores
✔ Inserta autores, títulos y etiquetas en la base SQLite
✔ Evita duplicar autores (verifica si ya existen antes de insertar)
✔ Crea las relaciones adecuadas usando EF Core

▶️ 6. Ejecución

Compila y ejecuta el proyecto:

dotnet build
dotnet run


Deberías ver algo como:

📖 Leyendo archivo CSV...
✅ Datos cargados correctamente en books.db

🧾 7. Verificar datos

Abre data/books.db en DB Browser for SQLite o VS Code y revisa:

Authors contiene autores únicos.

Titles tiene los títulos con AuthorId correcto.

TitlesTags contiene todas las etiquetas separadas.

Dijiste:
ok no esta funcionando probemos de otra manera

Perfecto 🔥 — vamos a probar de otra manera, bien estructurado, paso a paso, sin errores.
Te dejaré cada parte lista para copiar, ejecutar y probar.

🧱 PASO 1 — Crear el proyecto y entrar en la carpeta
```csharp
dotnet new console -n PP4CodeFirst
cd PP4CodeFirst
```

📦 PASO 2 — Instalar los paquetes necesarios

```csharp
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
```

(el último es para las migraciones)

📁 PASO 3 — Crear estructura de carpetas
```csharp
mkdir Data
mkdir Models
mkdir data
```

La carpeta data será donde estará books.csv y donde se creará books.db
📍 Debe quedar así:
```csharp
PP4CodeFirst/
 ┣ Data/
 ┣ Models/
 ┣ data/
 ┣ Program.cs
 ┣ PP4CodeFirst.csproj
```
🧩 PASO 4 — Crear las entidades
```csharp
📄 Models/Author.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PP4CodeFirst.Models
{
    public class Author
    {
        [Key]
        public int AuthorId { get; set; }

        [Required]
        public string AuthorName { get; set; } = string.Empty;

        public ICollection<Title> Titles { get; set; } = new List<Title>();
    }
}

📄 Models/Title.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PP4CodeFirst.Models
{
    public class Title
    {
        [Key]
        public int TitleId { get; set; }

        [Required]
        public int AuthorId { get; set; }

        [Required]
        public string TitleName { get; set; } = string.Empty;

        [ForeignKey("AuthorId")]
        public Author Author { get; set; } = null!;

        public ICollection<TitleTag> TitleTags { get; set; } = new List<TitleTag>();
    }
}

📄 Models/Tag.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PP4CodeFirst.Models
{
    public class Tag
    {
        [Key]
        public int TagId { get; set; }

        [Required]
        public string TagName { get; set; } = string.Empty;

        public ICollection<TitleTag> TitleTags { get; set; } = new List<TitleTag>();
    }
}

📄 Models/TitleTag.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PP4CodeFirst.Models
{
    public class TitleTag
    {
        [Key]
        public int TitleTagId { get; set; }

        [Required]
        public int TitleId { get; set; }

        [Required]
        public int TagId { get; set; }

        [ForeignKey("TitleId")]
        public Title Title { get; set; } = null!;

        [ForeignKey("TagId")]
        public Tag Tag { get; set; } = null!;
    }
}

🧠 PASO 5 — Crear el contexto
📄 Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using PP4CodeFirst.Models;
using System.IO;

namespace PP4CodeFirst.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TitleTag> TitlesTags { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Path.Combine("data", "books.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ✅ Cambiar el nombre de la tabla TitleTag → TitlesTags
            modelBuilder.Entity<TitleTag>().ToTable("TitlesTags");

            // ✅ Orden de columnas en Title
            modelBuilder.Entity<Title>(entity =>
            {
                entity.Property(e => e.TitleId).HasColumnOrder(1);
                entity.Property(e => e.AuthorId).HasColumnOrder(2);
                entity.Property(e => e.TitleName).HasColumnOrder(3);
            });
        }
    }
}
```
⚙️ PASO 6 — Crear la migración y la base de datos

```csharp
dotnet build
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Esto creará el archivo books.db dentro de /data.

📘 PASO 7 — Crear el Program.cs
📄 Program.cs

```csharp
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using PP4CodeFirst.Data;
using PP4CodeFirst.Models;

class Program
{
    static void Main()
    {
        using var context = new AppDbContext();

        if (!context.Authors.Any())
        {
            Console.WriteLine("La base de datos está vacía, por lo que será llenada a partir de los datos del archivo CSV.");
            Console.WriteLine("Procesando...");

            string csvPath = Path.Combine("data", "books.csv");

            if (!File.Exists(csvPath))
            {
                Console.WriteLine("❌ No se encontró el archivo books.csv en la carpeta data.");
                return;
            }

            var lines = File.ReadAllLines(csvPath).Skip(1); // Saltar encabezado
            foreach (var line in lines)
            {
                var parts = SplitCsvLine(line);
                if (parts.Length < 3) continue;

                string authorName = parts[0].Trim('"');
                string titleName = parts[1].Trim('"');
                string[] tagNames = parts[2].Split('|', StringSplitOptions.RemoveEmptyEntries);

                var author = context.Authors.FirstOrDefault(a => a.AuthorName == authorName)
                             ?? new Author { AuthorName = authorName };

                var title = new Title { TitleName = titleName, Author = author };

                foreach (var tagName in tagNames)
                {
                    var tag = context.Tags.FirstOrDefault(t => t.TagName == tagName.Trim())
                              ?? new Tag { TagName = tagName.Trim() };
                    context.TitlesTags.Add(new TitleTag { Title = title, Tag = tag });
                }

                context.Titles.Add(title);
            }

            context.SaveChanges();
            Console.WriteLine("✅ Datos cargados correctamente en books.db");
        }
        else
        {
            Console.WriteLine("La base de datos se está leyendo para crear los archivos TSV.");
            Console.WriteLine("Procesando...");

            var data = from author in context.Authors
                       from title in author.Titles
                       from tt in title.TitleTags
                       orderby author.AuthorName descending, title.TitleName descending, tt.Tag.TagName descending
                       select new
                       {
                           author.AuthorName,
                           title.TitleName,
                           tt.Tag.TagName
                       };

            var grouped = data.GroupBy(x => x.AuthorName[0]);

            foreach (var group in grouped)
            {
                string path = Path.Combine("data", $"{group.Key}.tsv");
                using var writer = new StreamWriter(path);
                writer.WriteLine("AuthorName\tTitleName\tTagName");

                foreach (var item in group)
                {
                    writer.WriteLine($"{item.AuthorName}\t{item.TitleName}\t{item.TagName}");
                }
            }

            Console.WriteLine("✅ Archivos TSV generados correctamente.");
        }
    }

    static string[] SplitCsvLine(string line)
    {
        var result = new List<string>();
        bool inQuotes = false;
        string current = "";

        foreach (char c in line)
        {
            if (c == '"') inQuotes = !inQuotes;
            else if (c == ',' && !inQuotes)
            {
                result.Add(current);
                current = "";
            }
            else current += c;
        }
        result.Add(current);
        return result.ToArray();
    }
}

```

🧾 PASO 8 — Coloca el archivo books.csv

Pega el archivo books.csv dentro de la carpeta:
```csharp
PP4CodeFirst/data/books.csv
```

▶️ PASO 9 — Ejecutar

Primera ejecución:

```csharp
dotnet run
```

Debería mostrar:

La base de datos está vacía, por lo que será llenada...
Procesando... ✅ Datos cargados correctamente en books.db


Segunda ejecución:
```csharp
dotnet run
```

Debería mostrar:

La base de datos se está leyendo para crear los archivos TSV.
Procesando... ✅ Archivos TSV generados correctamente.


Y en la carpeta /data aparecerán archivos como:

```csharp
A.tsv, B.tsv, C.tsv...
```

Dijiste:

```csharp
PS E:\Docs\Clase\PP4CodeFirst> dotnet run
La base de datos se está leyendo para crear los archivos TSV.
Procesando...
Unhandled exception. System.InvalidOperationException: Translating this query requires the SQL APPLY operation, which is not supported on SQLite.
```

Perfecto — el error que te aparece:

Translating this query requires the SQL APPLY operation, which is not supported on SQLite.

significa que la consulta LINQ que estás usando genera una operación “APPLY”, que SQLite no soporta.
Esto ocurre típicamente cuando haces combinaciones o subconsultas complejas con SelectMany, Join, o GroupBy en Entity Framework.

💡 Solución rápida

El truco es forzar que la parte compleja se ejecute en memoria (C#) en lugar de intentar traducirse a SQL.
Para eso, se usa .AsEnumerable() justo antes de la parte del LINQ que provoca el error.

✅ Ejemplo de corrección

Tu código probablemente tiene algo como esto (simplificado):
```csharp
var query = context.Authors
    .SelectMany(a => context.Titles.Where(t => t.AuthorId == a.AuthorId), (a, t) => new { a, t })
    .SelectMany(at => context.TitleTags.Where(tt => tt.TitleId == at.t.TitleId), (at, tt) => new { at, tt })
    .Join(context.Tags, att => att.tt.TagId, tag => tag.TagId, (att, tag) => new { att, tag })
    .OrderByDescending(x => x.att.at.a.AuthorName)
    .ThenByDescending(x => x.att.at.t.TitleName)
    .ThenByDescending(x => x.tag.TagName)
    .GroupBy(x => x.att.at.a.AuthorName[0], x => new {
        AuthorName = x.att.at.a.AuthorName,
        TitleName = x.att.at.t.TitleName,
        TagName = x.tag.TagName
    });
```

Para que funcione en SQLite, modifica así:

```csharp
var query = context.Authors
    .SelectMany(a => context.Titles.Where(t => t.AuthorId == a.AuthorId), (a, t) => new { a, t })
    .SelectMany(at => context.TitleTags.Where(tt => tt.TitleId == at.t.TitleId), (at, tt) => new { at, tt })
    .Join(context.Tags, att => att.tt.TagId, tag => tag.TagId, (att, tag) => new { att, tag })
    .AsEnumerable() // 👈 Forzamos a que lo siguiente se ejecute en memoria
    .OrderByDescending(x => x.att.at.a.AuthorName)
    .ThenByDescending(x => x.att.at.t.TitleName)
    .ThenByDescending(x => x.tag.TagName)
    .GroupBy(x => x.att.at.a.AuthorName[0], x => new {
        AuthorName = x.att.at.a.AuthorName,
        TitleName = x.att.at.t.TitleName,
        TagName = x.tag.TagName
    });
```

📘 Explicación

AsEnumerable() detiene la traducción a SQL y convierte el resultado actual a una colección en memoria.

Todo lo que va después se ejecuta con LINQ a objetos, no con LINQ to SQL.

Esto evita el error del SQL APPLY.

⚠️ Nota

Esto puede ser más lento si tu base tiene miles de registros, ya que descarga los datos primero.
Pero para fines académicos o pruebas, es totalmente válido y funcional.

¿Quieres que te pegue el Program.cs completo corregido con esta modificación incluida (ya listo para compilar)?


---


# Preguntas



¿Cómo cree que resultaría el uso de la estrategia de Code First para crear y actualizar una base de datos de tipo NoSQL (como por ejemplo MongoDB)? ¿Y con Database First? ¿Cree que habría complicaciones con las Foreign Keys?

Considero que dadas las complicaciones que habrían por el tipo de esquema que tiene una base NoSQL ya que estas no manejan llaves foráneas entonces habría que hacer las relaciones de forma manual lo cual no sería óptimo.

<br />


¿Cuál caracter, además de la coma (,) y el Tab (\t), se podría usar para separar valores en un archivo de texto con el objetivo de ser interpretado como una tabla (matriz)? ¿Qué extensión le pondría y por qué? Por ejemplo: Pipe (|) con extensión .pipe.

Utilizaría los corchetes cuadrados [] ya que se pueden interpretar fácilmente como una matriz dentro de los datos y seria con la extensión .brack para indicar que está delimitado con corchetes cuadrado.

<br />
