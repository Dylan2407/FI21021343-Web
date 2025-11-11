using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PP4CodeFirst.Data;
using PP4CodeFirst.Models;

class Program
{
    static void Main()
    {
        using (var context = new AppDbContext())
        {
            context.Database.EnsureCreated();

            if (!context.Authors.Any())
            {
                Console.WriteLine("La base de datos está vacía, por lo que será llenada a partir de los datos del archivo CSV.");
                Console.WriteLine("Procesando...");

                var dataPath = Path.Combine("data", "books.csv");

                if (!File.Exists(dataPath))
                {
                    Console.WriteLine("❌ No se encontró el archivo books.csv en la carpeta data.");
                    return;
                }

                var lines = File.ReadAllLines(dataPath).Skip(1);

                foreach (var line in lines)
                {
                    var values = SplitCsvLine(line);

                    if (values.Length < 3) continue;

                    string authorName = values[0].Trim('"');
                    string titleName = values[1].Trim('"');
                    string[] tags = values[2].Split('|');

                    var author = context.Authors.FirstOrDefault(a => a.AuthorName == authorName);
                    if (author == null)
                    {
                        author = new Author { AuthorName = authorName };
                        context.Authors.Add(author);
                        context.SaveChanges();
                    }

                    var title = new Title { TitleName = titleName, AuthorId = author.AuthorId };
                    context.Titles.Add(title);
                    context.SaveChanges();

                    foreach (var tagName in tags)
                    {
                        var tag = context.Tags.FirstOrDefault(t => t.TagName == tagName);
                        if (tag == null)
                        {
                            tag = new Tag { TagName = tagName };
                            context.Tags.Add(tag);
                            context.SaveChanges();
                        }

                        context.TitleTags.Add(new TitleTag
                        {
                            TitleId = title.TitleId,
                            TagId = tag.TagId
                        });
                    }
                    context.SaveChanges();
                }

                Console.WriteLine("✅ Base de datos cargada exitosamente desde books.csv");
            }
            else
            {
                Console.WriteLine("La base de datos se está leyendo para crear los archivos TSV.");
                Console.WriteLine("Procesando...");

                
                var flatData = (
                    from a in context.Authors
                    join t in context.Titles on a.AuthorId equals t.AuthorId
                    join tt in context.TitleTags on t.TitleId equals tt.TitleId
                    join tag in context.Tags on tt.TagId equals tag.TagId
                    select new
                    {
                        a.AuthorName,
                        t.TitleName,
                        tag.TagName
                    }
                ).AsEnumerable();

                // 🔹 Agrupar en memoria
                var groupedData = flatData
                    .OrderByDescending(e => e.AuthorName)
                    .ThenByDescending(e => e.TitleName)
                    .ThenByDescending(e => e.TagName)
                    .GroupBy(e => e.AuthorName[0]);

                Directory.CreateDirectory("data");

                foreach (var group in groupedData)
                {
                    var filePath = Path.Combine("data", $"{group.Key}.tsv");
                    using (var writer = new StreamWriter(filePath))
                    {
                        writer.WriteLine("AuthorName\tTitleName\tTagName");
                        foreach (var item in group)
                        {
                            writer.WriteLine($"{item.AuthorName}\t{item.TitleName}\t{item.TagName}");
                        }
                    }
                }

                Console.WriteLine("✅ Archivos TSV generados correctamente.");
            }
        }
    }

    static string[] SplitCsvLine(string line)
    {
        var result = new System.Collections.Generic.List<string>();
        bool inQuotes = false;
        string current = "";

        foreach (char c in line)
        {
            if (c == '"' && !inQuotes)
            {
                inQuotes = true;
            }
            else if (c == '"' && inQuotes)
            {
                inQuotes = false;
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current);
                current = "";
            }
            else
            {
                current += c;
            }
        }
        result.Add(current);
        return result.ToArray();
    }
}
